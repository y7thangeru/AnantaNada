# ANANTANADA: Hybrid Workstation & Stage Console
### Dedicated Performance Engine & Stylized Diegetic Stage Console for Roland Go:Keys 3 (61-Key Synthesizer/Arranger)

[![Runtime: Godot 4.x (.NET 8 / C# 12)](https://img.shields.io/badge/Runtime-Godot_4.3_.NET8_/_C%2312-478cbf?logo=godotengine&logoColor=white)](https://godotengine.org)
[![Target: Roland Go:Keys 3](https://img.shields.io/badge/Target-Roland_Go%3AKeys_3-ffb703?logo=roland&logoColor=black)](https://www.roland.com/global/products/gokeys_3/)
[![Architecture: Hardware--Safe](https://img.shields.io/badge/Architecture-Hardware--Safe_(Zero_Flash_Writes)-00f5d4)](#-hardware-safety-principles)
[![StageHub: Embedded HTTP/WS](https://img.shields.io/badge/StageHub-Embedded_HTTP_%26_WebSocket-ef233c)](#-embedded-stagehub-telemetry-server)
[![User Guide: Complete](https://img.shields.io/badge/User_Guide-Complete_SOP-success)](USER_GUIDE.md)

> **Design Paradigm:** Game-Grade Diegetic Stage Console (*Persona 5* / *Arknights* / *Hi-Fi Rush* / *Armored Core*)  
> **Host Runtime:** Godot Engine 4.3 (.NET 8 / C# 12)  
> **Companion Client:** Embedded Local HTTP & WebSocket Server + HTML5 / Canvas2D Remote  
> **Remote Repository:** [https://github.com/y7thangeru/AnantaNada](https://github.com/y7thangeru/AnantaNada)  
> **Buku Panduan Operasional Lengkap:** [USER_GUIDE.md](USER_GUIDE.md)

---

## 🎨 Visual Stage Showcase (Latest UI/UX Cockpit)

### 1. Main Stage Cockpit & Stand-Mounted Mobile Remote
| Godot 4 Native Stage Cockpit (1080p Forward+) | Stand-Mounted Mobile Remote (Canvas2D) |
| :---: | :---: |
| ![Godot Stage HUD](captures/screenshot_godot_stage.png) | <img src="captures/screenshot_web_remote.png" width="340" alt="Mobile Remote HUD" /> |

---

### 2. Detailed UI/UX Module Breakdown

| Modul Konsol | Tampilan Visual (UI/UX Capture) | Penjelasan Fitur Panggung |
| :--- | :---: | :--- |
| **Tactical Setlist Deck (100 Songs)**<br>*Arknights & Persona 5 Inspired* | ![Setlist Deck](captures/ui_setlist_deck.png) | • **100 Canonical Anthems**: 50 lagu Pop/Rock Indonesia & 50 lagu Pop/Rock Barat siap pentas.<br>• **Filter Kategori**: `[ ALL (100) ]`, `[ INDO (50) ]`, `[ WEST (50) ]`.<br>• **Pencarian Real-Time**: Pencarian instan judul/artis.<br>• **One-Touch Patch Dispatch**: Klik kartu misi langsung mengubah sound Roland Go:Keys 3 di Channel 4 tanpa menyentuh EEPROM. |
| **Cyber-Cascade Waterfall & Timing Coach**<br>*Hi-Fi Rush & Rhythm Action Inspired* | ![Waterfall & Coach](captures/ui_waterfall_coach.png) | • **18° 3D Trapezoidal Runway**: Jalur not jatuh 61-kolom dengan aksen grid futuristik.<br>• **Focused Hit Beam Bloom**: Sinar benturan bercahaya lembut di garis kontak.<br>• **Rhythm Timing Coach**: Evaluasi ketukan real-time (`PERFECT!`, `GREAT!`, `RUSHING`, `DRAGGING`) & Dynamic Strike Gauge Rank S/A/B. |
| **Tactical Chord HUD & Lyric Prompter**<br>*High-Contrast Stage Teleprompter* | ![Tactical Chord HUD](captures/ui_chord_hud.png) | • **Current Harmony (72pt)**: Akord aktif super besar, terbaca jelas dari jarak 2 meter.<br>• **Next Chord Transition (32pt)**: Indikator Cyan untuk antisipasi perpindahan jari.<br>• **Transpose Indicator**: Tampilan status transpose hardware via RPN.<br>• **Bar-Synced Lyric Prompter**: Lirik lagu bergerak sinkron dengan ketukan birama lagu. |
| **Synth-Runner 61-Key Keybed**<br>*Procedural Shader with Split Point C4* | ![61-Key Keybed](captures/ui_keybed_61.png) | • **36 Tuts Putih Gading & 25 Tuts Hitam 3D**: Rendered secara prosedural di GPU.<br>• **Split Point C4 Divider**: Garis merah tajam memisahkan zona tangan kiri dan kanan.<br>• **Dual Neon Glow**: Zone A (Amber `#ffb703`) untuk Bass/Chord; Zone B (Cyan `#00f5d4`) untuk Lead/Solo.<br>• **Tombol "⚡ AMBIL IDE TADI"**: Ekspor 5 menit rekaman circular RAM ke file `.mid`. |

---

## 🌟 Architecture & Core Guarantees

```mermaid
flowchart TD
    subgraph Hardware["Roland Go:Keys 3 Synthesizer"]
        Keybed["61-Key Dynamic Keybed (C2-C7, MIDI 36-96)"]
        ZenCore["ZEN-Core Sound Engine"]
        RAM["Volatile Patch Memory"]
        Flash["Non-Volatile Flash / EEPROM (PROTECTED)"]
    end

    subgraph DesktopHost["AnantaNada Workstation (Godot 4.3 .NET 8 / C# 12)"]
        subgraph MidiLayer["Hardware-Safe Low-Level MIDI Layer"]
            WinMM["Native WinMM Driver Interop (winmm.dll)"]
            WinRT["Modern Windows Bluetooth LE MIDI (Windows.Devices.Midi)"]
            Filter["System Realtime Message Filter"]
            PanicCircuit["16-Channel Panic Circuit (CC 120/123/64)"]
            SysExFIFO["Throttled SysEx FIFO Queue (20ms Debounce)"]
        end

        subgraph MemoryLayer["Retroactive Memory Layer"]
            RingBuf["Never-Lost Melody Scratchpad (262,144 Slots, ~4MB RAM)"]
            SMF["SMF Type 0 Binary Exporter (.mid)"]
        end

        subgraph VisualLayer["Diegetic Visualizer Engine"]
            KeyShader["61-Key Procedural Keybed Shader (Amber/Cyan Glow)"]
            WaterfallShader["Kinetic Waterfall Note Stream Shader"]
            TacticalShader["Tactical Chamfered Panel Shader"]
            StageHUD["3-Column Cockpit Konsol Panggung"]
        end

        subgraph ServerLayer["Embedded StageHub Mini-Server"]
            HttpServer["Zero-Dependency Embedded HTTP Server (:8080)"]
            WsServer["WebSocket Bidirectional State Bus (/ws)"]
        end
    end

    subgraph MobileCompanion["Stand-Mounted Mobile Remote (Tablet / Phone)"]
        Teleprompter["High-Contrast Canvas2D Teleprompter"]
        Setlist["Giant Touch-Slot Setlist Selector"]
        PanicMobile["Always-Visible Stage Panic Button"]
    end

    Keybed -->|USB-MIDI / BLE MIDI| WinMM
    Keybed -->|Bluetooth LE| WinRT
    WinMM --> Filter --> RingBuf
    WinRT --> Filter --> RingBuf
    RingBuf -->|1-Click Dump| SMF
    Filter --> KeyShader
    Filter --> WaterfallShader

    DesktopHost -->|Volatile MSB/LSB/PC Ch 4| RAM
    SysExFIFO -.->|BLOCKS FLASH WRITES| Flash

    WsServer <-->|PLAYBACK_TICK / TRIGGER_SETLIST| MobileCompanion
    PanicMobile -->|TRIGGER_PANIC| PanicCircuit --> WinMM --> ZenCore
```

---

## 🛡️ Hardware Safety Principles (Roland Go:Keys 3)

1. **Zero Non-Volatile Flash Writes (`SelectPatchVolatile`)**:
   - Tone navigation executed exclusively via volatile standard MIDI messaging:
     - Bank Select MSB (`CC 00`)
     - Bank Select LSB (`CC 32`)
     - Program Change (`PC 0-127`)
   - Internal keyboard EEPROM / system flash memory is strictly untouched, guaranteeing zero flash endurance wear during intensive live performances.

2. **Throttled SysEx FIFO Queue (`MidiConstants.SysExDebounceMs = 20ms`)**:
   - Outbound SysEx dumps are paced by a dedicated background worker thread with a strict 20ms debounce limiter, preventing processor buffer overflows on the Roland instrument.

3. **Global Panic Failsafe Circuit**:
   - Immediate emergency broadcast of `CC 120` (All Sound Off), `CC 123` (All Notes Off), and `CC 64` (Sustain Off = 0) across all 16 MIDI channels.
   - Clears hanging or stuck notes within 1 millisecond if USB cables are accidentally disconnected or Bluetooth LE MIDI suffers packet dropouts.

4. **Zero-Allocation Hot Path**:
   - Direct P/Invoke integration with `winmm.dll` and `Windows.Devices.Midi` using pinned delegate callbacks and value-type `MidiRawMessage` structs. Zero heap allocations during live performance.

---

## 🧠 "Never-Lost Melody" Retroactive Scratchpad

- Continuously buffers all incoming MIDI events in a lock-free circular ring buffer (262,144 event slots, consuming ~4 MB of RAM).
- Stores up to 5 minutes of continuous playing history with microsecond-accurate hardware timestamps.
- **1-Click Export**: Tapping *"⚡ AMBIL IDE TADI"* immediately serializes the buffer into a valid Standard MIDI File (`.mid`, SMF Type 0) with automatic resolution of hanging notes and variable-length quantity (VLQ) delta times.

---

## 📱 Embedded StageHub & Teleprompter

- Built-in zero-dependency HTTP and WebSocket server running inside the Godot .NET host on port `8080`.
- Connect any stand-mounted smartphone or tablet over local Wi-Fi or mobile hotspot.
- Displays auto-scrolling chord charts, metronome pulses, current/next chord lookahead, giant touch setlist slots, and an emergency panic button.

---

## 📖 Cara Menggunakan Project & Aplikasi (Quick Operational Guide)

> Untuk panduan langkah-demi-langkah mendalam dan SOP panggung, buka [USER_GUIDE.md](USER_GUIDE.md).

### 1. Persiapan Koneksi Hardware
1. Hubungkan port **USB Computer** di bagian belakang Roland Go:Keys 3 ke port USB laptop Anda menggunakan kabel USB Type-B ke USB-A / USB-C (atau aktifkan Bluetooth LE MIDI).
2. Nyalakan Roland Go:Keys 3. Driver Windows (`winmm.dll` / WinRT) akan mendeteksinya secara otomatis.
3. Di menu keyboard: pastikan `MIDI Rx/Tx Channel` aktif di Channel 1 atau Channel 4, dan `Program Change Tx/Rx` di-set **ON**.

### 2. Menjalankan Workstation di Komputer
- **Mode A: Verifikasi C# Subsystem (CLI)**:
  ```powershell
  dotnet run
  ```
  Menjalankan tes otomatis untuk buffer RAM, eksportir SMF Type 0, katalog tone Jalur C, dan 100-song repertoire dispatch.
- **Mode B: Menjalankan Konsol Panggung (Godot Engine)**:
  1. Buka **Godot Engine 4.3+ (.NET edition)**.
  2. Klik **Import** -> Pilih `project.godot`.
  3. Tekan **F5** (atau klik Run). Layar HUD panggung 1080p akan langsung menyala!

### 3. Menggunakan Konsol Utama (Desktop Stage Console)
- **Status Koneksi**: Indikator *Device Pill* di bar atas akan otomatis menampilkan nama perangkat Roland yang tersambung.
- **Memilih Lagu Repertoar (100 Lagu)**:
  - Gunakan filter `[ ALL (100) ]`, `[ INDO (50) ]`, atau `[ WEST (50) ]`.
  - Klik kartu lagu apa saja untuk mengubah sound keyboard di Channel 4 dan menyinkronkan akord serta lirik.
- **Visual Keybed 61-Tuts**: Tuts yang ditekan akan memancarkan cahaya neon GPU secara instan:
  - Tangan Kiri (Bass / Akord, tuts 1–24): **Electric Amber** (`#ffb703`).
  - Tangan Kanan (Melodi / Lead, tuts 25–61): **Cyber Cyan** (`#00f5d4`).
- **Ekspor Melodi Instan**: Klik tombol **"⚡ AMBIL IDE TADI (5-MIN RETROACTIVE)"** di bar bawah kapan saja untuk menyimpan 5 menit permainan terakhir Anda ke file MIDI `captures/jam_YYYYMMDD_HHmmss.mid`.
- **Tombol Darurat (Panic)**: Klik tombol merah **[ ! PANIC / PURGE ! ]** jika terjadi nada macet karena kabel tersenggol.

### 4. Menghubungkan HP / Tablet ke StageHub Mobile Remote
1. Sambungkan HP atau tablet ke jaringan Wi-Fi yang sama dengan laptop (atau nyalakan **Mobile Hotspot** di laptop Anda).
2. Buka browser (Chrome / Safari) di HP/tablet, lalu ketik alamat yang tertera di layar panggung, misalnya:
   ```text
   http://192.168.1.15:8080
   ```
3. Layar teleprompter hitam OLED akan terbuka seketika:
   - **Akord Raksasa**: Menampilkan akord saat ini (72pt) dan akord antisipasi berikutnya (32pt) dari jarak 2 meter.
   - **Setlist Sentuh**: Tap nama lagu di HP untuk langsung mengganti sound Roland Go:Keys 3 dan tempo metronom di laptop.
   - **Panic HP**: Tombol merah di bagian bawah HP untuk membisukan instrumen dari stand mikrofon.

---

## 📁 Multi-Tiered Directory Architecture

```text
AnantaNada_RolandGoKeys3/
├── addons/
│   └── godot_mcp/                      # Godlink WebSocket Editor Bridge (Port 6505)
├── scenes/
│   ├── MainStageView.tscn              # Godot 4 main stage scene & stylized node hierarchy
│   └── README.md                       # Documentation for scene hierarchy and HUD layout
├── scripts/
│   ├── capture_stage_screenshot.gd     # Headless 1080p stage HUD capture tool
│   ├── capture_web_remote.py           # Web remote companion client capture utility
│   ├── check_uwp_midi.ps1              # Windows BLE MIDI discovery diagnostic tool
│   └── create_desktop_shortcut.ps1     # 1-click desktop shortcut creator
├── setlists/
│   ├── default_band_setlist.json       # Baseline 4-mission stage setlist
│   └── master_100_repertoire.json      # Complete 100-song tactical stage repertoire database
├── src/
│   ├── Core/
│   │   ├── Data/
│   │   │   ├── gokeys3_tones.json      # Structured Roland Go:Keys 3 tone bank catalog (26 presets)
│   │   │   └── RepertoireDatabaseBuilder.cs # Static builder for 100 canonical band anthems
│   │   ├── Midi/
│   │   │   ├── MidiConstants.cs        # Roland Go:Keys 3 CC numbers, status bytes & safety timings
│   │   │   ├── NativeMidiEngine.cs     # Zero-allocation WinMM/WinRT BLE interop & Panic circuit
│   │   │   ├── RetroactiveBuffer.cs    # Lock-free 5-min RAM ring buffer & SMF Type 0 exporter
│   │   │   └── README.md               # Documentation for Core MIDI math & native drivers
│   │   ├── Models/
│   │   │   └── SetlistModel.cs         # Strong-typed schemas for setlists, missions, and tone presets
│   │   └── Services/
│   │       └── ToneService.cs          # O(1) tone catalog lookup and 100-song query engine
│   ├── Network/
│   │   ├── StageHubServer.cs           # Embedded HTTP & WebSocket server for stage remote
│   │   ├── TelemetryContracts.cs       # Strong-typed JSON contracts (PLAYBACK_TICK, etc.)
│   │   └── README.md                   # Documentation for network topology & data contracts
│   ├── Shaders/
│   │   ├── piano_keybed.gdshader       # 61-Key procedural keybed shader with velocity glow
│   │   ├── waterfall_visualizer.gdshader # High-energy kinetic falling notes shader
│   │   ├── tactical_panel.gdshader     # Arknights chamfered panel & emergency glitch shader
│   │   └── README.md                   # Documentation for GPU shader mathematics & uniforms
│   ├── UI/
│   │   ├── MainStageController.cs      # Main C# UI controller linking engine to Godot scene
│   │   ├── MainStageController.gd      # Native GDScript bridge controller for Godot 4.5
│   │   └── README.md                   # Documentation for controller lifecycle & state textures
│   ├── Tests/
│   │   └── VerificationHarness.cs      # Self-auditing unit verification suite (7/7 tests passing)
│   └── Program.cs                      # Host bootstrap entry point & interactive stage CLI
├── web/
│   ├── index.html                      # Mobile teleprompter & setlist HUD
│   ├── style.css                       # Stylized high-contrast dark theme
│   ├── app.js                          # WebSocket client & teleprompter canvas renderer
│   └── README.md                       # Documentation for mobile remote client
├── captures/                           # Output directory for retroactive .mid exports and screenshots
│   ├── screenshot_godot_stage.png      # 1080p visual capture of native stage console
│   ├── screenshot_web_remote.png       # Visual capture of mobile stage teleprompter HUD
│   ├── ui_setlist_deck.png             # Tactical Setlist Deck (100 Songs) component capture
│   ├── ui_waterfall_coach.png          # Cyber-Cascade Waterfall & Timing Coach capture
│   ├── ui_chord_hud.png                # Tactical Chord HUD & Lyric Prompter capture
│   └── ui_keybed_61.png                # Synth-Runner 61-Key Keybed component capture
├── project.godot                       # Godot 4.3 project configuration
├── AnantaNada.csproj                   # .NET 8 / C# 12 project file with GodotSharp integration
├── ANANTANADA_SPEC.md                  # Comprehensive architectural specification
├── USER_GUIDE.md                       # Buku panduan penggunaan lengkap & SOP panggung
├── Feature-ToBe-Implemented.md         # Single source of truth for feature tracking
├── update_tracker.md                   # Audit log of all file changes and rationale
└── Report-To-GeminiProject.md          # Consolidated master report for Gemini project threads
```

---

## 🚀 Quickstart & Verification Commands

```powershell
# 1. Kompilasi & Uji Mandiri Engine C# (7/7 Tests)
dotnet run

# 2. Buat Shortcut Desktop Sekali Klik
powershell -ExecutionPolicy Bypass -File scripts/create_desktop_shortcut.ps1

# 3. Jalankan Remote Companion di Browser
python scripts/capture_web_remote.py
```
