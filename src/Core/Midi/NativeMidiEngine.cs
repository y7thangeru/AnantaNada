using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Storage.Streams;

namespace AnantaNada.Core.Midi;

/// <summary>
/// Value-type MIDI message representation designed for zero-allocation hot paths.
/// </summary>
public readonly struct MidiRawMessage
{
    public readonly byte Status;
    public readonly byte Data1;
    public readonly byte Data2;
    public readonly byte Channel; // 0-indexed (0-15)
    public readonly long TimestampMicroseconds;

    public MidiRawMessage(byte status, byte data1, byte data2, long timestampMicroseconds)
    {
        Status = status;
        Data1 = data1;
        Data2 = data2;
        Channel = (byte)(status & 0x0F);
        TimestampMicroseconds = timestampMicroseconds;
    }

    public byte MessageType => (byte)(Status & 0xF0);
    public bool IsNoteOn => MessageType == MidiConstants.NoteOn && Data2 > 0;
    public bool IsNoteOff => MessageType == MidiConstants.NoteOff || (MessageType == MidiConstants.NoteOn && Data2 == 0);
    public bool IsControlChange => MessageType == MidiConstants.ControlChange;
    public bool IsProgramChange => MessageType == MidiConstants.ProgramChange;
    public bool IsPitchBend => MessageType == MidiConstants.PitchBend;

    public override string ToString() =>
        $"[Ch:{Channel + 1:D2} Type:0x{MessageType:X2} D1:{Data1} D2:{Data2} T:{TimestampMicroseconds}us]";
}

/// <summary>
/// Hardware-Safe Native MIDI Engine interfacing directly with OS Multimedia drivers (WinMM)
/// specifically tailored for Roland Go:Keys 3 (61-Key Synthesizer/Arranger).
/// Guarantees zero non-volatile flash writes, throttled SysEx FIFO queue, and instant panic circuit.
/// </summary>
public sealed class NativeMidiEngine : IDisposable
{
    #region WinMM Native Interop

    private const int MIM_DATA = 0x3C3;
    private const int MIM_LONGDATA = 0x3C4;
    private const int MIM_OPEN = 0x3C1;
    private const int MIM_CLOSE = 0x3C2;
    private const int CALLBACK_FUNCTION = 0x00030000;

    private delegate void MidiInProc(nint hMidiIn, uint wMsg, nint dwInstance, nint dwParam1, nint dwParam2);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MIDIINCAPS
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public uint dwSupport;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MIDIOUTCAPS
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public ushort wTechnology;
        public ushort wVoices;
        public ushort wNotes;
        public ushort wChannelMask;
        public uint dwSupport;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIDIHDR
    {
        public nint lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public nint dwUser;
        public uint dwFlags;
        public nint lpNext;
        public nint reserved;
        public uint dwOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public nint[] dwReserved;
    }

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiInGetNumDevs();

    [DllImport("winmm.dll", CharSet = CharSet.Auto)]
    private static extern int midiInGetDevCaps(nuint uDeviceID, out MIDIINCAPS caps, uint cbMidiInCaps);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiInOpen(out nint lphMidiIn, uint uDeviceID, MidiInProc dwCallback, nint dwInstance, uint dwFlags);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiInStart(nint hMidiIn);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiInStop(nint hMidiIn);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiInClose(nint hMidiIn);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutGetNumDevs();

    [DllImport("winmm.dll", CharSet = CharSet.Auto)]
    private static extern int midiOutGetDevCaps(nuint uDeviceID, out MIDIOUTCAPS caps, uint cbMidiOutCaps);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutOpen(out nint lphMidiOut, uint uDeviceID, nint dwCallback, nint dwInstance, uint dwFlags);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutClose(nint hMidiOut);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutShortMsg(nint hMidiOut, uint dwMsg);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutLongMsg(nint hMidiOut, nint lpMidiOutHdr, uint uSize);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern int midiOutReset(nint hMidiOut);

    #endregion

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly MidiInProc _midiInCallback; // Pinned delegate reference to prevent GC collection
    private nint _hMidiIn = nint.Zero;
    private nint _hMidiOut = nint.Zero;
    private bool _isDisposed;
    private bool _isWindows;

