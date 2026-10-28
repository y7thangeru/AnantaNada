using System.Text.Json;
using Godot;
using AnantaNada.Core.Data;
using AnantaNada.Core.Midi;
using AnantaNada.Core.Models;
using AnantaNada.Core.Services;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Lead Game Console Controller for Roland Go:Keys 3 (Cockpit HUD).
/// Orchestrates Jalur C (Tone Bank Catalog & Setlist Automation) with
/// hardware-safe Channel 4 patch changes, RPN coarse transposition,
/// dynamic 64x1 keybed texture, and bidirectional StageHub telemetry.
/// </summary>
public partial class MainStageController : Control
{
    [Export] public int HttpPort { get; set; } = 8080;
    [Export] public float ScreenShakeIntensity { get; set; } = 8.0f;

    // Subsystems & Services
    public NativeMidiEngine MidiEngine { get; private set; } = null!;
    public RetroactiveBuffer CaptureBuffer { get; private set; } = null!;
    public StageHubServer HubServer { get; private set; } = null!;
    public ToneService ToneLibrary { get; private set; } = null!;

    // UI Nodes
    private Label _lblDeviceStatus = null!;
    private Label _lblIpAddress = null!;
    private Label _lblBpmBeat = null!;
    private Label _lblActiveMission = null!;
    private Label _lblCurrentChord = null!;
    private Label _lblNextChord = null!;
    private Label _lblTranspose = null!;
    private Label _lblLyricTeleprompter = null!;
    private Label _lblLyricBar = null!;
    private Label _lblRhythmAccuracy = null!;
    private Label _lblTimingVerdict = null!;
    private Label _lblStrikeGauge = null!;
    private Label _lblVelocityRank = null!;
    private Label _lblZoneA = null!;
    private Label _lblZoneB = null!;
    private Label _lblSplitLine = null!;
    private Button _btnPanic = null!;
    private Button _btnNeverLostMelody = null!;
    private ColorRect _keybedRect = null!;
    private ColorRect _waterfallRect = null!;
    private PanelContainer _nextChordBox = null!;
    private PanelContainer _currentChordBox = null!;
    private PanelContainer _nextCard1 = null!;
    private PanelContainer _nextCard2 = null!;
    private PanelContainer _nextCard3 = null!;
    private Label _lblNextCard1 = null!;
    private Label _lblNextCard2 = null!;
    private Label _lblNextCard3 = null!;
    private LineEdit _searchBox = null!;
    private Button _btnFilterAll = null!;
    private Button _btnFilterIndo = null!;
    private Button _btnFilterWestern = null!;
    private Button _btnPrevPage = null!;
    private Button _btnNextPage = null!;
    private Label _lblPageIndicator = null!;

    private string _currentCategory = "ALL";
    private string _searchKeyword = "";
    private int _currentPage = 0;
    private readonly List<SongMission> _displayedSongs = new();
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

    // Setlist & Mission State
    private SetlistDocument _setlist = new();
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

