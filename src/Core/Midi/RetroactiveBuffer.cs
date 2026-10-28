using System.Buffers.Binary;

namespace AnantaNada.Core.Midi;

/// <summary>
/// Lightweight 16-byte value type stored in the circular RAM buffer.
/// </summary>
public readonly struct MidiEventRecord
{
    public readonly long TimestampUs;
    public readonly byte Status;
    public readonly byte Data1;
    public readonly byte Data2;
    public readonly byte Channel;

    public MidiEventRecord(long timestampUs, byte status, byte data1, byte data2)
    {
        TimestampUs = timestampUs;
        Status = status;
        Data1 = data1;
        Data2 = data2;
        Channel = (byte)(status & 0x0F);
    }
}

/// <summary>
/// High-performance lock-free circular RAM buffer capturing all incoming MIDI events
/// for a continuous 5-minute sliding window ("Never-Lost Melody" scratchpad).
/// Serializes directly to valid Standard MIDI File (SMF Type 0) on demand.
/// </summary>
public sealed class RetroactiveBuffer
{
    // 2^18 = 262,144 events (~4MB RAM), comfortably holding >30 minutes of dense playing
    public const int Capacity = 262_144;
    private const int IndexMask = Capacity - 1;
    public const long FiveMinutesUs = 5L * 60L * 1_000_000L; // 300,000,000 microseconds

    private readonly MidiEventRecord[] _buffer = new MidiEventRecord[Capacity];
    private long _head = 0; // Total events pushed, monotonically increasing

    public int MaxCapacity => Capacity;
    public long TotalEventsCaptured => Interlocked.Read(ref _head);

    /// <summary>
    /// Pushes an incoming MIDI message into the circular buffer.
    /// Thread-safe and lock-free; zero allocation.
    /// </summary>
    public void Push(in MidiRawMessage message)
    {
        long index = Interlocked.Increment(ref _head) - 1;
        _buffer[index & IndexMask] = new MidiEventRecord(
            message.TimestampMicroseconds,
            message.Status,
            message.Data1,
            message.Data2
        );
    }

    /// <summary>
    /// Pushes raw values directly.
    /// </summary>
    public void Push(long timestampUs, byte status, byte data1, byte data2)
    {
        long index = Interlocked.Increment(ref _head) - 1;
        _buffer[index & IndexMask] = new MidiEventRecord(timestampUs, status, data1, data2);
    }

    /// <summary>
    /// Retrieves a chronologically sorted snapshot of events within the last <paramref name="durationMicroseconds"/> (defaults to 5 minutes).
    /// </summary>
    public List<MidiEventRecord> GetRecentEvents(long durationMicroseconds = FiveMinutesUs)
    {
        long currentHead = Interlocked.Read(ref _head);
        if (currentHead == 0) return new List<MidiEventRecord>();

        long eventsAvailable = Math.Min(currentHead, Capacity);
        long newestTimestamp = _buffer[(currentHead - 1) & IndexMask].TimestampUs;
        long cutoffTimestamp = newestTimestamp - durationMicroseconds;

        var result = new List<MidiEventRecord>((int)Math.Min(eventsAvailable, 32768));

        // Scan backwards from newest to cutoff
        for (long i = 1; i <= eventsAvailable; i++)
        {
            long idx = (currentHead - i) & IndexMask;
            var evt = _buffer[idx];

            if (evt.TimestampUs < cutoffTimestamp && durationMicroseconds > 0)
            {
                break;
            }
            result.Add(evt);
        }

        // Reverse to restore chronological order
        result.Reverse();
        return result;
    }

    /// <summary>
    /// Instantly serializes the captured buffer into a valid Standard MIDI File (SMF Format 0, .mid).
    /// Automatically resolves dangling Note Ons and computes variable-length delta times.
    /// </summary>
    /// <param name="filePath">Target destination file path.</param>
    /// <param name="durationMicroseconds">Time window to export (default 5 minutes).</param>
    /// <param name="targetBpm">Default tempo to embed (default 120 BPM).</param>
    /// <param name="ppqn">Pulses per quarter note resolution (default 480).</param>
    public bool ExportToMidi(string filePath, long durationMicroseconds = FiveMinutesUs, int targetBpm = 120, short ppqn = 480)
    {
        return ExportToMidi(filePath, out _, out _, durationMicroseconds, targetBpm, ppqn);
    }