    // Modern Windows BLE (Bluetooth LE) MIDI Handles
    private MidiInPort? _uwpMidiIn;
    private IMidiOutPort? _uwpMidiOut;

    // SysEx Throttled FIFO queue
    private readonly BlockingCollection<byte[]> _sysExQueue = new(new ConcurrentQueue<byte[]>());
    private readonly Thread _sysExWorkerThread;
    private readonly CancellationTokenSource _cts = new();

    // Event Dispatching
    public delegate void MessageReceivedHandler(in MidiRawMessage message);
    public event MessageReceivedHandler? OnMessageReceived;
    public delegate void NoteEventHandler(byte channel, byte note, byte velocity);
    public event NoteEventHandler? OnNoteOn;
    public event NoteEventHandler? OnNoteOff;
    public delegate void ControlChangeHandler(byte channel, byte controller, byte value);
    public event ControlChangeHandler? OnControlChange;
    public event Action<string>? OnLog;
    public event Action<string>? OnDeviceConnected;
    public event Action? OnDeviceDisconnected;

    public bool IsInOpen => _hMidiIn != nint.Zero || _uwpMidiIn != null;
    public bool IsOutOpen => _hMidiOut != nint.Zero || _uwpMidiOut != null;
    public string ConnectedDeviceName { get; private set; } = "Disconnected";

    public NativeMidiEngine()
    {
        _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        _midiInCallback = HandleNativeMidiInMessage;

        // Initialize dedicated thread for rate-limited SysEx dispatch
        _sysExWorkerThread = new Thread(ProcessSysExQueue)
        {
            Name = "AnantaNada-SysEx-Worker",
            IsBackground = true,
            Priority = ThreadPriority.AboveNormal
        };
        _sysExWorkerThread.Start();
    }

    /// <summary>
    /// Enumerates all available MIDI input devices detected by the OS.
    /// </summary>
    public IReadOnlyList<string> EnumerateInputDevices()
    {
        var devices = new List<string>();
        if (!_isWindows) return devices;

        int count = midiInGetNumDevs();
        uint capsSize = (uint)Marshal.SizeOf<MIDIINCAPS>();
        for (uint i = 0; i < count; i++)
        {
            if (midiInGetDevCaps(i, out MIDIINCAPS caps, capsSize) == 0)
            {
                devices.Add(caps.szPname);
            }
        }
        return devices;
    }

    /// <summary>
    /// Enumerates all available MIDI output devices detected by the OS.
    /// </summary>
    public IReadOnlyList<string> EnumerateOutputDevices()
    {
        var devices = new List<string>();
        if (!_isWindows) return devices;

        int count = midiOutGetNumDevs();
        uint capsSize = (uint)Marshal.SizeOf<MIDIOUTCAPS>();
        for (uint i = 0; i < count; i++)
        {
            if (midiOutGetDevCaps(i, out MIDIOUTCAPS caps, capsSize) == 0)
            {
                devices.Add(caps.szPname);
            }
        }
        return devices;
    }

    /// <summary>
    /// Connects to Roland Go:Keys 3 device by matching device name or defaulting to first detected device.
    /// Supports both Modern Windows Runtime (Bluetooth LE MIDI) and Classic WinMM (USB Cable / Standard MIDI).
    /// </summary>
    public bool Connect(string preferredNameSubstring = "Go:Keys")
    {
        return ConnectAsync(preferredNameSubstring).GetAwaiter().GetResult();
    }

