using System.Diagnostics;
using AnantaNada.Core.Midi;
using AnantaNada.Core.Services;
using AnantaNada.Network;
using AnantaNada.Tests;

namespace AnantaNada;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length > 0 && (args[0] == "--verify" || args[0] == "--test"))
        {
            Console.WriteLine("[ANANTANADA] Running Engine Verification Harness...");
            return await VerificationHarness.RunAll();
        }

        Console.Title = "ANANTANADA - Roland Go:Keys 3 Live Stage Engine";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
+===========================================================================+
|    ___   _  _   _   _  _ _____ _   _  _   ___   _                         |
|   / _ \ | \| | /_\ | \| |_   _/_\ | \| | /   \ /_\                        |
|  / /_\ \| .` |/ _ \| .` | | |/ _ \| .` || () |/ _ \                       |
| /_/   \_\_|\_/_/ \_\_|\_| |_/_/ \_\_|\_| \___//_/ \_\                      |
|                                                                           |
|   ANANTANADA HYBRID WORKSTATION // ROLAND GO:KEYS 3 STAGE ENGINE          |
|   Hardware-Safe Volatile MIDI Bus + Never-Lost 5m Buffer + WebSocket Hub  |
+===========================================================================+
");
        Console.ResetColor();

        using var midiEngine = new NativeMidiEngine();
        var buffer = new RetroactiveBuffer();
        var toneService = new ToneService();
        int httpPort = 8080;
        using var hubServer = new StageHubServer(httpPort);

        midiEngine.OnLog += msg =>
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[MIDI] {msg}");
            Console.ResetColor();
        };

        midiEngine.OnDeviceConnected += name =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n>> [DEVICE LINKED] {name.ToUpper()} (USB/Bluetooth MIDI Ready)");
            Console.ResetColor();
        };

        midiEngine.OnDeviceDisconnected += () =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n>> [DEVICE OFFLINE] Roland Go:Keys 3 Disconnected.");
            Console.ResetColor();
        };

        // Wire MIDI ingress to buffer and WebSocket broadcast
        midiEngine.OnMessageReceived += (in MidiRawMessage msg) =>
        {
            buffer.Push(in msg);
        };

        midiEngine.OnNoteOn += (channel, note, velocity) =>
        {
            _ = hubServer.BroadcastNoteOnAsync(channel, note, velocity);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\r[NOTE ON] Note:{note:D2} Vel:{velocity:D3} (Ch:{channel + 1:D2})   ");
            Console.ResetColor();
        };

        midiEngine.OnNoteOff += (channel, note, velocity) =>
        {
            _ = hubServer.BroadcastNoteOffAsync(channel, note);
        };

        // Wire incoming WebSocket commands from Godot & Mobile remotes
        hubServer.OnLog += msg =>
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"[HUB] {msg}");
            Console.ResetColor();
        };

        hubServer.OnSetlistItemTriggered += payload =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n>> [SETLIST TRIGGER] Mission #{payload.SongIndex} (PC:{payload.ProgramChange} MSB:{payload.BankMsb} LSB:{payload.BankLsb} Transpose:{payload.Transpose})");
            Console.ResetColor();

            midiEngine.SelectPatchVolatile(
                MidiConstants.GoKeys3DefaultPatchChannel,
                (byte)payload.BankMsb,
                (byte)payload.BankLsb,
                (byte)payload.ProgramChange
            );

            midiEngine.SetCoarseTranspose(
                MidiConstants.GoKeys3DefaultPatchChannel,
                payload.Transpose
            );
        };

        hubServer.OnPanicTriggered += () =>
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n>> [PANIC TRIGGERED] Purging all notes across 16 channels!");
            Console.ResetColor();
            midiEngine.TriggerPanic();
            _ = hubServer.BroadcastPanicAsync();
        };

        // Connect MIDI (Scan for Go:Keys or any USB/Bluetooth MIDI hardware)
        Console.WriteLine("[1/3] Scanning and connecting to MIDI Devices...");
        bool midiConnected = midiEngine.Connect("Go:Keys");
        if (!midiConnected)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(">> Note: Roland Go:Keys 3 not detected yet. Connect via Bluetooth/USB to auto-link.");
            Console.ResetColor();
        }

        // Start StageHub Mini-Server
        Console.WriteLine("[2/3] Starting StageHub HTTP/WebSocket Server...");
        hubServer.Start();
        var localIps = StageHubServer.GetLocalIpAddresses();
        string ipText = localIps.Count > 0 ? localIps[0] : "127.0.0.1";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($">> Web Remote URL: http://{ipText}:{httpPort}");
        Console.WriteLine($">> WebSocket Hub: ws://127.0.0.1:{httpPort}/ws");
        Console.ResetColor();

        // Status Summary
        Console.WriteLine($"[3/3] Tone Library Loaded: {toneService.AllSongs.Count} Songs in Master Repertoire.");
        Console.WriteLine("\n-------------------------------------------------------------");
        Console.WriteLine("HOTKEYS:");
        Console.WriteLine("  [P] Emergency Panic / Silence All Notes");
        Console.WriteLine("  [M] Export Recent 5-Minute Performance to Standard MIDI (.mid)");
        Console.WriteLine("  [R] Rescan & Reconnect MIDI Hardware");
        Console.WriteLine("  [Q] Exit Workstation");
        Console.WriteLine("-------------------------------------------------------------\n");

        // Interactive Console Loop
        bool running = true;
        while (running)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.P:
                        midiEngine.TriggerPanic();
                        await hubServer.BroadcastPanicAsync();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[HOTKEY] PANIC BROADCASTED (CC120/CC123/CC64 on all 16 Ch)");
                        Console.ResetColor();
                        break;

                    case ConsoleKey.M:
                        string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string outPath = Path.Combine(Directory.GetCurrentDirectory(), "captures", $"session_{stamp}.mid");
                        bool ok = buffer.ExportToMidi(outPath, out int count, out int bars);
                        if (ok)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"\n[HOTKEY] Exported {count} events ({bars} bars) -> {outPath}");
                            Console.ResetColor();
                        }
                        break;

                    case ConsoleKey.R:
                        Console.WriteLine("\n[HOTKEY] Rescanning MIDI ports...");
                        midiEngine.Connect("Go:Keys");
                        break;

                    case ConsoleKey.Q:
                        running = false;
                        break;
                }
            }

            await Task.Delay(50);
        }

        Console.WriteLine("\n[ANANTANADA] Shutting down gracefully...");
        return 0;
    }
}
