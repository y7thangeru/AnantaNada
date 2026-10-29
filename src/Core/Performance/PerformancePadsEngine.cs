namespace AnantaNada.Core.Performance;

public enum PerformancePadType
{
    Stutter,
    TapeStop,
    PitchRiser,
    FilterSweep,
    ReverbFreeze
}

/// <summary>
/// Performance Pads Engine inspired by Ohpe's PerformancePads.cpp.
/// Provides real-time stage gestures: quantized stutter, vinyl tape-stop pitch fall,
/// filter cutoff riser sweeps, and ambient reverb-freeze throws.
/// </summary>
public sealed class PerformancePadsEngine
{
    private bool _isStutterActive;
    private bool _isTapeStopActive;
    private bool _isPitchRiserActive;
    private bool _isFilterSweepActive;
    private bool _isReverbFreezeActive;

    private double _tapeStopElapsed;
    private const double TapeStopDuration = 0.65; // 650ms vinyl brake drop

    private double _riserElapsed;
    private const double RiserDuration = 2.0; // 2 seconds sweep

    private double _stutterTimer;
    private double _stutterInterval = 0.125; // 1/16 note at 120 BPM = 125ms

    private readonly HashSet<byte> _heldNotes = new();
    private byte _activeChannel = 0; // Default Ch 1 (index 0)

    public event Action<byte, int>? OnPitchBend;
    public event Action<byte, byte, byte>? OnControlChange;
    public event Action<byte, byte, byte>? OnNoteTriggered;
    public event Action<string>? OnLog;

    public bool IsStutterActive => _isStutterActive;
    public bool IsTapeStopActive => _isTapeStopActive;
    public bool IsPitchRiserActive => _isPitchRiserActive;
    public bool IsFilterSweepActive => _isFilterSweepActive;
    public bool IsReverbFreezeActive => _isReverbFreezeActive;

    public void NoteOn(byte channel, byte note, byte velocity)
    {
        _activeChannel = channel;
        lock (_heldNotes)
        {
            _heldNotes.Add(note);
        }
    }

    public void NoteOff(byte channel, byte note)
    {
        lock (_heldNotes)
        {
            _heldNotes.Remove(note);
        }
    }

    public void SetStutterDivision(int bpm, int subdivision = 16)
    {
        if (bpm <= 0) bpm = 120;
        // 60 / BPM = quarter note duration in seconds
        double quarterSec = 60.0 / bpm;
        _stutterInterval = subdivision switch
        {
            4 => quarterSec,
            8 => quarterSec / 2.0,
            16 => quarterSec / 4.0,
            32 => quarterSec / 8.0,
            _ => quarterSec / 4.0
        };
    }

    public void TriggerPad(PerformancePadType type, bool active, byte channel = 0)
    {
        _activeChannel = channel;
        switch (type)
        {
            case PerformancePadType.Stutter:
                _isStutterActive = active;
                _stutterTimer = 0;
                OnLog?.Invoke($"[Pads] Stutter {(active ? "ENGAGED" : "RELEASED")}");
                break;

            case PerformancePadType.TapeStop:
                _isTapeStopActive = active;
                _tapeStopElapsed = 0;
                if (!active)
                {
                    // Restore pitch to center (0)
                    OnPitchBend?.Invoke(_activeChannel, 0);
                }
                OnLog?.Invoke($"[Pads] Tape Stop {(active ? "ENGAGED" : "RELEASED")}");
                break;

            case PerformancePadType.PitchRiser:
                _isPitchRiserActive = active;
                _riserElapsed = 0;
                if (!active)
                {
                    OnPitchBend?.Invoke(_activeChannel, 0);
                    // Reset CC 74 (Cutoff/Brightness) to normal center 64
                    OnControlChange?.Invoke(_activeChannel, 74, 64);
                }
                OnLog?.Invoke($"[Pads] Pitch Riser {(active ? "ENGAGED" : "RELEASED")}");
                break;

            case PerformancePadType.FilterSweep:
                _isFilterSweepActive = active;
                _riserElapsed = 0;
                if (!active)
                {
                    OnControlChange?.Invoke(_activeChannel, 74, 64); // Normal Cutoff
                    OnControlChange?.Invoke(_activeChannel, 71, 64); // Normal Resonance
                }
                OnLog?.Invoke($"[Pads] Filter Sweep {(active ? "ENGAGED" : "RELEASED")}");
                break;

            case PerformancePadType.ReverbFreeze:
                _isReverbFreezeActive = active;
                if (active)
                {
                    // Send CC 91 (Reverb send) = 127, CC 64 (Sustain) = 127
                    OnControlChange?.Invoke(_activeChannel, 91, 127);
                    OnControlChange?.Invoke(_activeChannel, 64, 127);
                }
                else
                {
                    // Release freeze
                    OnControlChange?.Invoke(_activeChannel, 91, 40); // Standard moderate reverb
                    OnControlChange?.Invoke(_activeChannel, 64, 0);  // Release sustain
                }
                OnLog?.Invoke($"[Pads] Reverb Freeze {(active ? "ENGAGED" : "RELEASED")}");
                break;
        }
    }

