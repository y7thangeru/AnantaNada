namespace AnantaNada.Core.Motion;

public sealed class MotionPoint
{
    public double TimeSeconds { get; set; }
    public int Value { get; set; } // CC: 0..127 or PitchBend: -8192..8191
}

public sealed class MotionLane
{
    public string Name { get; set; } = "Lane";
    public byte Channel { get; set; } = 0;
    public int ControllerId { get; set; } // CC number (0..127) or -1 for Pitch Bend
    public double LoopDurationSeconds { get; set; } = 2.0;
    public List<MotionPoint> Points { get; } = new();
    public bool IsRecording { get; set; } = false;
    public bool IsLooping { get; set; } = true;
    public double PlayheadSeconds { get; set; } = 0;
    public int LastDispatchedValue { get; set; } = -99999;
}

/// <summary>
/// Motion Recorder & Polymetric Automation Looper inspired by Ohpe's MotionRecorder.cpp.
/// Enables live keyboardists to capture knob, wheel, and expression gestures into looping lanes,
/// freeing both hands to perform complex stage chords and solos.
/// </summary>
public sealed class MotionRecorder
{
    private readonly Dictionary<int, MotionLane> _lanes = new(); // Key = ControllerId (-1 for Pitch Bend, 1 for Mod, 74 for Cutoff, etc.)
    private bool _isGlobalRecording = false;
    private bool _isGlobalPlaying = false;
    private double _recordElapsed = 0;

    public event Action<byte, byte, byte>? OnControlChange; // channel, cc, value
    public event Action<byte, int>? OnPitchBend;            // channel, bend
    public event Action<string>? OnLog;

    public bool IsGlobalRecording => _isGlobalRecording;
    public bool IsGlobalPlaying => _isGlobalPlaying;
    public IReadOnlyDictionary<int, MotionLane> Lanes => _lanes;

    public MotionRecorder()
    {
        // Pre-configure standard expressive lanes:
        // Controller -1: Pitch Bend
        AddLane(controllerId: -1, name: "Pitch Bend", loopDuration: 2.0, channel: 0);
        // Controller 1: Modulation Wheel
        AddLane(controllerId: 1, name: "Mod Wheel", loopDuration: 2.0, channel: 0);
        // Controller 74: Brightness / Cutoff
        AddLane(controllerId: 74, name: "Filter Cutoff", loopDuration: 4.0, channel: 0);
        // Controller 11: Expression
        AddLane(controllerId: 11, name: "Expression", loopDuration: 4.0, channel: 0);
    }

    public void AddLane(int controllerId, string name, double loopDuration, byte channel = 0)
    {
        var lane = new MotionLane
        {
            ControllerId = controllerId,
            Name = name,
            LoopDurationSeconds = Math.Max(0.1, loopDuration),
            Channel = channel
        };
        _lanes[controllerId] = lane;
    }

    public void StartRecording(double targetDuration = 2.0)
    {
        _isGlobalRecording = true;
        _isGlobalPlaying = false;
        _recordElapsed = 0;

        foreach (var lane in _lanes.Values)
        {
            lane.Points.Clear();
            lane.LoopDurationSeconds = targetDuration;
            lane.IsRecording = true;
            lane.PlayheadSeconds = 0;
            lane.LastDispatchedValue = -99999;
        }

        OnLog?.Invoke($"[MotionRecorder] Armed and Recording for {targetDuration:F1}s...");
    }

    public void StopRecording()
    {
        _isGlobalRecording = false;
        foreach (var lane in _lanes.Values)
        {
            lane.IsRecording = false;
            lane.PlayheadSeconds = 0;
        }

        _isGlobalPlaying = true;
        OnLog?.Invoke("[MotionRecorder] Recording Complete. Playback looping engaged.");
    }

    public void TogglePlayback(bool play)
    {
        _isGlobalPlaying = play;
        OnLog?.Invoke($"[MotionRecorder] Playback {(play ? "STARTED" : "STOPPED")}");
    }

    /// <summary>
    /// Feeds incoming hardware CC or Pitch Bend data from NativeMidiEngine into the motion recorder.
    /// </summary>
    public void RecordInput(int controllerId, int value, byte channel = 0)
    {
        if (!_isGlobalRecording) return;

        if (!_lanes.TryGetValue(controllerId, out var lane))
        {
            lane = new MotionLane
            {
                ControllerId = controllerId,
                Name = controllerId == -1 ? "Pitch Bend" : $"CC {controllerId}",
                Channel = channel,
                LoopDurationSeconds = 2.0
            };
            _lanes[controllerId] = lane;
        }

        lane.Points.Add(new MotionPoint
        {
            TimeSeconds = _recordElapsed,
            Value = value
        });
    }

    /// <summary>
    /// Frame tick to advance recording elapsed time or play back looping automation curves.
    /// </summary>
    public void Tick(double deltaSeconds)
    {
        if (_isGlobalRecording)
        {
            _recordElapsed += deltaSeconds;
            // Check if any lane reached its designated recording duration
            double maxDuration = _lanes.Values.Max(l => l.LoopDurationSeconds);
            if (_recordElapsed >= maxDuration)
            {
                StopRecording();
            }
            return;
        }

        if (!_isGlobalPlaying) return;

        foreach (var lane in _lanes.Values)
        {
            if (lane.Points.Count < 2 || !lane.IsLooping) continue;

            lane.PlayheadSeconds = (lane.PlayheadSeconds + deltaSeconds) % lane.LoopDurationSeconds;
            int interpolated = SampleLane(lane, lane.PlayheadSeconds);

            if (interpolated != lane.LastDispatchedValue)
            {
                lane.LastDispatchedValue = interpolated;
                if (lane.ControllerId == -1)
                {
                    OnPitchBend?.Invoke(lane.Channel, interpolated);
                }
                else
                {
                    OnControlChange?.Invoke(lane.Channel, (byte)lane.ControllerId, (byte)Math.Clamp(interpolated, 0, 127));
                }
            }
        }
    }

    private static int SampleLane(MotionLane lane, double time)
    {
        var points = lane.Points;
        if (points.Count == 0) return 0;
        if (points.Count == 1 || time <= points[0].TimeSeconds) return points[0].Value;
        if (time >= points[^1].TimeSeconds) return points[^1].Value;

        // Binary search for surrounding points
        int low = 0;
        int high = points.Count - 1;

        while (low <= high)
        {
            int mid = (low + high) / 2;
            if (points[mid].TimeSeconds <= time)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        int idx1 = Math.Max(0, high);
        int idx2 = Math.Min(points.Count - 1, low);

        var p1 = points[idx1];
        var p2 = points[idx2];

        if (Math.Abs(p2.TimeSeconds - p1.TimeSeconds) < 0.0001)
        {
            return p1.Value;
        }

        double t = (time - p1.TimeSeconds) / (p2.TimeSeconds - p1.TimeSeconds);
        return (int)Math.Round(p1.Value + (p2.Value - p1.Value) * t);
    }

    public void ClearAll()
    {
        _isGlobalRecording = false;
        _isGlobalPlaying = false;
        _recordElapsed = 0;
        foreach (var lane in _lanes.Values)
        {
            lane.Points.Clear();
            lane.PlayheadSeconds = 0;
            lane.LastDispatchedValue = -99999;
        }
        OnLog?.Invoke("[MotionRecorder] All automation lanes cleared.");
    }
}
