using Godot;
using AnantaNada.Core.Midi;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Lead Controller orchestrating the Godot 4 Stage HUD, hardware-safe MIDI engine,
/// retroactive 5-minute capture buffer, and embedded StageHub WebSocket server.
/// </summary>
public partial class MainStageController : Control
{
    [Export] public int HttpPort { get; set; } = 8080;
    [Export] public float KeyReleaseDecayRate { get; set; } = 8.0f; // Velocity visual decay rate

    // Subsystem instances
    public NativeMidiEngine MidiEngine { get; private set; } = null!;
    public RetroactiveBuffer CaptureBuffer { get; private set; } = null!;
    public StageHubServer HubServer { get; private set; } = null!;

    // UI Node References (bound in Godot Scene)
    private Label _lblDeviceStatus = null!;
    private Label _lblIpAddress = null!;
    private Label _lblSongTitle = null!;
    private Label _lblCurrentChord = null!;
    private Label _lblNextChord = null!;
    private Label _lblBarBeat = null!;
    private Label _lblBpm = null!;
    private Label _lblActivePatch = null!;
    private Button _btnPanic = null!;
    private Button _btnNeverLostMelody = null!;
    private ColorRect _keybedRect = null!;
    private ShaderMaterial? _keybedMaterial;

    // 64-width Keybed State Buffer (R: Velocity 0..1, G: Hand 0=Amber/1=Cyan, B: Impact)
    private readonly byte[] _keyStatePixels = new byte[64 * 4]; // RGBA8
    private readonly float[] _keyVelocities = new float[64];
    private readonly float[] _keyImpacts = new float[64];
    private Image _keyStateImage = null!;
    private ImageTexture _keyStateTexture = null!;

    // Runtime state
    private int _currentSongIndex = 0;
    private int _currentBar = 1;
    private int _currentBeat = 1;
    private int _currentBpm = 128;
    private double _beatTimer = 0.0;
    private double _secondsPerBeat = 60.0 / 128.0;

    // Example setlist
    private record SongItem(string Title, string CurrentChord, string NextChord, string PatchName, byte BankMsb, byte BankLsb, byte Pc, int Bpm);
    private readonly List<SongItem> _setlist = new()
    {
        new SongItem("01. Neo Shibuya Dawn", "G#m7", "C#m7", "Vintage EP 1", 87, 64, 14, 128),
        new SongItem("02. Cybernetic Groove", "F#maj7", "B7", "Fat Saw Lead", 87, 65, 4, 134),
        new SongItem("03. Midnight Reverie", "Emaj9", "A#dim7", "Warm Stage Grand", 87, 64, 1, 92),
        new SongItem("04. Overdrive Anthem", "D#m7", "G#7", "Brass Punch Sect", 87, 66, 22, 140)
    };