    /// <summary>
    /// Frame or timer tick to animate continuous time-varying gestures.
    /// </summary>
    public void Tick(double deltaSeconds)
    {
        if (_isTapeStopActive)
        {
            _tapeStopElapsed += deltaSeconds;
            double progress = Math.Clamp(_tapeStopElapsed / TapeStopDuration, 0.0, 1.0);
            // Exponential pitch decay from 0 down to -8192
            double factor = Math.Pow(progress, 1.5);
            int currentBend = (int)(-8192 * factor);
            OnPitchBend?.Invoke(_activeChannel, currentBend);

            if (progress >= 1.0)
            {
                // Silence held notes once brake reaches bottom
                lock (_heldNotes)
                {
                    foreach (byte note in _heldNotes)
                    {
                        OnNoteTriggered?.Invoke(_activeChannel, note, 0);
                    }
                }
            }
        }

        if (_isPitchRiserActive)
        {
            _riserElapsed += deltaSeconds;
            double progress = Math.Clamp(_riserElapsed / RiserDuration, 0.0, 1.0);
            // Linear upward pitch bend from 0 to +8191
            int bend = (int)(8191 * progress);
            OnPitchBend?.Invoke(_activeChannel, bend);

            // Cutoff rising from 64 to 127
            byte cutoff = (byte)Math.Clamp(64 + (int)(63 * progress), 0, 127);
            OnControlChange?.Invoke(_activeChannel, 74, cutoff);
        }

        if (_isFilterSweepActive)
        {
            _riserElapsed += deltaSeconds;
            double progress = Math.Clamp(_riserElapsed / RiserDuration, 0.0, 1.0);
            // Sweep resonance and cutoff
            byte cutoff = (byte)Math.Clamp((int)(127 * (1.0 - Math.Abs(progress * 2.0 - 1.0))), 20, 127);
            byte resonance = (byte)Math.Clamp(40 + (int)(70 * progress), 40, 110);
            OnControlChange?.Invoke(_activeChannel, 74, cutoff);
            OnControlChange?.Invoke(_activeChannel, 71, resonance);
        }

        if (_isStutterActive)
        {
            _stutterTimer += deltaSeconds;
            if (_stutterTimer >= _stutterInterval)
            {
                _stutterTimer -= _stutterInterval;
                lock (_heldNotes)
                {
                    foreach (byte note in _heldNotes)
                    {
                        // Retrigger held notes with velocity burst
                        OnNoteTriggered?.Invoke(_activeChannel, note, 110);
                    }
                }
            }
        }
    }

    public void ResetAll()
    {
        _isStutterActive = false;
        _isTapeStopActive = false;
        _isPitchRiserActive = false;
        _isFilterSweepActive = false;
        _isReverbFreezeActive = false;

        OnPitchBend?.Invoke(_activeChannel, 0);
        OnControlChange?.Invoke(_activeChannel, 74, 64);
        OnControlChange?.Invoke(_activeChannel, 71, 64);
        OnControlChange?.Invoke(_activeChannel, 91, 40);
        OnControlChange?.Invoke(_activeChannel, 64, 0);

        lock (_heldNotes)
        {
            _heldNotes.Clear();
        }
    }
}
