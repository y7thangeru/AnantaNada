using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using AnantaNada.Core.Audio;
using AnantaNada.Core.Data;
using AnantaNada.Core.Harmony;
using AnantaNada.Core.Midi;
using AnantaNada.Core.Motion;
using AnantaNada.Core.Performance;
using AnantaNada.Core.Sequencer;
using AnantaNada.Core.Services;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Autonomous in-scene C# Bridge for Godot Engine.
/// Instantiates and runs the embedded StageHub HTTP/WebSocket server (port 8080)
/// and hardware-safe NativeMidiEngine directly inside the Godot runtime, alongside
/// the full Ohpe-inspired performance, sequencer, harmony, motion, and spatial audio suite.
/// </summary>
public partial class StageEngineBridge : Node
{
    public static StageEngineBridge? Instance { get; private set; }

    [Export] public int HttpPort { get; set; } = 8080;

    public StageHubServer? HubServer { get; private set; }
    public NativeMidiEngine? MidiEngine { get; private set; }
    public RetroactiveBuffer? CaptureBuffer { get; private set; }
    public ToneService? ToneLibrary { get; private set; }

    // Ohpe-Inspired Master Engine Suite
    public PerformancePadsEngine? PerformancePads { get; private set; }
    public SequencerEngine? Sequencer { get; private set; }
    public HarmonyBrain? Harmony { get; private set; }
    public MotionRecorder? Motion { get; private set; }
    public SpatialAudioMatrix? SpatialMatrix { get; private set; }
    public SoundGenomeBreeder? PresetBreeder { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        GD.PrintRich("[color=green][b][StageEngineBridge] Booting In-Process C# MIDI, StageHub & Ohpe Suite...[/b][/color]");

        try
        {
            CaptureBuffer = new RetroactiveBuffer();
            MidiEngine = new NativeMidiEngine();
            ToneLibrary = new ToneService();

            // Instantiate Ohpe Suite
            PerformancePads = new PerformancePadsEngine();
            Sequencer = new SequencerEngine();
            Harmony = new HarmonyBrain();
            Motion = new MotionRecorder();
            SpatialMatrix = new SpatialAudioMatrix();
            PresetBreeder = new SoundGenomeBreeder();

            // Wire Performance Pads to MIDI Hardware
            PerformancePads.OnPitchBend += (ch, bend) => MidiEngine?.SendPitchBend(ch, bend);
            PerformancePads.OnControlChange += (ch, cc, val) => MidiEngine?.SendControlChange(ch, cc, val);
            PerformancePads.OnNoteTriggered += (ch, note, vel) => MidiEngine?.SendNoteOn(ch, note, vel);

            // Wire Sequencer to MIDI Hardware & Hub Broadcast
            Sequencer.OnNoteTriggered += (ch, note, vel) => MidiEngine?.SendNoteOn(ch, note, vel);
            Sequencer.OnStepAdvanced += (ring, step, total) => _ = HubServer?.BroadcastEuclideanTickAsync(ring, step, total);

            // Wire Harmony Brain
            Harmony.OnChordDetected += chord => _ = HubServer?.BroadcastHarmonyUpdateAsync(chord.Name, chord.Root, chord.Quality, chord.Bass, Harmony.GravityMode.ToString());

            // Wire Motion Recorder to MIDI Hardware
            Motion.OnControlChange += (ch, cc, val) => MidiEngine?.SendControlChange(ch, cc, val);
            Motion.OnPitchBend += (ch, bend) => MidiEngine?.SendPitchBend(ch, bend);

            string webRoot = Path.Combine(Directory.GetCurrentDirectory(), "web");
            if (!Directory.Exists(webRoot))
            {
                webRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web");
            }

            HubServer = new StageHubServer(HttpPort, webRoot);
            bool started = HubServer.Start();
            GD.PrintRich($"[color=cyan][StageHub Server][/color] Online on port {HttpPort} (Success: {started}). WebRoot: {webRoot}");

            // Wire Remote Server Events
            HubServer.OnPanicTriggered += () =>
            {
                PerformancePads?.ResetAll();
                Motion?.ClearAll();
                MidiEngine?.TriggerPanic();
                GD.PrintRich("[color=red][StageEngineBridge] PANIC Purged via Remote.[/color]");
            };

            HubServer.OnTransposeTriggered += semitones =>
            {
                MidiEngine?.SetCoarseTranspose(MidiConstants.GoKeys3DefaultPatchChannel, semitones);
                GD.PrintRich($"[color=yellow][StageEngineBridge] Remote Transpose Dispatched: {semitones:+#;-#;0}[/color]");
            };

            HubServer.OnExportMidiTriggered += () =>
            {
                if (CaptureBuffer != null)
                {
                    string fn = $"jam_{DateTime.Now:yyyyMMdd_HHmmss}.mid";
                    string capDir = Path.Combine(Directory.GetCurrentDirectory(), "captures");
                    Directory.CreateDirectory(capDir);
                    string fullPath = Path.Combine(capDir, fn);
                    bool ok = CaptureBuffer.ExportToMidi(fullPath, out int evts, out int bars);
                    if (ok)
                    {
                        GD.PrintRich($"[color=green][StageEngineBridge] Retro-buffer exported {evts} events to {fullPath}[/color]");
                        _ = HubServer.BroadcastCaptureToastAsync(fn, evts, bars);
                    }
                }
            };

            HubServer.OnSetlistItemTriggered += payload =>
            {
                if (MidiEngine != null)
                {
                    MidiEngine.SelectPatchVolatile(
                        MidiConstants.GoKeys3DefaultPatchChannel,
                        (byte)payload.BankMsb,
                        (byte)payload.BankLsb,
                        (byte)payload.ProgramChange
                    );
                    MidiEngine.SetCoarseTranspose(
                        MidiConstants.GoKeys3DefaultPatchChannel,
                        payload.Transpose
                    );
                    GD.PrintRich($"[color=cyan][StageEngineBridge] Switched patch via remote: MSB {payload.BankMsb} LSB {payload.BankLsb} PC {payload.ProgramChange}[/color]");
                }
            };

            // Ohpe WebSocket Handlers
            HubServer.OnPerformancePadTriggered += (padTypeStr, active, ch) =>
            {
                if (Enum.TryParse<PerformancePadType>(padTypeStr, true, out var padType))
                {
                    PerformancePads?.TriggerPad(padType, active, ch);
                    GD.PrintRich($"[color=magenta][PerformancePads] Remote Pad {padType}: {(active ? "ON" : "OFF")}[/color]");
                }
            };

            HubServer.OnEuclideanConfigTriggered += (ringIdx, steps, pulses, rot) =>
            {
                Sequencer?.UpdateRing(ringIdx, steps, pulses, rot);
                GD.PrintRich($"[color=cyan][Sequencer] Ring #{ringIdx} updated -> E({pulses},{steps}) rot:{rot}[/color]");
            };

            HubServer.OnKeyGravityTriggered += (modeStr, rootNote, scaleStr) =>
            {
                if (Enum.TryParse<KeyGravityMode>(modeStr, true, out var mode))
                {
                    if (Harmony != null)
                    {
                        Harmony.GravityMode = mode;
                        if (Enum.TryParse<MusicalScale>(scaleStr, true, out var scale))
                        {
                            Harmony.SetScale(rootNote, scale);
                        }
                    }
                    GD.PrintRich($"[color=yellow][HarmonyBrain] Key Gravity: {modeStr} (Root: {rootNote})[/color]");
                }
            };

            HubServer.OnMotionRecorderTriggered += (action, duration) =>
            {
                if (action.Equals("START", StringComparison.OrdinalIgnoreCase))
                {
                    Motion?.StartRecording(duration);
                }
                else if (action.Equals("STOP", StringComparison.OrdinalIgnoreCase))
                {
                    Motion?.StopRecording();
                }
                else if (action.Equals("CLEAR", StringComparison.OrdinalIgnoreCase))
                {
                    Motion?.ClearAll();
                }
            };

            // Connect to Roland Go:Keys 3 in background
            _ = ConnectMidiHardwareAsync();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[StageEngineBridge Init Error] {ex}");
        }
    }

    public override void _Process(double delta)
    {
        PerformancePads?.Tick(delta);
        Motion?.Tick(delta);
    }

    private async Task ConnectMidiHardwareAsync()
    {
        if (MidiEngine == null) return;
        try
        {
            await Task.Delay(100);
            await MidiEngine.ConnectAsync("Go:Keys");
            GD.PrintRich($"[color=green][StageEngineBridge] MIDI Hardware Connected: {MidiEngine.ConnectedDeviceName}[/color]");
            if (HubServer != null)
            {
                await HubServer.BroadcastDeviceStatusAsync(MidiEngine.ConnectedDeviceName, true);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[StageEngineBridge MIDI Connect Warning] {ex.Message}");
        }
    }

    public override void _ExitTree()
    {
        GD.Print("[StageEngineBridge] Shutting down C# background servers...");
        try
        {
            HubServer?.Dispose();
            MidiEngine?.Dispose();
        }
        catch { }
    }
}