    public override void _Ready()
    {
        // 1. Initialize Core Systems
        CaptureBuffer = new RetroactiveBuffer();
        MidiEngine = new NativeMidiEngine();
        HubServer = new StageHubServer(HttpPort);

        // 2. Wire Node References
        _lblDeviceStatus = GetNodeOrNull<Label>("HUD/TopBar/DevicePill/StatusLabel") ?? new Label();
        _lblIpAddress = GetNodeOrNull<Label>("HUD/TopBar/NetworkPill/IpLabel") ?? new Label();
        _lblSongTitle = GetNodeOrNull<Label>("HUD/CenterPanel/Teleprompter/SongTitle") ?? new Label();
        _lblCurrentChord = GetNodeOrNull<Label>("HUD/CenterPanel/Teleprompter/CurrentChord") ?? new Label();
        _lblNextChord = GetNodeOrNull<Label>("HUD/CenterPanel/Teleprompter/NextChord") ?? new Label();
        _lblBarBeat = GetNodeOrNull<Label>("HUD/CenterPanel/Teleprompter/BarBeatLabel") ?? new Label();
        _lblBpm = GetNodeOrNull<Label>("HUD/TopBar/BpmPill/BpmLabel") ?? new Label();
        _lblActivePatch = GetNodeOrNull<Label>("HUD/CenterPanel/Teleprompter/ActivePatch") ?? new Label();
        _btnPanic = GetNodeOrNull<Button>("HUD/TopBar/PanicButton") ?? new Button();
        _btnNeverLostMelody = GetNodeOrNull<Button>("HUD/BottomBar/NeverLostMelodyBtn") ?? new Button();
        _keybedRect = GetNodeOrNull<ColorRect>("HUD/BottomBar/KeybedContainer/KeybedRect") ?? new ColorRect();

        // 3. Setup Dynamic Keybed State Texture for Shader
        _keyStateImage = Image.CreateEmpty(64, 1, false, Image.Format.Rgba8);
        _keyStateTexture = ImageTexture.CreateFromImage(_keyStateImage);

        if (_keybedRect.Material is ShaderMaterial sm)
        {
            _keybedMaterial = sm;
            _keybedMaterial.SetShaderParameter("key_states", _keyStateTexture);
        }

        // 4. Hook Core Engine Events
        MidiEngine.OnMessageReceived += HandleIncomingMidi;
        MidiEngine.OnLog += msg => GD.Print(msg);
        MidiEngine.OnDeviceConnected += dev => CallDeferred(nameof(UpdateDeviceLabel), dev, true);
        MidiEngine.OnDeviceDisconnected += () => CallDeferred(nameof(UpdateDeviceLabel), "Disconnected", false);

        HubServer.OnLog += msg => GD.Print(msg);
        HubServer.OnPanicTriggered += () => CallDeferred(nameof(OnPanicPressed));
        HubServer.OnSetlistItemTriggered += payload => CallDeferred(nameof(ApplyRemoteSetlistItem), payload.SongIndex);

        _btnPanic.Pressed += OnPanicPressed;
        _btnNeverLostMelody.Pressed += OnNeverLostMelodyPressed;

        // 5. Connect Hardware & Start Mini-Server
        MidiEngine.Connect("Go:Keys");
        HubServer.Start();

        // Display local pairing IPs
        var ips = StageHubServer.GetLocalIpAddresses();
        string displayIp = ips.Count > 0 ? $"{ips[0]}:{HttpPort}" : $"127.0.0.1:{HttpPort}";
        _lblIpAddress.Text = $"HUB: http://{displayIp}";

        // Apply first setlist item
        SelectSong(0);
    }

    private void UpdateDeviceLabel(string name, bool connected)
    {
        _lblDeviceStatus.Text = connected ? $"ROLAND [{name}]" : "NO HARDWARE";
        _lblDeviceStatus.Modulate = connected ? new Color(0.0f, 0.96f, 0.83f) : new Color(0.93f, 0.14f, 0.23f);
    }

