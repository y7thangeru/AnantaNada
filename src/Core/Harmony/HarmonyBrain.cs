using AnantaNada.Core.Sequencer;

namespace AnantaNada.Core.Harmony;

public enum KeyGravityMode
{
    Off,
    SnapNearest,
    MuteOffScale,
    HighlightOnly
}

public sealed class DetectedChord
{
    public string Name { get; set; } = "None";
    public string Root { get; set; } = "";
    public string Quality { get; set; } = "";
    public string Bass { get; set; } = "";
    public int[] PitchClasses { get; set; } = [];
}

/// <summary>
/// Harmony Brain & Key Gravity Engine inspired by Ohpe's HarmonyBrain.cpp and KeyGravity.cpp.
/// Provides real-time polyphonic chord recognition from incoming keyboard notes and magnetic
/// pitch clamping (Key Gravity) to prevent accidental off-key slips during live stage performances.
/// </summary>
public sealed class HarmonyBrain
{
    private static readonly string[] NoteNames = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

    private readonly HashSet<byte> _activeNotes = new();
    private byte _currentScaleRoot = 0; // C
    private MusicalScale _currentScale = MusicalScale.Major;
    private KeyGravityMode _gravityMode = KeyGravityMode.SnapNearest;

    public event Action<DetectedChord>? OnChordDetected;
    public event Action<byte, byte>? OnNoteClamped; // originalNote, clampedNote
    public event Action<string>? OnLog;

    public KeyGravityMode GravityMode
    {
        get => _gravityMode;
        set
        {
            _gravityMode = value;
            OnLog?.Invoke($"[HarmonyBrain] Key Gravity Mode set to {_gravityMode}");
        }
    }

    public MusicalScale CurrentScale => _currentScale;
    public byte CurrentScaleRoot => _currentScaleRoot;
    public DetectedChord LastDetectedChord { get; private set; } = new();

    public void SetScale(byte rootNote, MusicalScale scale)
    {
        _currentScaleRoot = (byte)(rootNote % 12);
        _currentScale = scale;
        OnLog?.Invoke($"[HarmonyBrain] Scale calibrated to {NoteNames[_currentScaleRoot]} {_currentScale}");
    }

    /// <summary>
    /// Processes an incoming Note On event. If Key Gravity is active, snaps or adjusts the note.
    /// Returns the effective note to send to the sound engine.
    /// </summary>
    public byte ProcessNoteOn(byte note, byte velocity)
    {
        lock (_activeNotes)
        {
            _activeNotes.Add(note);
        }

        byte effectiveNote = note;

        if (_gravityMode == KeyGravityMode.SnapNearest)
        {
            byte clamped = SequencerEngine.QuantizeToScale(note, _currentScaleRoot, _currentScale);
            if (clamped != note)
            {
                OnNoteClamped?.Invoke(note, clamped);
                effectiveNote = clamped;
            }
        }

        AnalyzeActiveNotes();
        return effectiveNote;
    }

    public void ProcessNoteOff(byte note)
    {
        lock (_activeNotes)
        {
            _activeNotes.Remove(note);
        }
        AnalyzeActiveNotes();
    }

    public void ClearHeldNotes()
    {
        lock (_activeNotes)
        {
            _activeNotes.Clear();
        }
        LastDetectedChord = new DetectedChord { Name = "None" };
        OnChordDetected?.Invoke(LastDetectedChord);
    }

    private void AnalyzeActiveNotes()
    {
        List<byte> notes;
        lock (_activeNotes)
        {
            notes = [.. _activeNotes];
        }

        if (notes.Count == 0)
        {
            LastDetectedChord = new DetectedChord { Name = "None" };
            OnChordDetected?.Invoke(LastDetectedChord);
            return;
        }

        notes.Sort();
        byte lowestNote = notes[0];
        string bassName = NoteNames[lowestNote % 12];

        var pitchClassSet = new SortedSet<int>();
        foreach (byte n in notes)
        {
            pitchClassSet.Add(n % 12);
        }

        var detected = IdentifyChord([.. pitchClassSet], bassName);
        LastDetectedChord = detected;
        OnChordDetected?.Invoke(detected);
    }

