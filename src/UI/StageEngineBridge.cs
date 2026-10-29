using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using AnantaNada.Core.Midi;
using AnantaNada.Core.Services;
using AnantaNada.Network;

namespace AnantaNada.UI;

/// <summary>
/// Autonomous in-scene C# Bridge for Godot Engine.
/// Instantiates and runs the embedded StageHub HTTP/WebSocket server (port 8080)
/// and hardware-safe NativeMidiEngine directly inside the Godot runtime.
/// </summary>
public partial class StageEngineBridge : Node
{
    public static StageEngineBridge? Instance { get; private set; }

    [Export] public int HttpPort { get; set; } = 8080;

    public StageHubServer? HubServer { get; private set; }
    public NativeMidiEngine? MidiEngine { get; private set; }
    public RetroactiveBuffer? CaptureBuffer { get; private set; }
    public ToneService? ToneLibrary { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        GD.PrintRich("[color=green][b][StageEngineBridge] Booting In-Process C# MIDI & StageHub Server...[/b][/color]");

        try
        {
            CaptureBuffer = new RetroactiveBuffer();
            MidiEngine = new NativeMidiEngine();
            ToneLibrary = new ToneService();

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

            // Connect to Roland Go:Keys 3 in background
            _ = ConnectMidiHardwareAsync();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[StageEngineBridge Init Error] {ex}");
        }
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
