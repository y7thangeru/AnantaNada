# ANANTANADA: Hybrid Workstation & Stage Console
### Dedicated Performance Engine for Roland Go:Keys 3 (61-Key Synthesizer/Arranger)

> **Design Paradigm:** Game-Grade Diegetic Stage Console (Persona 5 / Arknights / Hi-Fi Rush)  
> **Host Runtime:** Godot Engine 4.x (.NET 8 / C# 12)  
> **Companion Client:** Embedded Local HTTP/WebSocket Telemetry Server + HTML5/Canvas Remote  

---

## 🌟 Architecture & Core Guarantees

1. **Hardware-Safe Low-Level MIDI Engine (`NativeMidiEngine.cs`)**:
   - Zero-allocation hot path interfacing directly with OS Multimedia drivers (`winmm.dll`).
   - **Zero Non-Volatile Flash Writes**: Tone navigation executed exclusively via in-RAM volatile messaging: Bank Select MSB (`CC 00`), Bank Select LSB (`CC 32`), and Program Change (`PC`). Keyboard internal EEPROM / ROM memory is strictly untouched.
   - **Throttled SysEx FIFO Queue**: Strict 20ms debounce limiter preventing Roland Go:Keys 3 processor buffer overflows.
   - **Global Panic Circuit (Failsafe Reset)**: Immediate emergency broadcast of `CC 120` (All Sound Off), `CC 123` (All Notes Off), and `CC 64` (Sustain Off = 0) across all 16 MIDI channels.

2. **Retroactive 5-Minute RAM Scratchpad (`RetroactiveBuffer.cs`)**:
   - Lock-free circular ring buffer (262,144 events, ~4MB RAM) continuously capturing all incoming MIDI events with microsecond timestamps.
   - "Never-Lost Melody" 1-Click Exporter: Instantly serializes the last 5 minutes of jamming into a valid Standard MIDI File (`.mid`, SMF Type 0) with automatic resolution of dangling sustained notes and variable-length delta times.

3. **Embedded StageHub Mini-Server & Telemetry (`StageHubServer.cs`)**:
   - Zero-dependency embedded `HttpListener` and `WebSocket` server inside Godot .NET.
   - Serves high-contrast stage teleprompter (Canvas2D) and touch setlist controller over local Wi-Fi / Hotspot to stand-mounted tablets/phones.
   - Real-time bidirectional telemetry (`PLAYBACK_TICK`, `TRIGGER_SETLIST_ITEM`, `TRIGGER_PANIC`).

4. **Diegetic Stylized UI/UX & Custom Shaders**:
   - Visual inspiration from Persona 5, Arknights, and Hi-Fi Rush.
   - 61-Key Interactive Keybed (`piano_keybed.gdshader`): Velocity-mapped dynamic neon glow (Electric Amber for Left Hand / Cyber Cyan for Right Hand), attack strike flashes, and chromatic bevels.
   - Vertical Waterfall Visualizer (`waterfall_visualizer.gdshader`): Beat-grid synchronized falling note bars flowing directly into the physical strike line.

---

## 📁 Project Directory Layout

```text
AnantaNada_RolandGoKeys3/
├── scenes/
│   └── MainStageView.tscn              # Godot 4 main stage scene & stylized node hierarchy
├── src/
│   ├── Core/
│   │   └── Midi/
│   │       ├── MidiConstants.cs        # Roland Go:Keys 3 CC numbers, status bytes & safety timings
│   │       ├── NativeMidiEngine.cs     # Zero-allocation WinMM interop, SysEx FIFO & Panic circuit
│   │       └── RetroactiveBuffer.cs    # Lock-free 5-min RAM ring buffer & SMF Type 0 exporter
│   ├── Network/
│   │   ├── StageHubServer.cs           # Embedded HTTP & WebSocket server for stage remote
│   │   └── TelemetryContracts.cs       # Strong-typed JSON contracts (PLAYBACK_TICK, etc.)
│   ├── Shaders/
│   │   ├── piano_keybed.gdshader       # 61-Key procedural keybed shader with velocity glow
│   │   └── waterfall_visualizer.gdshader # High-energy kinetic falling notes shader
│   ├── Tests/
│   │   └── VerificationHarness.cs      # Self-auditing unit verification suite
│   ├── UI/
│   │   └── MainStageController.cs      # Main Godot UI controller node connecting engine to scene
│   └── Program.cs                      # Host bootstrap entry point
├── web/
│   ├── index.html                      # Mobile teleprompter & setlist HUD
│   ├── style.css                       # Stylized high-contrast dark theme
│   └── app.js                          # WebSocket client & teleprompter canvas renderer
├── captures/                           # Output directory for retroactive .mid exports
├── project.godot                       # Godot 4.3 project configuration
├── AnantaNada.csproj                   # .NET 8 / C# 12 project file with GodotSharp integration
├── ANANTANADA_SPEC.md                  # Comprehensive architectural specification
├── Feature-ToBe-Implemented.md         # Single source of truth for feature tracking
├── update_tracker.md                   # Audit log of all file changes and rationale
└── Report-To-GeminiProject.md          # Consolidated master report for Gemini project threads
```

---

## 🚀 Quickstart & Verification

1. **Verify C# Engine & Audio/MIDI Subsystems**:
   ```powershell
   dotnet run
   ```
   Executes the built-in `VerificationHarness` validating:
   - Retroactive circular RAM buffer write/read integrity (1,000 events)
   - SMF Type 0 binary chunk header (`MThd`) and track chunk (`MTrk`) compliance
   - Hardware-safe volatile tone switching (zero flash writes)
   - 16-channel Panic Circuit broadcast

2. **Launch with Godot 4 Engine**:
   - Open Godot 4.3+ (.NET edition).
   - Import `project.godot`.
   - Run `res://scenes/MainStageView.tscn` (or press F5).

3. **Connect Mobile Stage Remote**:
   - Open a browser on any mobile phone or tablet connected to the same Wi-Fi / Hotspot.
   - Navigate to `http://<HOST_IP>:8080` (e.g. `http://192.168.1.15:8080`).
   - Real-time teleprompter, setlist switcher, and emergency panic button are instantly active.