    private void HandleIncomingMidi(in MidiRawMessage msg)
    {
        // 1. Push directly into retroactive lock-free circular RAM buffer
        CaptureBuffer.Push(in msg);

        // 2. Update visual key state for 61-key range (MIDI 36 = C2 to 96 = C7)
        if (msg.IsNoteOn)
        {
            int keyIndex = msg.Data1 - MidiConstants.LowestNote;
            if (keyIndex >= 0 && keyIndex < 61)
            {
                float normalizedVel = msg.Data2 / 127.0f;
                _keyVelocities[keyIndex] = normalizedVel;
                _keyImpacts[keyIndex] = 1.0f; // Attack flash
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

    public override void _Process(double delta)
    {
        // 1. Visual keybed decay and texture update
        bool textureNeedsUpdate = false;
        for (int i = 0; i < 61; i++)
        {
            // Decay attack impact flash
            if (_keyImpacts[i] > 0.0f)
            {
                _keyImpacts[i] = Mathf.Max(0.0f, _keyImpacts[i] - (float)delta * 12.0f);
                textureNeedsUpdate = true;
            }

            int pixelOffset = i * 4;
            byte velByte = (byte)(_keyVelocities[i] * 255);
            byte handByte = (byte)(i < 24 ? 0 : 255); // Split point: Bass/Left hand = Amber (0), Right = Cyan (255)
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

        // 2. Metronome & Telemetry Clock Simulation
        _beatTimer += delta;
        if (_beatTimer >= _secondsPerBeat)
        {
            _beatTimer -= _secondsPerBeat;
            AdvanceBeat();
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

        _lblBarBeat.Text = $"BAR {CurrentBarFormatted()} . BEAT {_currentBeat}";

        // Broadcast telemetry to connected mobile teleprompters
        var currentSong = _setlist[_currentSongIndex];
        var tickData = new PlaybackTickData
        {
            SongId = $"song_{_currentSongIndex + 1:D3}",
            Bar = _currentBar,
            Beat = _currentBeat,
            Bpm = _currentBpm,
            CurrentChord = currentSong.CurrentChord,
            NextChord = currentSong.NextChord,
            ActivePatch = currentSong.PatchName
        };

        _ = HubServer.BroadcastPlaybackTickAsync(tickData);
    }

    private string CurrentBarFormatted() => _currentBar.ToString("D3");

    public void SelectSong(int index)
    {
        if (index < 0 || index >= _setlist.Count) return;
        _currentSongIndex = index;
        var song = _setlist[index];

        _currentBpm = song.Bpm;
        _secondsPerBeat = 60.0 / _currentBpm;
        _currentBar = 1;
        _currentBeat = 1;

        _lblSongTitle.Text = song.Title;
        _lblCurrentChord.Text = song.CurrentChord;
        _lblNextChord.Text = $"NEXT: {song.NextChord}";
        _lblBpm.Text = $"{_currentBpm} BPM";
        _lblActivePatch.Text = $"PATCH: {song.PatchName} (MSB:{song.BankMsb} LSB:{song.BankLsb} PC:{song.Pc})";

        // Dispatch hardware-safe volatile patch change to Roland Go:Keys 3
        MidiEngine.SelectPatchVolatile(channel: 0, song.BankMsb, song.BankLsb, song.Pc);
    }

    private void ApplyRemoteSetlistItem(int songIndex)
    {
        SelectSong(songIndex);
    }

    public void OnPanicPressed()
    {
        // 1. Immediately trigger hardware panic reset
        MidiEngine.TriggerPanic();

        // 2. Clear visual key states
        Array.Clear(_keyVelocities, 0, _keyVelocities.Length);
        Array.Clear(_keyImpacts, 0, _keyImpacts.Length);
        Array.Clear(_keyStatePixels, 0, _keyStatePixels.Length);
        _keyStateImage.SetData(64, 1, false, Image.Format.Rgba8, _keyStatePixels);
        _keyStateTexture.Update(_keyStateImage);

        GD.PrintRich("[color=red][b]GLOBAL PANIC EXECUTED - ALL CHANNELS SILENCED[/b][/color]");
    }

    public void OnNeverLostMelodyPressed()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string exportDir = Path.Combine(Directory.GetCurrentDirectory(), "captures");
        string filePath = Path.Combine(exportDir, $"AnantaNada_Capture_{timestamp}.mid");

        bool success = CaptureBuffer.ExportToMidi(filePath, RetroactiveBuffer.FiveMinutesUs, _currentBpm);
        if (success)
        {
            GD.PrintRich($"[color=green][b]Retroactive Scratchpad Exported:[/b] {filePath}[/color]");
        }
        else
        {
            GD.PrintRich("[color=yellow]No events captured in the buffer yet.[/color]");
        }
    }

    public override void _ExitTree()
    {
        HubServer.Dispose();
        MidiEngine.Dispose();
        base._ExitTree();
    }
}
