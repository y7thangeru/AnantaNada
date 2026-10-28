using Godot;
using AnantaNada.Core.Midi;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Lead Game Console Controller for Roland Go:Keys 3 (Cockpit HUD).
/// Orchestrates the live MIDI pipeline, dynamic 64x1 keybed texture,
/// 5-minute retroactive capture buffer, and StageHub WebSocket telemetry.
/// </summary>
public partial class MainStageController : Control
{
    [Export] public int HttpPort { get; set; } = 8080;
    [Export] public float ScreenShakeIntensity { get; set; } = 8.0f;

    // Subsystems
    public NativeMidiEngine MidiEngine { get; private set; } = null!;
    public RetroactiveBuffer CaptureBuffer { get; private set; } = null!;
    public StageHubServer HubServer { get; private set; } = null!;

    // UI Nodes
    private Label _lblDeviceStatus = null!;
    private Label _lblIpAddress = null!;
    private Label _lblBpmBeat = null!;
    private Label _lblActiveMission = null!;
    private Label _lblCurrentChord = null!;
    private Label _lblNextChord = null!;
    private Label _lblLyricTeleprompter = null!;
    private Label _lblLyricBar = null!;
    private Label _lblRhythmAccuracy = null!;
    private Label _lblTimingVerdict = null!;
    private Label _lblStrikeGauge = null!;
    private Label _lblVelocityRank = null!;
    private Button _btnPanic = null!;
    private Button _btnNeverLostMelody = null!;
    private ColorRect _keybedRect = null!;
    private ColorRect _waterfallRect = null!;
    private PanelContainer _nextChordBox = null!;
    private PanelContainer _currentChordBox = null!;
    private PanelContainer _nextCard1 = null!;
    private PanelContainer _nextCard2 = null!;
    private PanelContainer _nextCard3 = null!;
    private Control _hudContainer = null!;

    // Shaders
    private ShaderMaterial? _keybedMaterial;
    private ShaderMaterial? _waterfallMaterial;
    private ShaderMaterial? _nextChordMaterial;
    private ShaderMaterial? _currentChordMaterial;

    // Keybed 64x1 State Texture (R: Velocity, G: Split Zone, B: Impact Flash)
    private readonly byte[] _keyStatePixels = new byte[64 * 4];
    private readonly float[] _keyVelocities = new float[64];
    private readonly float[] _keyImpacts = new float[64];
    private Image _keyStateImage = null!;
    private ImageTexture _keyStateTexture = null!;
    private readonly object _stateLock = new();

    // Gameplay & Timing State
    private int _currentSongIndex = 0;
    private int _currentBar = 1;
    private int _currentBeat = 1;
    private int _currentBpm = 128;
    private double _beatTimer = 0.0;
    private double _secondsPerBeat = 60.0 / 128.0;
    private double _lastBeatTimestamp = 0.0;

    // Rhythm Coaching
    private int _totalHits = 0;
    private int _perfectHits = 0;
    private float _rollingAccuracy = 98.4f;

    // Visual FX (Hit Beam, Beat Pulse, Glitch Flash, Screen Shake, Transition Pulse)
    private float _beatPulse = 0.0f;
    private float _hitBeamFlash = 0.0f;
    private float _screenShake = 0.0f;
    private float _glitchIntensity = 0.0f;
    private float _transitionPulse = 0.0f;
    private Vector2 _originalHudPos = Vector2.Zero;

    // Retroactive Feedback Timer
    private double _retroFeedbackTimer = 0.0;
    private bool _isShowingRetroFeedback = false;

    // Song Missions with Lyrics
    public record SongMission(
        string Title,
        string CurrentChord,
        string NextChord,
        string PatchName,
        byte BankMsb,
        byte BankLsb,
        byte Pc,
        int Bpm,
        string[] Lyrics
    );