    /// <summary>
    /// Instantly serializes the captured buffer into a valid Standard MIDI File (SMF Format 0, .mid),
    /// outputting the total captured events and estimated bars.
    /// </summary>
    public bool ExportToMidi(string filePath, out int eventCount, out int estimatedBars, long durationMicroseconds = FiveMinutesUs, int targetBpm = 120, short ppqn = 480)
    {
        eventCount = 0;
        estimatedBars = 0;

        var events = GetRecentEvents(durationMicroseconds);
        if (events.Count == 0)
        {
            return false;
        }

        eventCount = events.Count;
        long elapsedUs = Math.Max(0, events[^1].TimestampUs - events[0].TimestampUs);
        double elapsedSec = elapsedUs / 1_000_000.0;
        double barSec = (60.0 / targetBpm) * 4.0;
        estimatedBars = Math.Max(1, (int)Math.Round(elapsedSec / barSec));

        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new BinaryWriter(fileStream);

        // --- 1. WRITE HEADER CHUNK ('MThd') ---
        writer.Write(new byte[] { 0x4D, 0x54, 0x68, 0x64 }); // "MThd"
        WriteBigEndianUInt32(writer, 6);                     // Header length = 6
        WriteBigEndianUInt16(writer, 0);                     // Format 0 (Single Track)
        WriteBigEndianUInt16(writer, 1);                     // Track count = 1
        WriteBigEndianUInt16(writer, (ushort)ppqn);          // PPQN division

        // --- 2. COMPILE TRACK DATA ('MTrk') IN MEMORY ---
        using var trackStream = new MemoryStream(events.Count * 8);
        using var trackWriter = new BinaryWriter(trackStream);

        // Microseconds per quarter note: 60,000,000 / BPM (e.g. 120 BPM = 500,000 us)
        int usPerQuarter = 60_000_000 / targetBpm;
        double usPerTick = (double)usPerQuarter / ppqn;

        // Track active note-ons to resolve any dangling sustained notes
        // Key: (Channel << 8) | NoteNumber
        var activeNotes = new HashSet<int>();

        long firstTimestampUs = events[0].TimestampUs;
        long lastTick = 0;

        // Meta-event: Set Tempo at delta 0
        WriteVariableLengthQuantity(trackWriter, 0); // Delta 0
        trackWriter.Write((byte)0xFF);               // Meta event
        trackWriter.Write((byte)0x51);               // Set Tempo
        trackWriter.Write((byte)0x03);               // Length 3
        trackWriter.Write((byte)((usPerQuarter >> 16) & 0xFF));
        trackWriter.Write((byte)((usPerQuarter >> 8) & 0xFF));
        trackWriter.Write((byte)(usPerQuarter & 0xFF));

        // Meta-event: Track Name ("AnantaNada Scratchpad")
        byte[] trackName = System.Text.Encoding.ASCII.GetBytes("AnantaNada Go:Keys 3 Retroactive Capture");
        WriteVariableLengthQuantity(trackWriter, 0);
        trackWriter.Write((byte)0xFF);
        trackWriter.Write((byte)0x03);
        WriteVariableLengthQuantity(trackWriter, (uint)trackName.Length);
        trackWriter.Write(trackName);

        for (int i = 0; i < events.Count; i++)
        {
            var evt = events[i];
            long eventElapsedUs = Math.Max(0, evt.TimestampUs - firstTimestampUs);
            long currentTick = (long)Math.Round(eventElapsedUs / usPerTick);

            long deltaTick = Math.Max(0, currentTick - lastTick);
            lastTick = currentTick;

            byte messageType = (byte)(evt.Status & 0xF0);
            int noteKey = (evt.Channel << 8) | evt.Data1;

            if (messageType == MidiConstants.NoteOn && evt.Data2 > 0)
            {
                activeNotes.Add(noteKey);
            }
            else if (messageType == MidiConstants.NoteOff || (messageType == MidiConstants.NoteOn && evt.Data2 == 0))
            {
                activeNotes.Remove(noteKey);
            }

            // Write delta time
            WriteVariableLengthQuantity(trackWriter, (uint)deltaTick);

            // Write MIDI status and data bytes
            trackWriter.Write(evt.Status);
            trackWriter.Write(evt.Data1);

            // Program Change and Channel Pressure only use 1 data byte
            if (messageType != MidiConstants.ProgramChange && messageType != MidiConstants.ChannelPressure)
            {
                trackWriter.Write(evt.Data2);
            }
        }

        // Failsafe: Close all hanging Note-Ons so notes never stick in DAWs
        foreach (int key in activeNotes)
        {
            byte channel = (byte)((key >> 8) & 0x0F);
            byte note = (byte)(key & 0x7F);

            WriteVariableLengthQuantity(trackWriter, 0); // delta 0
            trackWriter.Write((byte)(MidiConstants.NoteOff | channel));
            trackWriter.Write(note);
            trackWriter.Write((byte)64); // standard release velocity
        }

        // Meta-event: End of Track (00 FF 2F 00)
        WriteVariableLengthQuantity(trackWriter, 120); // brief trailing padding
        trackWriter.Write((byte)0xFF);
        trackWriter.Write((byte)0x2F);
        trackWriter.Write((byte)0x00);

        // --- 3. WRITE TRACK CHUNK TO FILE ---
        byte[] trackBytes = trackStream.ToArray();
        writer.Write(new byte[] { 0x4D, 0x54, 0x72, 0x6B }); // "MTrk"
        WriteBigEndianUInt32(writer, (uint)trackBytes.Length);
        writer.Write(trackBytes);

        writer.Flush();
        return true;
    }

    private static void WriteVariableLengthQuantity(BinaryWriter writer, uint value)
    {
        uint buffer = value & 0x7F;
        while ((value >>= 7) > 0)
        {
            buffer <<= 8;
            buffer |= 0x80;
            buffer += (value & 0x7F);
        }

        while (true)
        {
            writer.Write((byte)(buffer & 0xFF));
            if ((buffer & 0x80) != 0)
            {
                buffer >>= 8;
            }
            else
            {
                break;
            }
        }
    }

    private static void WriteBigEndianUInt32(BinaryWriter writer, uint value)
    {
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }

    private static void WriteBigEndianUInt16(BinaryWriter writer, ushort value)
    {
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
    }
}
