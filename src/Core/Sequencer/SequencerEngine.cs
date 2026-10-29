namespace AnantaNada.Core.Sequencer;

public sealed class OrbitRing
{
    public string Name { get; set; } = "Ring";
    public int Steps { get; set; } = 8;
    public int Pulses { get; set; } = 3;
    public int Rotation { get; set; } = 0;
    public byte MidiNote { get; set; } = 36; // C1 (Kick) or melodic root
    public byte Velocity { get; set; } = 100;
    public byte Channel { get; set; } = 9;  // MIDI Ch 10 (index 9) for drums or Ch 1 (index 0)
    public bool[] Pattern { get; set; } = [];
    public int CurrentStep { get; set; } = 0;
    public bool IsMuted { get; set; } = false;
}

public enum MusicalScale
{
    Chromatic,
    Major,
    NaturalMinor,
    MajorPentatonic,
    MinorPentatonic,
    Dorian,
    Mixolydian,
    Blues,
    Hirajoshi
}

/// <summary>
/// Euclidean and Orbit Polymetric Sequencer inspired by Ohpe's EuclidSequencer and OrbitSequencer.
/// Generates Euclidean rhythms using Bjorklund's algorithm and drives multi-ring polymetric sequences
/// directly to Roland Go:Keys 3 drum and synth channels.
/// </summary>
public sealed class SequencerEngine
{
    private readonly List<OrbitRing> _rings = new();
    private bool _isPlaying = false;
    private int _globalStep = 0;

    public event Action<byte, byte, byte>? OnNoteTriggered; // channel, note, velocity
    public event Action<string, int, int>? OnStepAdvanced;  // ringName, currentStep, totalSteps
    public event Action<string>? OnLog;

    public bool IsPlaying => _isPlaying;
    public IReadOnlyList<OrbitRing> Rings => _rings;

    public SequencerEngine()
    {
        // Default standard stage setup: 4-ring polymetric suite
        // Ring 1: Kick drum (4/4 pulse)
        AddOrbitRing("Kick", steps: 16, pulses: 4, rotation: 0, midiNote: 36, velocity: 110, channel: 9);
        // Ring 2: Snare (Euclid 2 across 16)
        AddOrbitRing("Snare", steps: 16, pulses: 2, rotation: 4, midiNote: 38, velocity: 100, channel: 9);
        // Ring 3: Hi-Hat (Euclid 7 across 16 - funky syncopation)
        AddOrbitRing("HiHat", steps: 16, pulses: 7, rotation: 0, midiNote: 42, velocity: 85, channel: 9);
        // Ring 4: Orbit Polymetric Melodic Arp (5 steps loop over synth channel)
        AddOrbitRing("OrbitArp", steps: 5, pulses: 3, rotation: 0, midiNote: 60, velocity: 95, channel: 0);
    }

    /// <summary>
    /// Bjorklund algorithm for distributing k pulses evenly into n steps.
    /// Used for Euclidean rhythms (e.g. E(3,8) Tresillo, E(5,8) Cinquillo, E(7,16) Samba).
    /// </summary>
    public static bool[] GenerateEuclidean(int steps, int pulses, int rotation = 0)
    {
        if (steps <= 0) return [false];
        if (pulses <= 0) return new bool[steps];
        if (pulses >= steps)
        {
            var all = new bool[steps];
            Array.Fill(all, true);
            return all;
        }

        var groups = new List<List<bool>>();
        for (int i = 0; i < pulses; i++)
        {
            groups.Add([true]);
        }
        for (int i = 0; i < steps - pulses; i++)
        {
            groups.Add([false]);
        }

        while (true)
        {
            int numPulses = 0;
            int numZeros = 0;
            while (numPulses < groups.Count && groups[numPulses][0]) numPulses++;
            numZeros = groups.Count - numPulses;

            if (numZeros <= 1) break;

            int minCount = Math.Min(numPulses, numZeros);
            for (int i = 0; i < minCount; i++)
            {
                groups[i].AddRange(groups[groups.Count - 1 - i]);
            }
            groups.RemoveRange(groups.Count - minCount, minCount);
        }

        var result = new List<bool>();
        foreach (var group in groups)
        {
            result.AddRange(group);
        }

        // Apply rotation / phase offset
        bool[] pattern = new bool[steps];
        for (int i = 0; i < steps; i++)
        {
            int sourceIndex = (i - rotation) % steps;
            if (sourceIndex < 0) sourceIndex += steps;
            pattern[i] = result[sourceIndex];
        }

        return pattern;
    }