    public override void _Ready()
    {
        // 1. Initialize Core Engines & Catalog Services
        CaptureBuffer = new RetroactiveBuffer();
        MidiEngine = new NativeMidiEngine();
        HubServer = new StageHubServer(HttpPort);
        ToneLibrary = new ToneService();

        // 2. Load Setlist Configuration
        LoadSetlist();

        // 3. Resolve Scene Tree Nodes
        _hudContainer = GetNodeOrNull<Control>("HUD") ?? this;
        _originalHudPos = _hudContainer.Position;

        _lblDeviceStatus = GetNodeOrNull<Label>("HUD/TopBar/DevicePill/StatusLabel") ?? new Label();
        _lblIpAddress = GetNodeOrNull<Label>("HUD/TopBar/NetworkPill/IpLabel") ?? new Label();
        _lblBpmBeat = GetNodeOrNull<Label>("HUD/TopBar/BpmPill/BpmLabel") ?? new Label();
        _lblActiveMission = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/ActiveCard/MissionName") ?? new Label();

        _currentChordBox = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/ChordHud/CurrentChordBox") ?? new PanelContainer();
        _lblCurrentChord = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/CurrentChordBox/VBox/ChordLabel") ?? new Label();
        _lblTranspose = GetNodeOrNull<Label>("HUD/CenterSplit/ChordHud/CurrentChordBox/VBox/TransposePill") ?? new Label();

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

        _lblZoneA = GetNodeOrNull<Label>("HUD/BottomKeybed/ZoneRow/ZoneALabel") ?? new Label();
        _lblSplitLine = GetNodeOrNull<Label>("HUD/BottomKeybed/ZoneRow/SplitLine") ?? new Label();
        _lblZoneB = GetNodeOrNull<Label>("HUD/BottomKeybed/ZoneRow/ZoneBLabel") ?? new Label();

        _nextCard1 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard1") ?? new PanelContainer();
        _nextCard2 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard2") ?? new PanelContainer();
        _nextCard3 = GetNodeOrNull<PanelContainer>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard3") ?? new PanelContainer();

        _lblNextCard1 = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard1/Label") ?? new Label();
        _lblNextCard2 = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard2/Label") ?? new Label();
        _lblNextCard3 = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard3/Label") ?? new Label();

        _searchBox = GetNodeOrNull<LineEdit>("HUD/CenterSplit/SetlistDeck/MissionContainer/SearchBox") ?? new LineEdit();
        _btnFilterAll = GetNodeOrNull<Button>("HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterAll") ?? new Button();
        _btnFilterIndo = GetNodeOrNull<Button>("HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterIndo") ?? new Button();
        _btnFilterWestern = GetNodeOrNull<Button>("HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterWestern") ?? new Button();
        _btnPrevPage = GetNodeOrNull<Button>("HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/BtnPrevPage") ?? new Button();
        _btnNextPage = GetNodeOrNull<Button>("HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/BtnNextPage") ?? new Button();
        _lblPageIndicator = GetNodeOrNull<Label>("HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/PageIndicator") ?? new Label();

        // 4. Shaders
        if (_keybedRect.Material is ShaderMaterial km) _keybedMaterial = km;
        if (_waterfallRect.Material is ShaderMaterial wm) _waterfallMaterial = wm;
        if (_nextChordBox.Material is ShaderMaterial ncm) _nextChordMaterial = ncm;
        if (_currentChordBox.Material is ShaderMaterial ccm) _currentChordMaterial = ccm;

        // 5. Setup Keybed State Texture (64x1 RGBA8)
        _keyStateImage = Image.CreateEmpty(64, 1, false, Image.Format.Rgba8);
        _keyStateTexture = ImageTexture.CreateFromImage(_keyStateImage);
        _keybedMaterial?.SetShaderParameter("key_states", _keyStateTexture);

        // 6. Wire MIDI Engine Pipelines
        MidiEngine.OnMessageReceived += (in MidiRawMessage msg) => CaptureBuffer.Push(in msg);
        MidiEngine.OnNoteOn += HandleMidiNoteOn;
        MidiEngine.OnNoteOff += HandleMidiNoteOff;
        MidiEngine.OnLog += msg => GD.Print(msg);
        MidiEngine.OnDeviceConnected += dev => CallDeferred(nameof(UpdateDeviceLabel), dev, true);
        MidiEngine.OnDeviceDisconnected += () => CallDeferred(nameof(UpdateDeviceLabel), "DISCONNECTED", false);

        // 7. Wire StageHub Server Pipelines
        HubServer.OnLog += msg => GD.Print(msg);
        HubServer.OnPanicTriggered += () => CallDeferred(nameof(TriggerPanicPurge));
        HubServer.OnSetlistItemTriggered += payload => CallDeferred(nameof(SelectMission), payload.SongIndex);

        // 8. Wire UI Button Actions & Repertoire Filtering
        _btnPanic.Pressed += TriggerPanicPurge;
        _btnNeverLostMelody.Pressed += OnNeverLostMelodyPressed;

        _searchBox.TextChanged += OnSearchTextChanged;
        _btnFilterAll.Pressed += () => SetFilterCategory("ALL");
        _btnFilterIndo.Pressed += () => SetFilterCategory("Indo_Band");
        _btnFilterWestern.Pressed += () => SetFilterCategory("Western_Band");
        _btnPrevPage.Pressed += OnPrevPagePressed;
        _btnNextPage.Pressed += OnNextPagePressed;

        // Desktop Setlist Card Click handlers
        _nextCard1.GuiInput += @event => HandleCardSlotClick(@event, 0);
        _nextCard2.GuiInput += @event => HandleCardSlotClick(@event, 1);
        _nextCard3.GuiInput += @event => HandleCardSlotClick(@event, 2);

        // 9. Connect Hardware & Start Mini-Server
        MidiEngine.Connect("Go:Keys");
        HubServer.Start();

        var ips = StageHubServer.GetLocalIpAddresses();
        string displayIp = ips.Count > 0 ? $"{ips[0]}:{HttpPort}" : $"127.0.0.1:{HttpPort}";
        _lblIpAddress.Text = $"> LINK: http://{displayIp}";

        // Select initial mission
        SelectMission(0);
    }