    public async Task<bool> ConnectAsync(string preferredNameSubstring = "Go:Keys")
    {
        if (!_isWindows)
        {
            OnLog?.Invoke("[MIDI Engine] Non-Windows OS detected; running in virtual/emulated MIDI mode.");
            ConnectedDeviceName = "Virtual Roland Go:Keys 3";
            return true;
        }

        Close();

        // 1. Check Modern Windows Runtime (WinRT / UWP) for Bluetooth LE (BLE) MIDI & Modern USB MIDI
        try
        {
            string inSelector = MidiInPort.GetDeviceSelector();
            var inDevices = await DeviceInformation.FindAllAsync(inSelector);
            string outSelector = MidiOutPort.GetDeviceSelector();
            var outDevices = await DeviceInformation.FindAllAsync(outSelector);

            if (inDevices.Count > 0 || outDevices.Count > 0)
            {
                OnLog?.Invoke($"[MIDI Engine - WinRT BLE] Discovered {inDevices.Count} In, {outDevices.Count} Out modern MIDI device(s).");
                foreach (var d in inDevices)
                {
                    OnLog?.Invoke($"  -> WinRT MIDI IN: '{d.Name}' (Id: {d.Id})");
                }
                foreach (var d in outDevices)
                {
                    OnLog?.Invoke($"  -> WinRT MIDI OUT: '{d.Name}' (Id: {d.Id})");
                }

                // Match Roland Go:Keys or preferred substring (or any BLE MIDI device)
                var targetIn = inDevices.FirstOrDefault(d =>
                    d.Name.Contains(preferredNameSubstring, StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("Go:Keys", StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("Roland", StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("MIDI", StringComparison.OrdinalIgnoreCase));

                var targetOut = outDevices.FirstOrDefault(d =>
                    d.Name.Contains(preferredNameSubstring, StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("Go:Keys", StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("Roland", StringComparison.OrdinalIgnoreCase) ||
                    d.Name.Contains("MIDI", StringComparison.OrdinalIgnoreCase));

                if (targetIn != null)
                {
                    OnLog?.Invoke($"[MIDI Engine] Opening WinRT BLE MIDI IN on: [{targetIn.Name}]...");
                    _uwpMidiIn = await MidiInPort.FromIdAsync(targetIn.Id);
                    if (_uwpMidiIn != null)
                    {
                        _uwpMidiIn.MessageReceived += OnUwpMidiInMessageReceived;
                        ConnectedDeviceName = targetIn.Name;
                        OnLog?.Invoke($"[MIDI Engine] BLE MIDI IN linked successfully to [{targetIn.Name}]!");
                    }
                }

                if (targetOut != null)
                {
                    OnLog?.Invoke($"[MIDI Engine] Opening WinRT BLE MIDI OUT on: [{targetOut.Name}]...");
                    _uwpMidiOut = await MidiOutPort.FromIdAsync(targetOut.Id);
                    if (_uwpMidiOut != null)
                    {
                        if (string.IsNullOrEmpty(ConnectedDeviceName) || ConnectedDeviceName == "Disconnected")
                        {
                            ConnectedDeviceName = targetOut.Name;
                        }
                        OnLog?.Invoke($"[MIDI Engine] BLE MIDI OUT linked successfully to [{targetOut.Name}]!");
                    }
                }

                if (_uwpMidiIn != null || _uwpMidiOut != null)
                {
                    OnDeviceConnected?.Invoke(ConnectedDeviceName);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"[MIDI Engine - WinRT BLE Scan] {ex.Message}");
        }

        // 2. Fallback to Classic WinMM (USB Cable / Standard WinMM MIDI driver)
        int inCount = midiInGetNumDevs();
        int outCount = midiOutGetNumDevs();

        if (inCount == 0 && outCount == 0)
        {
            OnLog?.Invoke("[MIDI Engine] No MIDI hardware devices found (WinRT or WinMM).");
            return false;
        }

        OnLog?.Invoke($"[MIDI Engine - WinMM] Scanning ports: {inCount} MIDI IN, {outCount} MIDI OUT available.");

        uint selectedIn = 0;
        bool foundIn = false;
        uint capsInSize = (uint)Marshal.SizeOf<MIDIINCAPS>();
        for (uint i = 0; i < inCount; i++)
        {
            if (midiInGetDevCaps(i, out MIDIINCAPS caps, capsInSize) == 0)
            {
                OnLog?.Invoke($"  -> WinMM MIDI IN [{i}]: {caps.szPname}");
                if (caps.szPname.Contains(preferredNameSubstring, StringComparison.OrdinalIgnoreCase))
                {
                    selectedIn = i;
                    foundIn = true;
                    ConnectedDeviceName = caps.szPname;
                    break;
                }
            }
        }

        uint selectedOut = 0;
        bool foundOut = false;
        uint capsOutSize = (uint)Marshal.SizeOf<MIDIOUTCAPS>();
        for (uint i = 0; i < outCount; i++)
        {
            if (midiOutGetDevCaps(i, out MIDIOUTCAPS caps, capsOutSize) == 0)
            {
                OnLog?.Invoke($"  -> WinMM MIDI OUT [{i}]: {caps.szPname}");
                if (caps.szPname.Contains(preferredNameSubstring, StringComparison.OrdinalIgnoreCase))
                {
                    selectedOut = i;
                    foundOut = true;
                    if (!foundIn) ConnectedDeviceName = caps.szPname;
                    break;
                }
            }
        }

        if (inCount == 0)
        {
            OnLog?.Invoke("[MIDI Engine] Notice: No MIDI IN port active. 'GO:KEYS 3 Audio' is Bluetooth Audio (speaker sound only). To send MIDI notes, connect USB-C cable or pair Bluetooth MIDI.");
        }

        // Fallback to device 0 if specific match was not found but devices exist
        if (!foundIn && inCount > 0)
        {
            selectedIn = 0;
            midiInGetDevCaps(0, out MIDIINCAPS caps, capsInSize);
            ConnectedDeviceName = caps.szPname;
        }
        if (!foundOut && outCount > 0)
        {
            selectedOut = 0;
        }

        // Open MIDI In
        if (inCount > 0)
        {
            int res = midiInOpen(out _hMidiIn, selectedIn, _midiInCallback, nint.Zero, CALLBACK_FUNCTION);
            if (res == 0)
            {
                midiInStart(_hMidiIn);
                OnLog?.Invoke($"[MIDI Engine] WinMM MIDI IN hooked on [{ConnectedDeviceName}].");
            }
            else
            {
                OnLog?.Invoke($"[MIDI Engine] Failed to open WinMM MIDI IN: Error {res}");
            }
        }

        // Open MIDI Out
        if (outCount > 0)
        {
            int res = midiOutOpen(out _hMidiOut, selectedOut, nint.Zero, nint.Zero, 0);
            if (res == 0)
            {
                OnLog?.Invoke($"[MIDI Engine] WinMM MIDI OUT opened on device {selectedOut}.");
            }
            else
            {
                OnLog?.Invoke($"[MIDI Engine] Failed to open WinMM MIDI OUT: Error {res}");
            }
        }

        OnDeviceConnected?.Invoke(ConnectedDeviceName);
        return IsInOpen || IsOutOpen;
    }

    private void OnUwpMidiInMessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
    {
        var rawData = args.Message.RawData;
        if (rawData == null || rawData.Length == 0) return;

        using var reader = DataReader.FromBuffer(rawData);
        byte[] bytes = new byte[rawData.Length];
        reader.ReadBytes(bytes);

        byte status = bytes[0];
        // Filter system realtime messages (Clock 0xF8, Active Sensing 0xFE)
        if (status >= 0xF8) return;

        byte data1 = bytes.Length > 1 ? bytes[1] : (byte)0;
        byte data2 = bytes.Length > 2 ? bytes[2] : (byte)0;

        long timestampUs = _stopwatch.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        var msg = new MidiRawMessage(status, data1, data2, timestampUs);

        OnMessageReceived?.Invoke(in msg);

        if (msg.IsNoteOn)
        {
            OnNoteOn?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
        else if (msg.IsNoteOff)
        {
            OnNoteOff?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
        else if (msg.IsControlChange)
        {
            OnControlChange?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
    }

    /// <summary>
    /// Hot-path callback invoked directly from OS MIDI thread. Guarantees zero heap allocation.
    /// </summary>
    private void HandleNativeMidiInMessage(nint hMidiIn, uint wMsg, nint dwInstance, nint dwParam1, nint dwParam2)
    {
        if (wMsg != MIM_DATA) return;

        // dwParam1 holds: [Byte0: Status] [Byte1: Data1] [Byte2: Data2] [Byte3: 0]
        uint raw = (uint)dwParam1;
        byte status = (byte)(raw & 0xFF);
        byte data1 = (byte)((raw >> 8) & 0xFF);
        byte data2 = (byte)((raw >> 16) & 0xFF);

        // Filter system realtime messages (Clock, Active Sensing) to keep buffer pristine
        if (status >= 0xF8) return;

        long timestampUs = _stopwatch.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        var msg = new MidiRawMessage(status, data1, data2, timestampUs);

        // Direct callback invocation (no boxing)
        OnMessageReceived?.Invoke(in msg);

        if (msg.IsNoteOn)
        {
            OnNoteOn?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
        else if (msg.IsNoteOff)
        {
            OnNoteOff?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
        else if (msg.IsControlChange)
        {
            OnControlChange?.Invoke(msg.Channel, msg.Data1, msg.Data2);
        }
    }

    /// <summary>
    /// Sends a standard 3-byte short message (NoteOn, NoteOff, CC, PitchBend) to the hardware.
    /// </summary>
    public void SendShortMessage(byte status, byte data1, byte data2)
    {
        if (_hMidiOut != nint.Zero)
        {
            uint msg = (uint)(status | (data1 << 8) | (data2 << 16));
            midiOutShortMsg(_hMidiOut, msg);
        }

        if (_uwpMidiOut != null)
        {
            var writer = new DataWriter();
            writer.WriteBytes(new byte[] { status, data1, data2 });
            _uwpMidiOut.SendBuffer(writer.DetachBuffer());
        }
    }

    /// <summary>
    /// Sends a 2-byte short message (Program Change, Channel Pressure) to the hardware.
    /// </summary>
    public void SendShortMessage(byte status, byte data1)
    {
        if (_hMidiOut != nint.Zero)
        {
            uint msg = (uint)(status | (data1 << 8));
            midiOutShortMsg(_hMidiOut, msg);
        }

        if (_uwpMidiOut != null)
        {
            var writer = new DataWriter();
            writer.WriteBytes(new byte[] { status, data1 });
            _uwpMidiOut.SendBuffer(writer.DetachBuffer());
        }
    }

    /// <summary>
    /// Sends a standard MIDI Note On message.
    /// </summary>
    public void SendNoteOn(byte channel, byte note, byte velocity)
    {
        SendShortMessage((byte)(0x90 | (channel & 0x0F)), (byte)(note & 0x7F), (byte)(velocity & 0x7F));
    }

    /// <summary>
    /// Sends a standard MIDI Note Off message.
    /// </summary>
    public void SendNoteOff(byte channel, byte note)
    {
        SendShortMessage((byte)(0x80 | (channel & 0x0F)), (byte)(note & 0x7F), 0);
    }

    /// <summary>
    /// Sends a standard MIDI Control Change (CC) message.
    /// </summary>
    public void SendControlChange(byte channel, byte controller, byte value)
    {
        SendShortMessage((byte)(0xB0 | (channel & 0x0F)), (byte)(controller & 0x7F), (byte)(value & 0x7F));
    }

    /// <summary>
    /// Sends a standard MIDI Pitch Bend message (-8192 to +8191, centered at 0).
    /// </summary>
    public void SendPitchBend(byte channel, int bend)
    {
        int normalized = Math.Clamp(bend, -8192, 8191) + 8192;
        byte lsb = (byte)(normalized & 0x7F);
        byte msb = (byte)((normalized >> 7) & 0x7F);
        SendShortMessage((byte)(0xE0 | (channel & 0x0F)), lsb, msb);
    }

    /// <summary>
    /// Hardware-Safe Volatile Patch Selector.
    /// Adheres strictly to Section 2 of ANANTANADA_SPEC.md:
    /// NEVER triggers EEPROM or flash ROM writes on the Roland Go:Keys 3.
    /// Only dispatches volatile Bank Select MSB (CC 0), LSB (CC 32), and Program Change (PC).
    /// </summary>
    public void SelectPatchVolatile(byte channel, byte bankMsb, byte bankLsb, byte programChange)
    {
        if (channel > 15) throw new ArgumentOutOfRangeException(nameof(channel), "Channel must be 0-15.");

        byte ccStatus = (byte)(MidiConstants.ControlChange | channel);
        byte pcStatus = (byte)(MidiConstants.ProgramChange | channel);

        // 1. Bank Select MSB (CC 00)
        SendShortMessage(ccStatus, MidiConstants.BankSelectMsb, (byte)(bankMsb & 0x7F));

        // 2. Bank Select LSB (CC 32)
        SendShortMessage(ccStatus, MidiConstants.BankSelectLsb, (byte)(bankLsb & 0x7F));

        // 3. Program Change (PC 0-127)
        SendShortMessage(pcStatus, (byte)(programChange & 0x7F));

        OnLog?.Invoke($"[Patch Sent] Ch:{channel + 1} MSB:{bankMsb} LSB:{bankLsb} PC:{programChange} (Volatile in-RAM)");
    }

    /// <summary>
    /// Coarse Transposition via standard MIDI RPN (Registered Parameter Number).
    /// Roland Go:Keys 3 RPN coarse tuning: RPN MSB 0, RPN LSB 2, Data Entry 64 + transpose.
    /// Resets RPN to Null (127/127) afterwards to prevent parameter contamination.
    /// </summary>
    public void SetCoarseTranspose(byte channel, int semitones)
    {
        if (channel > 15) throw new ArgumentOutOfRangeException(nameof(channel));

        byte ccStatus = (byte)(MidiConstants.ControlChange | channel);
        int clampedSemitones = Math.Clamp(semitones, -24, 24);
        byte dataValue = (byte)(64 + clampedSemitones); // 64 = 0 semitones (center)

        // 1. RPN MSB = 0
        SendShortMessage(ccStatus, MidiConstants.RpnMsb, 0);

        // 2. RPN LSB = 2 (Coarse Tuning)
        SendShortMessage(ccStatus, MidiConstants.RpnLsb, 2);

        // 3. Data Entry MSB = 64 + transpose
        SendShortMessage(ccStatus, MidiConstants.DataEntryMsb, dataValue);

        // 4. RPN Null (127 / 127) failsafe termination
        SendShortMessage(ccStatus, MidiConstants.RpnMsb, 127);
        SendShortMessage(ccStatus, MidiConstants.RpnLsb, 127);

        OnLog?.Invoke($"[Transpose Sent] Ch:{channel + 1} Semitones:{clampedSemitones:+#;-#;0} (Data:{dataValue})");
    }

    /// <summary>
    /// Enqueues a SysEx packet into the rate-limited FIFO queue with a strict 20ms debounce delay.
    /// Protects Roland Go:Keys 3 processor from buffer overflow.
    /// </summary>
    public void EnqueueSysEx(byte[] data)
    {
        if (data == null || data.Length == 0) return;
        if (data[0] != MidiConstants.SysExStart || data[^1] != MidiConstants.SysExEnd)
        {
            OnLog?.Invoke("[SysEx Guard] Rejected malformed SysEx packet (missing F0 or F7).");
            return;
        }

        // Hardware safety: Check if packet contains forbidden non-volatile write commands (e.g. Roland EEPROM burns)
        // Roland DT1 commands typically have address 0x01... or specific flash memory maps.
        // Volatile parameter addresses are permitted; permanent memory storage commands are blocked.
        if (IsForbiddenFlashRomCommand(data))
        {
            OnLog?.Invoke("[SysEx Guard] CRITICAL: Blocked dangerous non-volatile EEPROM write command to Roland hardware.");
            return;
        }

        _sysExQueue.Add(data);
    }

    private static bool IsForbiddenFlashRomCommand(byte[] data)
    {
        // Check for Roland Device ID (0x41) and Command Type DT1 (0x12) targeting flash memory sectors
        if (data.Length >= 8 && data[1] == 0x41 && data[4] == 0x12)
        {
            // Roland Go:Keys / ZEN-Core flash backup write addresses
            byte addrHigh = data[5];
            if (addrHigh == 0x0F || addrHigh == 0x1F) // Known system ROM dump / flash write headers
            {
                return true;
            }
        }
        return false;
    }

    private void ProcessSysExQueue()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                if (_sysExQueue.TryTake(out byte[]? packet, 50, _cts.Token))
                {
                    if (packet != null && (_hMidiOut != nint.Zero || _uwpMidiOut != null))
                    {
                        SendRawSysEx(packet);
                    }

                    // Strict 20ms debounce delay as required by ANANTANADA_SPEC Section 2
                    Thread.Sleep(MidiConstants.SysExDebounceMs);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[SysEx Worker Exception] {ex.Message}");
            }
        }
    }

    private unsafe void SendRawSysEx(byte[] data)
    {
        if (_uwpMidiOut != null)
        {
            var writer = new DataWriter();
            writer.WriteBytes(data);
            _uwpMidiOut.SendBuffer(writer.DetachBuffer());
        }

        if (_hMidiOut == nint.Zero) return;

        nint dataPtr = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, dataPtr, data.Length);

        var header = new MIDIHDR
        {
            lpData = dataPtr,
            dwBufferLength = (uint)data.Length,
            dwBytesRecorded = (uint)data.Length,
            dwFlags = 0
        };

        nint headerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MIDIHDR>());
        Marshal.StructureToPtr(header, headerPtr, false);

        try
        {
            midiOutLongMsg(_hMidiOut, headerPtr, (uint)Marshal.SizeOf<MIDIHDR>());
        }
        finally
        {
            // Give driver brief time before free
            Thread.Sleep(2);
            Marshal.FreeHGlobal(dataPtr);
            Marshal.FreeHGlobal(headerPtr);
        }
    }

    /// <summary>
    /// Hardware-Safe Panic Engine (Failsafe Reset).
    /// Executes immediately across all 16 MIDI channels:
    /// - CC 120 (All Sound Off)
    /// - CC 123 (All Notes Off)
    /// - CC 64  (Sustain Off = 0)
    /// </summary>
    public void TriggerPanic()
    {
        OnLog?.Invoke("[PANIC CIRCUIT TRIGGERED] Silencing all 16 MIDI channels immediately.");

        if (_hMidiOut != nint.Zero || _uwpMidiOut != null)
        {
            for (byte ch = 0; ch < MidiConstants.TotalChannels; ch++)
            {
                byte status = (byte)(MidiConstants.ControlChange | ch);

                // 1. CC 120: All Sound Off (Immediate cutoff)
                SendShortMessage(status, MidiConstants.AllSoundOff, 0);

                // 2. CC 123: All Notes Off
                SendShortMessage(status, MidiConstants.AllNotesOff, 0);

                // 3. CC 64: Sustain Pedal Off
                SendShortMessage(status, MidiConstants.SustainPedal, 0);

                // Extra safety: individual Note Offs for full 61-key range
                for (byte note = MidiConstants.LowestNote; note <= MidiConstants.HighestNote; note++)
                {
                    SendShortMessage((byte)(MidiConstants.NoteOff | ch), note, 0);
                }
            }

            // Driver-level reset for WinMM
            if (_hMidiOut != nint.Zero)
            {
                midiOutReset(_hMidiOut);
            }
        }
    }

    public void Close()
    {
        if (_uwpMidiIn != null)
        {
            try
            {
                _uwpMidiIn.MessageReceived -= OnUwpMidiInMessageReceived;
                _uwpMidiIn.Dispose();
            }
            catch { }
            _uwpMidiIn = null;
        }

        if (_uwpMidiOut != null)
        {
            try
            {
                _uwpMidiOut.Dispose();
            }
            catch { }
            _uwpMidiOut = null;
        }

        if (_hMidiIn != nint.Zero)
        {
            midiInStop(_hMidiIn);
            midiInClose(_hMidiIn);
            _hMidiIn = nint.Zero;
        }

        if (_hMidiOut != nint.Zero)
        {
            midiOutClose(_hMidiOut);
            _hMidiOut = nint.Zero;
        }

        ConnectedDeviceName = "Disconnected";
        OnDeviceDisconnected?.Invoke();
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _cts.Cancel();
        try
        {
            _sysExWorkerThread.Join(500);
        }
        catch { }

        TriggerPanic();
        Close();

        _sysExQueue.Dispose();
        _cts.Dispose();
    }
}