    private readonly List<SongMission> _missions = new()
    {
        new SongMission("01. SENJA DUST", "G#m7", "C#m7", "Vintage EP 1", 87, 64, 14, 128,
            new[] { "Kala fajar tiba,", "melangkah di tanah tua", "gemuruh nada membelah malam", "kita berdiri tak tergoyahkan" }),
        new SongMission("02. KELANA BEAT", "F#maj7", "B7", "Fat Saw Lead", 87, 65, 4, 134,
            new[] { "Denyut neon memanggil,", "menembus batas waktu", "alirkan melodi tanpa henti", "di ujung cakrawala siber" }),
        new SongMission("03. FAJAR REVERIE", "Emaj9", "A#dim7", "Warm Stage Grand", 87, 64, 1, 92,
            new[] { "Hening merayap pelan,", "menyapa tuts gading", "setiap resonansi berbisik", "tentang mimpi yang tersisa" }),
        new SongMission("04. OVERDRIVE ANTHEM", "D#m7", "G#7", "Brass Punch Sect", 87, 66, 22, 140,
            new[] { "Semua mata tertuju,", "bersiap menyulut bara", "hantam ketukan puncak", "kemenangan di genggaman!" })
    };

    public override void _Ready()
    {
        // 1. Initialize Subsystems
        CaptureBuffer = new RetroactiveBuffer();
        MidiEngine = new NativeMidiEngine();
        HubServer = new StageHubServer(HttpPort);

        // 2. Resolve UI Nodes
        _hudContainer = GetNodeOrNull<Control>("HUD") ?? this;
        _originalHudPos = _hudContainer.Position;

        _lblDeviceStatus = GetNodeOrNull<Label>("HUD/TopBar/DevicePill/StatusLabel") ?? new Label();
        _lblIpAddress = GetNodeOrNull<Label>("HUD/TopBar/NetworkPill/IpLabel") ?? new Label();
        _lblBpmBeat = GetNodeOrNull<Label>("HUD/TopBar/BpmPill/BpmLabel") ?? new Label();
        _lblActiveMission = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/ActiveCard/MissionName") ?? new Label();

        _currentChordBox = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/ChordHud/CurrentChordBox") ?? new PanelContainer();
        _lblCurrentChord = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/CurrentChordBox/VBox/ChordLabel") ?? new Label();

        _nextChordBox = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/ChordHud/NextChordBox") ?? new PanelContainer();
        _lblNextChord = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/NextChordBox/NextLabel") ?? new Label();

        _lblLyricTeleprompter = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/LyricBox/VBox/LyricText") ?? new Label();
        _lblLyricBar = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/LyricBox/VBox/LyricBarIndicator") ?? new Label();

        _lblRhythmAccuracy = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/AccuracyValue") ?? new Label();
        _lblTimingVerdict = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/TimingVerdict") ?? new Label();
        _lblStrikeGauge = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/StrikeGauge") ?? new Label();
        _lblVelocityRank = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/RankBadge") ?? new Label();

        _btnPanic = GetNodeOrNull<Button>("HUD/TopBar/PanicButtonContainer/PanicButton") ?? new Button();
        _btnNeverLostMelody = GetNodeOrNull<Button>("HUD/CenterSplit/CenterCascade/CoachDeck/RetroBufferBtn") ?? new Button();

        _keybedRect = GetNodeOrNull<ColorRect>("HUD/BottomKeybed/KeybedContainer/KeybedRect") ?? new ColorRect();
        _waterfallRect = GetNodeOrNull<ColorRect>("HUD/CenterSplit/CenterCascade/WaterfallRect") ?? new ColorRect();

        _nextCard1 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard1") ?? new PanelContainer();
        _nextCard2 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard2") ?? new PanelContainer();
        _nextCard3 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard3") ?? new PanelContainer();

        // 3. Shaders
        if (_keybedRect.Material is ShaderMaterial km) _keybedMaterial = km;
        if (_waterfallRect.Material is ShaderMaterial wm) _waterfallMaterial = wm;
        if (_nextChordBox.Material is ShaderMaterial ncm) _nextChordMaterial = ncm;
        if (_currentChordBox.Material is ShaderMaterial ccm) _currentChordMaterial = ccm;

        // 4. Setup Keybed State Texture (64x1 RGBA8)
        _keyStateImage = Image.CreateEmpty(64, 1, false, Image.Format.Rgba8);
        _keyStateTexture = ImageTexture.CreateFromImage(_keyStateImage);
        _keybedMaterial?.SetShaderParameter("key_states", _keyStateTexture);

        // 5. Wire MIDI Engine Pipelines
        MidiEngine.OnMessageReceived += (in MidiRawMessage msg) => CaptureBuffer.Push(in msg);
        MidiEngine.OnNoteOn += HandleMidiNoteOn;
        MidiEngine.OnNoteOff += HandleMidiNoteOff;
        MidiEngine.OnLog += msg => GD.Print(msg);
        MidiEngine.OnDeviceConnected += dev => CallDeferred(nameof(UpdateDeviceLabel), dev, true);
        MidiEngine.OnDeviceDisconnected += () => CallDeferred(nameof(UpdateDeviceLabel), "DISCONNECTED", false);

        // 6. Wire StageHub Server Pipelines
        HubServer.OnLog += msg => GD.Print(msg);
        HubServer.OnPanicTriggered += () => CallDeferred(nameof(TriggerPanicPurge));
        HubServer.OnSetlistItemTriggered += payload => CallDeferred(nameof(SelectMission), payload.SongIndex);

        // 7. Wire UI Button Actions
        _btnPanic.Pressed += TriggerPanicPurge;
        _btnNeverLostMelody.Pressed += OnNeverLostMelodyPressed;

        // Desktop Setlist Card Click handlers
        _nextCard1.GuiInput += @event => HandleCardClick(@event, 1);
        _nextCard2.GuiInput += @event => HandleCardClick(@event, 2);
        _nextCard3.GuiInput += @event => HandleCardClick(@event, 3);

        // 8. Connect Hardware & Start Server
        MidiEngine.Connect("Go:Keys");
        HubServer.Start();

        var ips = StageHubServer.GetLocalIpAddresses();
        string displayIp = ips.Count > 0 ? $"{ips[0]}:{HttpPort}" : $"127.0.0.1:{HttpPort}";
        _lblIpAddress.Text = $"> LINK: http://{displayIp}";

        // Select initial mission
        SelectMission(0);
    }