    public static DetectedChord IdentifyChord(int[] pitchClasses, string bassName)
    {
        if (pitchClasses.Length == 1)
        {
            string rootName = NoteNames[pitchClasses[0]];
            return new DetectedChord
            {
                Name = rootName,
                Root = rootName,
                Quality = "Single",
                Bass = bassName,
                PitchClasses = pitchClasses
            };
        }

        if (pitchClasses.Length == 2)
        {
            int interval = (pitchClasses[1] - pitchClasses[0] + 12) % 12;
            string rootName = NoteNames[pitchClasses[0]];
            string quality = interval switch
            {
                7 => "5 (Power Chord)",
                4 => "Major 3rd",
                3 => "Minor 3rd",
                _ => $"Dyad (+{interval})"
            };
            return new DetectedChord
            {
                Name = $"{rootName}{quality}",
                Root = rootName,
                Quality = quality,
                Bass = bassName,
                PitchClasses = pitchClasses
            };
        }

        // Test every pitch class as a candidate root
        foreach (int root in pitchClasses)
        {
            var relative = new HashSet<int>();
            foreach (int pc in pitchClasses)
            {
                relative.Add((pc - root + 12) % 12);
            }

            string rootName = NoteNames[root];

            // Triads
            if (relative.Contains(4) && relative.Contains(7) && relative.Count == 3)
                return FormatChord(rootName, "", "Major", bassName, pitchClasses);
            if (relative.Contains(3) && relative.Contains(7) && relative.Count == 3)
                return FormatChord(rootName, "m", "Minor", bassName, pitchClasses);
            if (relative.Contains(3) && relative.Contains(6) && relative.Count == 3)
                return FormatChord(rootName, "dim", "Diminished", bassName, pitchClasses);
            if (relative.Contains(4) && relative.Contains(8) && relative.Count == 3)
                return FormatChord(rootName, "aug", "Augmented", bassName, pitchClasses);
            if (relative.Contains(2) && relative.Contains(7) && relative.Count == 3)
                return FormatChord(rootName, "sus2", "Suspended 2", bassName, pitchClasses);
            if (relative.Contains(5) && relative.Contains(7) && relative.Count == 3)
                return FormatChord(rootName, "sus4", "Suspended 4", bassName, pitchClasses);

            // 7th Chords
            if (relative.Contains(4) && relative.Contains(7) && relative.Contains(11))
                return FormatChord(rootName, "maj7", "Major 7th", bassName, pitchClasses);
            if (relative.Contains(4) && relative.Contains(7) && relative.Contains(10))
                return FormatChord(rootName, "7", "Dominant 7th", bassName, pitchClasses);
            if (relative.Contains(3) && relative.Contains(7) && relative.Contains(10))
                return FormatChord(rootName, "m7", "Minor 7th", bassName, pitchClasses);
            if (relative.Contains(3) && relative.Contains(6) && relative.Contains(10))
                return FormatChord(rootName, "m7b5", "Half-Diminished", bassName, pitchClasses);
            if (relative.Contains(3) && relative.Contains(6) && relative.Contains(9))
                return FormatChord(rootName, "dim7", "Diminished 7th", bassName, pitchClasses);
            if (relative.Contains(4) && relative.Contains(7) && relative.Contains(2))
                return FormatChord(rootName, "add9", "Add 9", bassName, pitchClasses);
        }

        // Fallback for complex voicings
        string fallbackRoot = NoteNames[pitchClasses[0]];
        return new DetectedChord
        {
            Name = $"{fallbackRoot}poly",
            Root = fallbackRoot,
            Quality = "Cluster/Extended",
            Bass = bassName,
            PitchClasses = pitchClasses
        };
    }

    private static DetectedChord FormatChord(string rootName, string suffix, string quality, string bassName, int[] pitchClasses)
    {
        string fullName = $"{rootName}{suffix}";
        if (bassName != rootName)
        {
            fullName += $"/{bassName}";
        }

        return new DetectedChord
        {
            Name = fullName,
            Root = rootName,
            Quality = quality,
            Bass = bassName,
            PitchClasses = pitchClasses
        };
    }
}