    public void AddOrbitRing(string name, int steps, int pulses, int rotation, byte midiNote, byte velocity, byte channel)
    {
        var ring = new OrbitRing
        {
            Name = name,
            Steps = Math.Max(1, steps),
            Pulses = Math.Clamp(pulses, 0, steps),
            Rotation = rotation,
            MidiNote = midiNote,
            Velocity = velocity,
            Channel = channel,
            Pattern = GenerateEuclidean(steps, pulses, rotation),
            CurrentStep = 0
        };

        _rings.Add(ring);
        OnLog?.Invoke($"[Sequencer] Added Orbit Ring '{name}' E({pulses},{steps}) on Ch {channel + 1}");
    }

    public void UpdateRing(int index, int steps, int pulses, int rotation)
    {
        if (index < 0 || index >= _rings.Count) return;
        var ring = _rings[index];
        ring.Steps = Math.Max(1, steps);
        ring.Pulses = Math.Clamp(pulses, 0, ring.Steps);
        ring.Rotation = rotation;
        ring.Pattern = GenerateEuclidean(ring.Steps, ring.Pulses, rotation);
        ring.CurrentStep = 0;
        OnLog?.Invoke($"[Sequencer] Updated Ring #{index} '{ring.Name}' -> E({ring.Pulses},{ring.Steps})");
    }

    public void Start()
    {
        _isPlaying = true;
        _globalStep = 0;
        foreach (var ring in _rings)
        {
            ring.CurrentStep = 0;
        }
        OnLog?.Invoke("[Sequencer] Playback Started.");
    }

    public void Stop()
    {
        _isPlaying = false;
        OnLog?.Invoke("[Sequencer] Playback Stopped.");
    }

    /// <summary>
    /// Advances the sequencer by 1 16th-note step. Synchronized with the master transport clock.
    /// </summary>
    public void StepTick()
    {
        if (!_isPlaying) return;

        foreach (var ring in _rings)
        {
            if (ring.Pattern.Length == 0) continue;

            int currentStep = ring.CurrentStep % ring.Pattern.Length;
            bool hit = ring.Pattern[currentStep];

            if (hit && !ring.IsMuted)
            {
                OnNoteTriggered?.Invoke(ring.Channel, ring.MidiNote, ring.Velocity);
            }

            OnStepAdvanced?.Invoke(ring.Name, currentStep, ring.Steps);
            ring.CurrentStep = (ring.CurrentStep + 1) % ring.Steps;
        }

        _globalStep++;
    }

    /// <summary>
    /// Quantizes a chromatic semitone value to the selected musical scale.
    /// </summary>
    public static byte QuantizeToScale(byte note, byte rootNote, MusicalScale scale)
    {
        int[] intervals = scale switch
        {
            MusicalScale.Major => [0, 2, 4, 5, 7, 9, 11],
            MusicalScale.NaturalMinor => [0, 2, 3, 5, 7, 8, 10],
            MusicalScale.MajorPentatonic => [0, 2, 4, 7, 9],
            MusicalScale.MinorPentatonic => [0, 3, 5, 7, 10],
            MusicalScale.Dorian => [0, 2, 3, 5, 7, 9, 10],
            MusicalScale.Mixolydian => [0, 2, 4, 5, 7, 9, 10],
            MusicalScale.Blues => [0, 3, 5, 6, 7, 10],
            MusicalScale.Hirajoshi => [0, 2, 3, 7, 8],
            _ => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
        };

        int relative = (note - rootNote) % 12;
        if (relative < 0) relative += 12;

        int closestInterval = intervals[0];
        int minDiff = 100;
        foreach (int interval in intervals)
        {
            int diff = Math.Abs(relative - interval);
            if (diff < minDiff)
            {
                minDiff = diff;
                closestInterval = interval;
            }
        }

        int octaveOffset = (note - rootNote) / 12;
        if (note < rootNote && relative != 0) octaveOffset--;

        int quantized = rootNote + (octaveOffset * 12) + closestInterval;
        return (byte)Math.Clamp(quantized, 0, 127);
    }
}