    private void HandleCardClick(InputEvent @event, int missionIndex)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
            SelectMission(missionIndex);
        }
    }

    private void UpdateDeviceLabel(string name, bool connected)
    {
        _lblDeviceStatus.Text = connected ? $"> LINK: ROLAND [{name.ToUpper()}]" : "> LINK: NO HARDWARE (STANDALONE)";
        _lblDeviceStatus.Modulate = connected ? new Color(0.0f, 0.96f, 0.83f) : new Color(0.93f, 0.14f, 0.23f);
    }

    /// <summary>
    /// Hot-path Note On event handler called directly from MIDI thread.
    /// Updates zero-allocation key buffer and evaluates timing metrics.
    /// </summary>
    private void HandleMidiNoteOn(byte channel, byte note, byte velocity)
    {
        int keyIndex = note - MidiConstants.LowestNote; // C2 = 36 to C7 = 96
        if (keyIndex >= 0 && keyIndex < 61)
        {
            lock (_stateLock)
            {
                _keyVelocities[keyIndex] = velocity / 127.0f;
                _keyImpacts[keyIndex] = 1.0f;
            }

            CallDeferred(nameof(EvaluateTimingAndVelocity), velocity);
        }
    }

    /// <summary>
    /// Hot-path Note Off event handler.
    /// </summary>
    private void HandleMidiNoteOff(byte channel, byte note, byte velocity)
    {
        int keyIndex = note - MidiConstants.LowestNote;
        if (keyIndex >= 0 && keyIndex < 61)
        {
            lock (_stateLock)
            {
                _keyVelocities[keyIndex] = 0.0f;
            }
        }
    }

    private void EvaluateTimingAndVelocity(byte rawVelocity)
    {
        // 1. Dynamic Strike Gauge & Rank Display
        string rank;
        Color rankColor;
        string dynamicDashes;

        if (rawVelocity >= 110)
        {
            rank = "RANK [S]";
            rankColor = new Color(1.0f, 0.2f, 0.4f);
            dynamicDashes = "||||||||||||||||||||";
            _screenShake = ScreenShakeIntensity;
            _hitBeamFlash = 1.8f;
        }
        else if (rawVelocity >= 90)
        {
            rank = "RANK [A]";
            rankColor = new Color(0.0f, 0.96f, 0.83f);
            dynamicDashes = "|||||||||||||||.....";
            _hitBeamFlash = 1.2f;
        }
        else if (rawVelocity >= 70)
        {
            rank = "RANK [B]";
            rankColor = new Color(1.0f, 0.82f, 0.05f);
            dynamicDashes = "||||||||||..........";
            _hitBeamFlash = 0.8f;
        }
        else
        {
            rank = "RANK [C]";
            rankColor = new Color(0.6f, 0.7f, 0.8f);
            dynamicDashes = "|||||...............";
            _hitBeamFlash = 0.4f;
        }

        _lblVelocityRank.Text = rank;
        _lblVelocityRank.Modulate = rankColor;
        _lblStrikeGauge.Text = $"STRIKE: [ {dynamicDashes} ] {rawVelocity} ({(rawVelocity >= 110 ? "fff" : rawVelocity >= 90 ? "ff" : "p")})";

        // 2. Rhythm Timing Coach (Evaluation against metronome ticks)
        double currentSec = Time.GetTicksMsec() / 1000.0;
        double timeSinceLastBeat = currentSec - _lastBeatTimestamp;
        double deviationSec = timeSinceLastBeat % _secondsPerBeat;
        if (deviationSec > _secondsPerBeat * 0.5) deviationSec -= _secondsPerBeat;

        double deviationMs = deviationSec * 1000.0;
        string verdict;
        Color verdictColor;

        _totalHits++;

        if (Math.Abs(deviationMs) <= 32.0)
        {
            verdict = "[ PERFECT! ]";
            verdictColor = new Color(0.0f, 0.96f, 0.83f);
            _perfectHits++;
        }
        else if (Math.Abs(deviationMs) <= 65.0)
        {
            verdict = "[ GREAT! ]";
            verdictColor = new Color(0.3f, 0.9f, 0.4f);
            _perfectHits++;
        }
        else if (deviationMs < 0)
        {
            verdict = "[ RUSHING! ]";
            verdictColor = new Color(1.0f, 0.72f, 0.01f);
        }
        else
        {
            verdict = "[ DRAGGING! ]";
            verdictColor = new Color(0.93f, 0.14f, 0.23f);
        }

        _rollingAccuracy = Mathf.Clamp((float)_perfectHits / _totalHits * 100.0f, 60.0f, 100.0f);

        if (!_isShowingRetroFeedback)
        {
            _lblTimingVerdict.Text = $"ACCURACY: {verdict}";
            _lblTimingVerdict.Modulate = verdictColor;
        }

        _lblRhythmAccuracy.Text = $"SCORE ACC: {_rollingAccuracy:F1}%";
    }

    public override void _Process(double delta)
    {
        // 1. Stream key state buffer into 64x1 texture for piano_keybed.gdshader
        bool textureNeedsUpdate = false;
        lock (_stateLock)
        {
            for (int i = 0; i < 61; i++)
            {
                if (_keyImpacts[i] > 0.0f)
                {
                    _keyImpacts[i] = Mathf.Max(0.0f, _keyImpacts[i] - (float)delta * 14.0f);
                    textureNeedsUpdate = true;
                }

                int pixelOffset = i * 4;
                byte velByte = (byte)(_keyVelocities[i] * 255);
                byte handByte = (byte)(i < 24 ? 0 : 255); // C4 Split Point
                byte impactByte = (byte)(_keyImpacts[i] * 255);

                if (_keyStatePixels[pixelOffset] != velByte || _keyStatePixels[pixelOffset + 2] != impactByte)
                {
                    _keyStatePixels[pixelOffset] = velByte;
                    _keyStatePixels[pixelOffset + 1] = handByte;
                    _keyStatePixels[pixelOffset + 2] = impactByte;
                    _keyStatePixels[pixelOffset + 3] = 255;
                    textureNeedsUpdate = true;
                }
            }
        }

        if (textureNeedsUpdate && _keybedMaterial != null)
        {
            _keyStateImage.SetData(64, 1, false, Image.Format.Rgba8, _keyStatePixels);
            _keyStateTexture.Update(_keyStateImage);
        }

        // 2. Advance Metronome Clock
        _beatTimer += delta;
        if (_beatTimer >= _secondsPerBeat)
        {
            _beatTimer -= _secondsPerBeat;
            _lastBeatTimestamp = Time.GetTicksMsec() / 1000.0;
            AdvanceBeat();
        }

        // 3. Beat Pulse Decay
        if (_beatPulse > 0.0f)
        {
            _beatPulse = Mathf.Max(0.0f, _beatPulse - (float)delta * 4.5f);
            _keybedMaterial?.SetShaderParameter("beat_pulse", _beatPulse);
            _waterfallMaterial?.SetShaderParameter("beat_pulse", _beatPulse);
        }

        // 4. Hit Beam Flash Decay
        if (_hitBeamFlash > 0.0f)
        {
            _hitBeamFlash = Mathf.Max(0.0f, _hitBeamFlash - (float)delta * 8.0f);
            _waterfallMaterial?.SetShaderParameter("hit_beam_flash", _hitBeamFlash);
        }

        // 5. Incoming Chord Transition Breathing Pulse (Intensifies on beat 3 & 4)
        float targetPulse = (_currentBeat >= 3) ? 0.75f : 0.0f;
        _transitionPulse = Mathf.MoveToward(_transitionPulse, targetPulse, (float)delta * 3.5f);
        _nextChordMaterial?.SetShaderParameter("pulse_intensity", _transitionPulse);

        // 6. Red Glitch Flash Decay
        if (_glitchIntensity > 0.0f)
        {
            _glitchIntensity = Mathf.Max(0.0f, _glitchIntensity - (float)delta * 3.5f);
            _keybedMaterial?.SetShaderParameter("glitch_intensity", _glitchIntensity);
        }

        // 7. Kinetic Screen Shake
        if (_screenShake > 0.01f)
        {
            _screenShake = Mathf.Max(0.0f, _screenShake - (float)delta * 25.0f);
            float offsetX = (float)GD.RandRange(-_screenShake, _screenShake);
            float offsetY = (float)GD.RandRange(-_screenShake, _screenShake);
            _hudContainer.Position = _originalHudPos + new Vector2(offsetX, offsetY);
        }
        else
        {
            _hudContainer.Position = _originalHudPos;
        }

        // 8. Revert Retroactive Capture UI Button after timeout
        if (_isShowingRetroFeedback)
        {
            _retroFeedbackTimer -= delta;
            if (_retroFeedbackTimer <= 0.0)
            {
                _isShowingRetroFeedback = false;
                _btnNeverLostMelody.Text = "RETRO-BUFFER: [ 5m CAPTURE ARMED ]";
                _btnNeverLostMelody.Modulate = new Color(1.0f, 1.0f, 1.0f);
                _lblTimingVerdict.Modulate = new Color(0.0f, 0.96f, 0.83f);
                _lblTimingVerdict.Text = "ACCURACY: [ PERFECT! ]";
            }
        }
    }

    private void AdvanceBeat()
    {
        _currentBeat++;
        if (_currentBeat > 4)
        {
            _currentBeat = 1;
            _currentBar++;
        }

        _beatPulse = 1.0f;

        _lblBpmBeat.Text = $"[BPM: {_currentBpm} /// 4/4] [BAR: {_currentBar:D3} . BEAT: {_currentBeat}/4]";
        _lblLyricBar.Text = $">> CURRENT BAR {_currentBar:D3} // BEAT {_currentBeat} OF 4";

        var mission = _missions[_currentSongIndex];

        // Synchronize lyric lines to bars
        if (mission.Lyrics.Length > 0)
        {
            int lyricIndex = (_currentBar - 1) % mission.Lyrics.Length;
            _lblLyricTeleprompter.Text = $"\"{mission.Lyrics[lyricIndex]}\"";
        }

        // Broadcast telemetry to connected mobile teleprompters
        var tickData = new PlaybackTickData
        {
            SongId = $"mission_{_currentSongIndex + 1:D2}",
            Bar = _currentBar,
            Beat = _currentBeat,
            Bpm = _currentBpm,
            CurrentChord = mission.CurrentChord,
            NextChord = mission.NextChord,
            ActivePatch = mission.PatchName
        };

        _ = HubServer.BroadcastPlaybackTickAsync(tickData);
    }

    public void SelectMission(int index)
    {
        if (index < 0 || index >= _missions.Count) return;
        _currentSongIndex = index;
        var m = _missions[index];

        _currentBpm = m.Bpm;
        _secondsPerBeat = 60.0 / _currentBpm;
        _currentBar = 1;
        _currentBeat = 1;
        _totalHits = 0;
        _perfectHits = 0;

        _lblActiveMission.Text = $"> {m.Title}";
        _lblCurrentChord.Text = $"[ {m.CurrentChord} ]";
        _lblNextChord.Text = $"NEXT CHORD -> {m.NextChord}";
        _lblBpmBeat.Text = $"[BPM: {_currentBpm} /// 4/4] [BEAT: 01/4]";
        _lblLyricBar.Text = ">> CURRENT BAR 001 // BEAT 1 OF 4";

        if (m.Lyrics.Length > 0)
        {
            _lblLyricTeleprompter.Text = $"\"{m.Lyrics[0]}\"";
        }

        // Volatile hardware patch change to Roland Go:Keys 3 (Zero flash writes)
        MidiEngine.SelectPatchVolatile(channel: 0, m.BankMsb, m.BankLsb, m.Pc);
    }

    public void TriggerPanicPurge()
    {
        // 1. Hardware Panic (CC 120, CC 123, CC 64 across 16 channels)
        MidiEngine.TriggerPanic();

        // 2. Trigger Red Glitch Flash & Screen Shake
        _glitchIntensity = 1.0f;
        _screenShake = ScreenShakeIntensity * 2.5f;

        // 3. Clear all active key states
        lock (_stateLock)
        {
            Array.Clear(_keyVelocities, 0, _keyVelocities.Length);
            Array.Clear(_keyImpacts, 0, _keyImpacts.Length);
            Array.Clear(_keyStatePixels, 0, _keyStatePixels.Length);
        }

        _keyStateImage.SetData(64, 1, false, Image.Format.Rgba8, _keyStatePixels);
        _keyStateTexture.Update(_keyStateImage);

        _lblTimingVerdict.Text = "ACCURACY: [ ALL CHANNELS PURGED ]";
        _lblTimingVerdict.Modulate = new Color(0.93f, 0.14f, 0.23f);

        GD.PrintRich("[color=red][b]/// EMERGENCY PURGE EXECUTED - ALL SOUND TERMINATED[/b][/color]");
    }

    public void OnNeverLostMelodyPressed()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        string exportDir = Path.Combine(Directory.GetCurrentDirectory(), "captures");
        string fileName = $"session_{timestamp}.mid";
        string filePath = Path.Combine(exportDir, fileName);

        bool success = CaptureBuffer.ExportToMidi(
            filePath,
            out int eventCount,
            out int estimatedBars,
            RetroactiveBuffer.FiveMinutesUs,
            _currentBpm
        );

        if (success)
        {
            _isShowingRetroFeedback = true;
            _retroFeedbackTimer = 3.0; // 3 seconds feedback

            // Golden/Cyan HUD Flash on Button & Toast
            _btnNeverLostMelody.Text = $">> CAPTURED: {fileName} ({estimatedBars} bars saved)";
            _btnNeverLostMelody.Modulate = new Color(0.0f, 0.96f, 0.83f);

            _lblTimingVerdict.Text = $">> SAVED: {fileName} ({eventCount} events)";
            _lblTimingVerdict.Modulate = new Color(1.0f, 0.72f, 0.01f);

            GD.PrintRich($"[color=green][b]>> Retroactive Melody Exported:[/b] {filePath} ({eventCount} events, {estimatedBars} bars)[/color]");
        }
        else
        {
            _lblTimingVerdict.Text = "RETRO-BUFFER: [ NO RECENT EVENTS ]";
            _lblTimingVerdict.Modulate = new Color(1.0f, 0.82f, 0.05f);
        }
    }

    public override void _ExitTree()
    {
        HubServer.Dispose();
        MidiEngine.Dispose();
        base._ExitTree();
    }
}