    private void LoadSetlist()
    {
        // Prioritize the 100-song master repertoire if available
        if (ToneLibrary.AllSongs.Count > 0)
        {
            _setlist = new SetlistDocument
            {
                SetlistName = "Master 100 Stage Repertoire",
                Songs = ToneLibrary.AllSongs.ToList()
            };
        }
        else
        {
            string repPath = Path.Combine(Directory.GetCurrentDirectory(), "setlists", "master_100_repertoire.json");
            if (File.Exists(repPath))
            {
                try
                {
                    string json = File.ReadAllText(repPath);
                    var list = JsonSerializer.Deserialize<List<SongMission>>(json);
                    if (list != null && list.Count > 0)
                    {
                        _setlist = new SetlistDocument { SetlistName = "Master 100 Stage Repertoire", Songs = list };
                        RefreshFilteredSongs();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[Setlist] Notice parsing master repertoire: {ex.Message}");
                }
            }

            _setlist = new SetlistDocument
            {
                SetlistName = "Fallback 100 Repertoire",
                Songs = RepertoireDatabaseBuilder.Build100Repertoire()
            };
        }

        RefreshFilteredSongs();
    }

    private void SetFilterCategory(string category)
    {
        _currentCategory = category;
        _currentPage = 0;
        RefreshFilteredSongs();
    }

    private void OnSearchTextChanged(string newText)
    {
        _searchKeyword = newText;
        _currentPage = 0;
        RefreshFilteredSongs();
    }

    private void OnPrevPagePressed()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            RefreshCardsUI();
        }
    }

    private void OnNextPagePressed()
    {
        int maxPage = Math.Max(0, (_displayedSongs.Count - 1) / 3);
        if (_currentPage < maxPage)
        {
            _currentPage++;
            RefreshCardsUI();
        }
    }

    private void RefreshFilteredSongs()
    {
        _displayedSongs.Clear();
        var results = ToneLibrary.SearchSongs(_searchKeyword, _currentCategory);
        _displayedSongs.AddRange(results);
        RefreshCardsUI();
    }

    private void RefreshCardsUI()
    {
        int total = _displayedSongs.Count;
        int maxPage = Math.Max(1, (int)Math.Ceiling(total / 3.0));
        _currentPage = Math.Clamp(_currentPage, 0, Math.Max(0, maxPage - 1));

        _lblPageIndicator.Text = $"PAGE {_currentPage + 1}/{maxPage} ({total})";

        int startIdx = _currentPage * 3;
        UpdateCardSlot(_nextCard1, _lblNextCard1, startIdx);
        UpdateCardSlot(_nextCard2, _lblNextCard2, startIdx + 1);
        UpdateCardSlot(_nextCard3, _lblNextCard3, startIdx + 2);
    }

    private void UpdateCardSlot(PanelContainer card, Label label, int index)
    {
        if (index < _displayedSongs.Count)
        {
            card.Visible = true;
            var s = _displayedSongs[index];
            label.Text = $"  {s.Id}. {s.Title} [{s.KeySignature} // {s.Bpm} BPM]";
        }
        else
        {
            card.Visible = false;
        }
    }

    private void HandleCardSlotClick(InputEvent @event, int slotOffset)
    {
        if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
            int targetIdx = (_currentPage * 3) + slotOffset;
            if (targetIdx >= 0 && targetIdx < _displayedSongs.Count)
            {
                var targetSong = _displayedSongs[targetIdx];
                int setlistIdx = _setlist.Songs.IndexOf(targetSong);
                if (setlistIdx >= 0)
                {
                    SelectMission(setlistIdx);
                }
            }
        }
    }

    private void UpdateDeviceLabel(string name, bool connected)
    {
        _lblDeviceStatus.Text = connected ? $"> LINK: ROLAND [{name.ToUpper()}]" : "> LINK: NO HARDWARE (STANDALONE)";
        _lblDeviceStatus.Modulate = connected ? new Color(0.0f, 0.96f, 0.83f) : new Color(0.93f, 0.14f, 0.23f);
    }

    private void HandleMidiNoteOn(byte channel, byte note, byte velocity)
    {
        int keyIndex = note - MidiConstants.LowestNote; // C2 (36) to C7 (96)
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
        // 1. Keybed visual updates
        bool textureNeedsUpdate = false;
        int splitKeyIndex = Math.Clamp(GetCurrentSplitPoint() - MidiConstants.LowestNote, 0, 60);

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
                byte handByte = (byte)(i < splitKeyIndex ? 0 : 255); // Zone A = Amber (0), Zone B = Cyan (255)
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

        // 5. Incoming Chord Transition Breathing Pulse
        float targetPulse = (_currentBeat >= 3) ? 0.75f : 0.0f;
        _transitionPulse = Mathf.MoveToward(_transitionPulse, targetPulse, (float)delta * 3.5f);
        _nextChordMaterial?.SetShaderParameter("pulse_intensity", _transitionPulse);

        // 6. Red Glitch Flash Decay
        if (_glitchIntensity > 0.0f)
        {
            _glitchIntensity = Mathf.Max(0.0f, _glitchIntensity - (float)delta * 3.5f);
            _keybedMaterial?.SetShaderParameter("glitch_intensity", _glitchIntensity);
        }

        // 7. Screen Shake
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

        // 8. Revert Retroactive Capture UI Button
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

    private int GetCurrentSplitPoint()
    {
        if (_setlist.Songs.Count > _currentSongIndex)
        {
            return _setlist.Songs[_currentSongIndex].SplitPoint;
        }
        return 60; // C4
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

        var song = _setlist.Songs[_currentSongIndex];

        // Resolve current and next chord from ChordsTimeline
        string currentChord = "G#m7";
        string nextChord = "C#m7";

        if (song.ChordsTimeline.Count > 0)
        {
            // Find most recent chord event at or before current bar
            var chordEvt = song.ChordsTimeline.LastOrDefault(c => c.Bar <= _currentBar) ?? song.ChordsTimeline[0];
            currentChord = chordEvt.CurrentChord;
            nextChord = chordEvt.NextChord;
        }

        _lblCurrentChord.Text = $"[ {currentChord} ]";
        _lblNextChord.Text = $"NEXT CHORD -> {nextChord}";

        // Synchronize lyrics from LyricsTimeline
        if (song.LyricsTimeline.Count > 0)
        {
            int lyricIdx = (_currentBar - 1) % song.LyricsTimeline.Count;
            _lblLyricTeleprompter.Text = $"\"{song.LyricsTimeline[lyricIdx].Text}\"";
        }

        // Broadcast telemetry to connected mobile teleprompters
        var primaryTone = ToneLibrary.ResolveTone(song.PrimaryTone);
        var tickData = new PlaybackTickData
        {
            SongId = song.SongId,
            Bar = _currentBar,
            Beat = _currentBeat,
            Bpm = _currentBpm,
            CurrentChord = currentChord,
            NextChord = nextChord,
            ActivePatch = primaryTone?.Name ?? song.PrimaryTone.Name
        };

        _ = HubServer.BroadcastPlaybackTickAsync(tickData);
    }

    /// <summary>
    /// Jalur C: Executes hardware-safe volatile tone selection on Roland Go:Keys 3 (Channel 4 = index 3)
    /// and coarse transposition via RPN.
    /// </summary>
    public void SelectMission(int index)
    {
        if (index < 0 || index >= _setlist.Songs.Count) return;
        _currentSongIndex = index;
        var song = _setlist.Songs[index];

        _currentBpm = song.Bpm;
        _secondsPerBeat = 60.0 / _currentBpm;
        _currentBar = 1;
        _currentBeat = 1;
        _totalHits = 0;
        _perfectHits = 0;

        // Resolve Tones from ToneService
        var primaryTone = ToneLibrary.ResolveTone(song.PrimaryTone);
        var splitTone = song.SplitTone != null ? ToneLibrary.ResolveTone(song.SplitTone) : null;

        // 1. Dispatch Hardware-Safe Volatile Tone Change to Roland Go:Keys 3 on Channel 4 (0-indexed 3)
        if (primaryTone != null)
        {
            MidiEngine.SelectPatchVolatile(
                channel: MidiConstants.GoKeys3DefaultPatchChannel, // MIDI Channel 4 (0-indexed 3)
                primaryTone.Msb,
                primaryTone.Lsb,
                primaryTone.RawProgramChange // Normalizes 1-indexed to 0-indexed (pc - 1)
            );
        }

        // 2. Dispatch Coarse Transposition via RPN
        MidiEngine.SetCoarseTranspose(
            channel: MidiConstants.GoKeys3DefaultPatchChannel,
            song.Transpose
        );

        // 3. Update HUD Display Elements
        _lblActiveMission.Text = $"> {song.Title.ToUpper()} - {song.Artist.ToUpper()}";
        _lblBpmBeat.Text = $"[BPM: {_currentBpm} /// {song.TimeSignature}] [BEAT: 01/4]";
        _lblTranspose.Text = $"TRANSPOSE: [ {(song.Transpose >= 0 ? "+" : "")}{song.Transpose} SEMITONES ]";
        _lblLyricBar.Text = ">> CURRENT BAR 001 // BEAT 1 OF 4";

        string currentChord = "C";
        string nextChord = "G";
        if (song.ChordsTimeline.Count > 0)
        {
            currentChord = song.ChordsTimeline[0].CurrentChord;
            nextChord = song.ChordsTimeline[0].NextChord;
        }
        else if (song.ChordProgression != null)
        {
            var parts = song.ChordProgression.Intro.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) currentChord = parts[0];
            if (parts.Length > 1) nextChord = parts[1];
        }

        _lblCurrentChord.Text = $"[ {currentChord} ]";
        _lblNextChord.Text = $"NEXT CHORD -> {nextChord}";

        if (song.LyricsTimeline.Count > 0)
        {
            _lblLyricTeleprompter.Text = $"\"{song.LyricsTimeline[0].Text}\"";
        }
        else if (song.ChordProgression != null)
        {
            _lblLyricTeleprompter.Text = $"\"{song.Title} - {song.Artist} // Intro: {song.ChordProgression.Intro}\"";
        }

        // 4. Update Split Zone Labels
        string splitNoteName = $"C{song.SplitPoint / 12 - 1}"; // e.g. 60 -> C4
        _lblSplitLine.Text = $"[ SPLIT POINT: {splitNoteName} ]";
        _lblZoneA.Text = $"ZONE A: SUB-BASS (AMBER PULSE // {splitTone?.Name ?? "PAD/BASS"} // C2-{splitNoteName})";
        _lblZoneB.Text = $"ZONE B: NEO-SOLO LEAD (CYAN OVERDRIVE // {primaryTone?.Name ?? "LEAD"} // {splitNoteName}-C7)";

        // Set shader split point parameter
        float splitNorm = (song.SplitPoint - MidiConstants.LowestNote) / 60.0f;
        _keybedMaterial?.SetShaderParameter("split_point_norm", splitNorm);

        GD.PrintRich($"[color=cyan][b]100-Repertoire Mission Activated:[/b] '{song.Title}' ({song.Artist}) -> Ch4 MSB:{primaryTone?.Msb} LSB:{primaryTone?.Lsb} PC:{primaryTone?.Pc} Transpose:{song.Transpose}[/color]");
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
