using Godot;
using AnantaNada.Core.Midi;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Diegetic Game Console Controller for Roland Go:Keys 3 (Cockpit HUD).
/// Blends Atlus Persona 5 slanted typography, Arknights tactical telemetry,
/// and Hi-Fi Rush beat-synced rhythm scoring.
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
    private Label _lblRhythmAccuracy = null!;
    private Label _lblTimingVerdict = null!;
    private Label _lblStrikeGauge = null!;
    private Label _lblVelocityRank = null!;
    private Button _btnPanic = null!;
    private Button _btnNeverLostMelody = null!;
    private ColorRect _keybedRect = null!;
    private ColorRect _waterfallRect = null!;
    private Control _hudContainer = null!;

    // Shaders
    private ShaderMaterial? _keybedMaterial;
    private ShaderMaterial? _waterfallMaterial;

    // Keybed 64x1 State Texture
    private readonly byte[] _keyStatePixels = new byte[64 * 4];
    private readonly float[] _keyVelocities = new float[64];
    private readonly float[] _keyImpacts = new float[64];
    private Image _keyStateImage = null!;
    private ImageTexture _keyStateTexture = null!;

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

    // Visual FX (Hit Beam, Beat Pulse, Glitch Flash, Screen Shake)
    private float _beatPulse = 0.0f;
    private float _hitBeamFlash = 0.0f;
    private float _screenShake = 0.0f;
    private float _glitchIntensity = 0.0f;
    private Vector2 _originalHudPos = Vector2.Zero;

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
        // 1. Initialize Audio/MIDI & Network
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
        _lblCurrentChord = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/CurrentChordBox/ChordLabel") ?? new Label();
        _lblNextChord = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/NextChordBox/NextLabel") ?? new Label();
        _lblLyricTeleprompter = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/LyricBox/LyricText") ?? new Label();
        _lblRhythmAccuracy = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/AccuracyValue") ?? new Label();
        _lblTimingVerdict = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/TimingVerdict") ?? new Label();
        _lblStrikeGauge = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/StrikeGauge") ?? new Label();
        _lblVelocityRank = GetNodeOrNull<Label>("HUD/CenterSplit/CenterCascade/CoachDeck/RankBadge") ?? new Label();
        _btnPanic = GetNodeOrNull<Button>("HUD/TopBar/PanicButton") ?? new Button();
        _btnNeverLostMelody = GetNodeOrNull<Button>("HUD/CenterSplit/CenterCascade/CoachDeck/RetroBufferBtn") ?? new Button();
        _keybedRect = GetNodeOrNull<ColorRect>("HUD/BottomKeybed/KeybedContainer/KeybedRect") ?? new ColorRect();
        _waterfallRect = GetNodeOrNull<ColorRect>("HUD/CenterSplit/CenterCascade/WaterfallRect") ?? new ColorRect();

        // 3. Materials
        if (_keybedRect.Material is ShaderMaterial km) _keybedMaterial = km;
        if (_waterfallRect.Material is ShaderMaterial wm) _waterfallMaterial = wm;

        // 4. Setup Keybed State Texture
        _keyStateImage = Image.CreateEmpty(64, 1, false, Image.Format.Rgba8);
        _keyStateTexture = ImageTexture.CreateFromImage(_keyStateImage);
        _keybedMaterial?.SetShaderParameter("key_states", _keyStateTexture);

        // 5. Wire MIDI Engine & Server Callbacks
        MidiEngine.OnMessageReceived += HandleIncomingMidi;
        MidiEngine.OnLog += msg => GD.Print(msg);
        MidiEngine.OnDeviceConnected += dev => CallDeferred(nameof(UpdateDeviceLabel), dev, true);
        MidiEngine.OnDeviceDisconnected += () => CallDeferred(nameof(UpdateDeviceLabel), "DISCONNECTED", false);

        HubServer.OnLog += msg => GD.Print(msg);
        HubServer.OnPanicTriggered += () => CallDeferred(nameof(TriggerPanicPurge));
        HubServer.OnSetlistItemTriggered += payload => CallDeferred(nameof(SelectMission), payload.SongIndex);

        _btnPanic.Pressed += TriggerPanicPurge;
        _btnNeverLostMelody.Pressed += OnNeverLostMelodyPressed;

        // 6. Connect Hardware & Start Mini-Server
        MidiEngine.Connect("Go:Keys");
        HubServer.Start();

        var ips = StageHubServer.GetLocalIpAddresses();
        string displayIp = ips.Count > 0 ? $"{ips[0]}:{HttpPort}" : $"127.0.0.1:{HttpPort}";
        _lblIpAddress.Text = $"> LINK: http://{displayIp}";

        // Select initial mission
        SelectMission(0);
    }

    private void UpdateDeviceLabel(string name, bool connected)
    {
        _lblDeviceStatus.Text = connected ? $"> LINK: ROLAND [{name.ToUpper()}]" : "> LINK: NO HARDWARE (STANDALONE)";
        _lblDeviceStatus.Modulate = connected ? new Color(0.0f, 0.96f, 0.83f) : new Color(0.93f, 0.14f, 0.23f);
    }

    private void HandleIncomingMidi(in MidiRawMessage msg)
    {
        // 1. Retroactive circular RAM buffer push
        CaptureBuffer.Push(in msg);

        if (msg.IsNoteOn)
        {
            int keyIndex = msg.Data1 - MidiConstants.LowestNote;
            if (keyIndex >= 0 && keyIndex < 61)
            {
                float normalizedVel = msg.Data2 / 127.0f;
                _keyVelocities[keyIndex] = normalizedVel;
                _keyImpacts[keyIndex] = 1.0f;

                CallDeferred(nameof(EvaluateTimingAndVelocity), msg.Data2);
            }
        }
        else if (msg.IsNoteOff)
        {
            int keyIndex = msg.Data1 - MidiConstants.LowestNote;
            if (keyIndex >= 0 && keyIndex < 61)
            {
                _keyVelocities[keyIndex] = 0.0f;
            }
        }
    }

    private void EvaluateTimingAndVelocity(byte rawVelocity)
    {
        // 1. Velocity Dynamic Gauge & Rank (Armored Core / Cyberpunk HUD)
        string rank;
        Color rankColor;
        string dynamicDashes;

        if (rawVelocity >= 110)
        {
            rank = "RANK [S]";
            rankColor = new Color(1.0f, 0.2f, 0.4f); // Crimson Overdrive
            dynamicDashes = "||||||||||||||||||||";
            _screenShake = ScreenShakeIntensity; // Screen shake on S-rank strike
            _hitBeamFlash = 1.8f;
        }
        else if (rawVelocity >= 90)
        {
            rank = "RANK [A]";
            rankColor = new Color(0.0f, 0.96f, 0.83f); // Cyber Cyan
            dynamicDashes = "|||||||||||||||.....";
            _hitBeamFlash = 1.2f;
        }
        else if (rawVelocity >= 70)
        {
            rank = "RANK [B]";
            rankColor = new Color(1.0f, 0.82f, 0.05f); // Electric Amber
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

        // 2. Rhythm Timing Coach (Hi-Fi Rush / Muse Dash Accuracy Evaluation)
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
        _lblTimingVerdict.Text = $"ACCURACY: {verdict}";
        _lblTimingVerdict.Modulate = verdictColor;
        _lblRhythmAccuracy.Text = $"SCORE ACC: {_rollingAccuracy:F1}%";
    }

    public override void _Process(double delta)
    {
        // 1. Decay visual impact flashes and update Keybed Shader texture
        bool textureNeedsUpdate = false;
        for (int i = 0; i < 61; i++)
        {
            if (_keyImpacts[i] > 0.0f)
            {
                _keyImpacts[i] = Mathf.Max(0.0f, _keyImpacts[i] - (float)delta * 14.0f);
                textureNeedsUpdate = true;
            }

            int pixelOffset = i * 4;
            byte velByte = (byte)(_keyVelocities[i] * 255);
            // Split Point: C4 (Note 60, index 24). Zone A Bass = 0 (Amber), Zone B Lead = 255 (Cyan)
            byte handByte = (byte)(i < 24 ? 0 : 255);
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

        if (textureNeedsUpdate && _keybedMaterial != null)
        {
            _keyStateImage.SetData(64, 1, false, Image.Format.Rgba8, _keyStatePixels);
            _keyStateTexture.Update(_keyStateImage);
        }

        // 2. Metronome Beat Advance
        _beatTimer += delta;
        if (_beatTimer >= _secondsPerBeat)
        {
            _beatTimer -= _secondsPerBeat;
            _lastBeatTimestamp = Time.GetTicksMsec() / 1000.0;
            AdvanceBeat();
        }

        // 3. Hi-Fi Rush Beat Pulse Decay
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

        // 5. Red Glitch Flash Decay (Post-Panic)
        if (_glitchIntensity > 0.0f)
        {
            _glitchIntensity = Mathf.Max(0.0f, _glitchIntensity - (float)delta * 3.5f);
            _keybedMaterial?.SetShaderParameter("glitch_intensity", _glitchIntensity);
        }

        // 6. Kinetic Screen Shake
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
    }

    private void AdvanceBeat()
    {
        _currentBeat++;
        if (_currentBeat > 4)
        {
            _currentBeat = 1;
            _currentBar++;
        }

        _beatPulse = 1.0f; // Trigger rhythm pulse on beat

        _lblBpmBeat.Text = $"[BPM: {_currentBpm} /// 4/4] [BAR: {_currentBar:D3} . BEAT: {_currentBeat}/4]";

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

        if (m.Lyrics.Length > 0)
        {
            _lblLyricTeleprompter.Text = $"\"{m.Lyrics[0]}\"";
        }

        // Volatile hardware patch change to Roland Go:Keys 3 (Zero flash writes)
        MidiEngine.SelectPatchVolatile(channel: 0, m.BankMsb, m.BankLsb, m.Pc);
    }

    public void TriggerPanicPurge()
    {
        // 1. Hardware Panic
        MidiEngine.TriggerPanic();

        // 2. Trigger Red Glitch Flash & Screen Shake
        _glitchIntensity = 1.0f;
        _screenShake = ScreenShakeIntensity * 2.0f;

        // 3. Clear all active key states
        Array.Clear(_keyVelocities, 0, _keyVelocities.Length);
        Array.Clear(_keyImpacts, 0, _keyImpacts.Length);
        Array.Clear(_keyStatePixels, 0, _keyStatePixels.Length);
        _keyStateImage.SetData(64, 1, false, Image.Format.Rgba8, _keyStatePixels);
        _keyStateTexture.Update(_keyStateImage);

        _lblTimingVerdict.Text = "ACCURACY: [ ALL CHANNELS PURGED ]";
        _lblTimingVerdict.Modulate = new Color(0.93f, 0.14f, 0.23f);

        GD.PrintRich("[color=red][b]/// EMERGENCY PURGE EXECUTED - ALL SOUND TERMINATED[/b][/color]");
    }

    public void OnNeverLostMelodyPressed()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string exportDir = Path.Combine(Directory.GetCurrentDirectory(), "captures");
        string filePath = Path.Combine(exportDir, $"AnantaNada_Cockpit_{timestamp}.mid");

        bool success = CaptureBuffer.ExportToMidi(filePath, RetroactiveBuffer.FiveMinutesUs, _currentBpm);
        if (success)
        {
            _lblTimingVerdict.Text = "RETRO-BUFFER: [ 5m SMF EXPORTED! ]";
            _lblTimingVerdict.Modulate = new Color(0.0f, 0.96f, 0.83f);
            GD.PrintRich($"[color=green][b]Never-Lost Melody Saved:[/b] {filePath}[/color]");
        }
    }

    public override void _ExitTree()
    {
        HubServer.Dispose();
        MidiEngine.Dispose();
        base._ExitTree();
    }
}
