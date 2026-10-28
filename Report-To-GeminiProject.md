# CONSOLIDATED MASTER REPORT: ANANTANADA HYBRID WORKSTATION
### Dedicated Performance Engine & Stylized Diegetic Stage Console for Roland Go:Keys 3

**Target Instrument:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
**Host Runtime:** Godot Engine 4.x (.NET 8 / C# 12)  
**Companion Link:** Embedded Local Mini-Server (HTTP/WebSocket) + Mobile Stage Remote (Canvas2D)  
**Design Paradigm:** Stylized Diegetic Game HUD & Stage Performance Console (*Persona 5* / *Arknights* / *Hi-Fi Rush* / *Armored Core*)  
**Remote VCS:** [https://github.com/y7thangeru/AnantaNada](https://github.com/y7thangeru/AnantaNada)  
**Date of Audit:** October 28, 2026  

---

# PART I: EXECUTIVE & TECHNICAL PROJECT REPORT

## 1. Executive Summary & Design Blueprint
ANANTANADA adalah **Cockpit Konsol Panggung Game-Grade** yang menggabungkan:
1. **Persona 5 / Metaphor (Atlus)**:
   - Panel Setlist kartu miring (*slanted polygon cards* -4° s/d -8°).
   - Tipografi dinamis dan transisi sapuan cahaya saat memilih misi lagu.
2. **Arknights / Zenless Zone Zero (Tactical Techwear)**:
   - Frame batas bergaris tajam, titik desimal, aksen strip bahaya (*caution stripes*), dan sudut terpotong (*chamfered corners*).
   - Tombol **! EMERGENCY PURGE / PANIC !** ala sakelar militer dengan kedipan *red glitch flash*.
3. **Hi-Fi Rush / Muse Dash (Rhythm Action)**:
   - Evaluasi ketukan metronom real-time: `[ PERFECT! ]`, `[ GREAT! ]`, `[ RUSHING! ]`, atau `[ DRAGGING! ]` dengan persentase skor akurasi ritmis.
   - Denyut metronom (*beat pulse*) menyala di garis benturan (*hit beam*) dan tuts pada setiap downbeat birama.
4. **Armored Core VI / Cyberpunk 2077 (Diegetic HUD)**:
   - Pengukur daya pukulan tuts (*Dynamic Strike Gauge*) dengan tingkatan Rank: `RANK [S]` (110-127 `fff`), `RANK [A]`, `RANK [B]`, `RANK [C]`.
   - Efek guncangan layar halus (*screen shake*) pada pukulan berdaya tinggi (`velocity >= 100`).

---

## 2. Arsitektur 3 Kolom Cockpit Konsol Panggung

```text
+===================================================================================================+
| // SYS_STATUS: OPERATIONAL [ANANTANADA V1.0]                     [BPM: 128 /// 4/4] [BEAT: 03/16] |
| > LINK: ROLAND GO:KEYS 3 [ONLINE]   > MEMORY: IN-RAM SAFE   [ ! EMERGENCY PURGE / PANIC ! ]       |
+===================================================================================================+
| [SETLIST DECK]           | [CYBER-CASCADE: WATERFALL VISUALIZER]       | [CHORD TACTICAL HUD]     |
|                          |                                             |                          |
|  // ACTIVE MISSION       |     |  *  |     |     |  *  |     |  *  |   | // CURRENT HARMONY       |
|  /------------------\    |     |  |  |     |     |  |  |     |  |  |   | /----------------------\ |
|  | > 01. SENJA DUST |    |     |  v  |     |     |  v  |     |  v  |   | |      [ G#m7 ]        | |
|  \------------------/    |     |     |     |     |     |     |     |   | |  TRANSPOSE: [ +0 ]   | |
|                          |     v     v           v     v     v         | \----------------------/ |
|  // NEXT MISSIONS        |  -----------------------------------------  |                          |
|  /------------------\    |  ================ HIT BEAM ===============  | // INCOMING TRANSITION   |
|  |   02. KELANA BEAT|    |  (Neon Explosion FX / Velocity Sparkles)    | [ NEXT CHORD -> C#m7 ]   |
|  \------------------/    +---------------------------------------------+                          |
|  /------------------\    | [PERFORMANCE COMBO / TIMING COACH]          | // LYRIC TELEPROMPTER    |
|  |   03. FAJAR LEAD |    | >> RHYTHM ACCURACY: [ PERFECT! ]  98.4%     | "Kala fajar tiba,        |
|  \------------------/    | >> STRIKE GAUGE: [ |||||||||||||||| ] 118 ff| melangkah di tanah tua"|
|                          | >> RETRO-BUFFER: [ 5m CAPTURE ARMED ]       |                          |
+--------------------------+---------------------------------------------+--------------------------+
| [SYNTH-RUNNER 61-KEY DIEGETIC KEYBED]                                                             |
| [/ / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / ]|
|  ZONE A: SUB-BASS (AMBER PULSE)  |  ZONE B: NEO-SOLO LEAD (CYAN OVERDRIVE)                         |
+===================================================================================================+
```

---

## 3. Jalur C: Roland Go:Keys 3 Tone Catalog & Setlist Mapping Subsystem

Sistem katalog tone dan otomasi preset panggung (Jalur C) telah diimplementasikan penuh sesuai standar pabrikan Roland:
1. **Spesifikasi MIDI Hardware Roland Go:Keys 3**:
   - Saluran Default Patch Change: **MIDI Channel 4** (0-indexed channel `3`).
   - Urutan 3-Byte Volatile In-RAM: `CC 00` (Bank Select MSB) -> `CC 32` (Bank Select LSB) -> `Program Change` (PC 0–127).
   - Normalisasi PC: Roland Tone List menggunakan 1-indexed (1–128) yang dinormalisasi otomatis menjadi 0-indexed (`pc - 1`) pada wire protocol.
   - Bank Presets: Primary Presets `PR` (`MSB = 71`, `LSB = 96`) dan Common Synthesizer Bank `CM` (`MSB = 87`, `LSB = 85/88/89`).
2. **Coarse Transposition via Standard RPN**:
   - Mengirim RPN MSB = 0 (`CC 101 = 0`), RPN LSB = 2 (`CC 100 = 2`).
   - Mengirim Data Entry MSB (`CC 06 = 64 + transposeValue`, di mana 64 = 0 semitone).
   - Mengirim RPN Null (`CC 101 = 127`, `CC 100 = 127`) untuk mencegah kontaminasi parameter hardware.
3. **Database & Layanan Terpusat**:
   - `src/Core/Data/gokeys3_tones.json`: Database 26 preset Roland Go:Keys 3 terkurasi dalam 7 kategori (`Ac.Piano`, `E.Piano`, `Organ`, `Strings`, `Synth Lead`, `Synth Brass`, `Bass`).
   - `src/Core/Services/ToneService.cs`: Layanan pencarian O(1) berbasis `Dictionary`, filter kategori, dan pencarian multi-tag dengan toleransi zero-crash fallback.
   - `setlists/default_band_setlist.json`: Setlist 4 lagu band panggung lengkap dengan timeline akord dan lirik.
   - `src/UI/MainStageController.cs`: Terintegrasi langsung dengan klik kartu setlist, pergantian mission lagu, dan sinkronisasi real-time ke StageHub mobile.

---

## 4. Hasil Audit Kualitas & Unit Test (Verification Harness)

Eksekusi build .NET 8 dan suite pengujian otomatis (`VerificationHarness.cs`):
```text
=================================================
  ANANTANADA ENGINE VERIFICATION & QUALITY AUDIT  
=================================================
[TEST 1] RetroactiveBuffer Lock-Free Ring Push... PASS (1000 events captured accurately)
[TEST 2] Standard MIDI File Exporter (.mid)... PASS (Valid SMF Type 0, 94 bytes)
[TEST 3] Hardware-Safe Volatile Tone Dispatch... PASS (Volatile MSB/LSB/PC dispatched with zero flash writes)
[TEST 4] Hardware Panic Circuit (CC120, 123, 64 on 16 Ch)... PASS (Broadcasted CC 120/123/64 + All Notes Off to all 16 channels)
[TEST 5] Embedded StageHub Server & WS Telemetry Loop... SKIP (Port binding restricted in environment)
[TEST 6] Jalur C Tone Catalog & Channel 4 Volatile Dispatch... PASS (26 tones loaded across 7 categories; Ch 4 dispatch + RPN verified)
[TEST 7] 100-Song Tactical Repertoire & Random Sampling Dispatch... PASS (100 songs verified [50 Indo / 50 West]; 10 random dispatches on Ch 4 OK)
=================================================
  AUDIT RESULT: 6 PASSED, 0 FAILED
=================================================
```

---

## 5. Master 100-Song Stage Repertoire Database & Preset Matrices

Basis data 100 lagu repertoar panggung festival band SMP/SMA (`setlists/master_100_repertoire.json`) telah diintegrasikan:
1. **50 Lagu Band Pop / Rock Indonesia**:
   - Sheila on 7 (*Dan*, *Kisah Klasik*, *Melompat Lebih Tinggi*, *Sahabat Sejati*, *Sephia*, *J.A.P*, *Kita*, *Radio*)
   - Dewa 19 (*Kangen*, *Pupus*, *Separuh Nafas*, *Kamulah Satu-Satunya*, *Cemburu*, *Pangeran Cinta*)
   - Andra and the Backbone (*Sempurna*, *Main Hati*)
   - Nidji (*Laskar Pelangi*, *Hapus Aku*)
   - The Adams (*Disco Darurat*, *Konservatif*)
   - Diskoria & Laleilmanino (*C.H.R.I.S.Y.E.*, *Serenata Jiwa Disko*)
   - Hindia & .Feast (*Evaluasi*, *Rumah ke Rumah*, *Secukupnya*, *Berita Kehilangan*, *Peradaban*)
   - RAN, Glenn Fredly, Ari Lasso, Fourtwnty, Iwan Fals, Pamungkas, dsb.
2. **50 Lagu Band Pop / Rock Mancanegara**:
   - Oasis (*Don't Look Back in Anger*, *Wonderwall*)
   - Coldplay (*Yellow*, *The Scientist*, *Fix You*, *Viva La Vida*, *Adventure of a Lifetime*)
   - Maroon 5 (*Sunday Morning*, *This Love*, *She Will Be Loved*, *Payphone*, *Moves Like Jagger*)
   - Bon Jovi (*It's My Life*, *Livin' on a Prayer*)
   - Radiohead (*Creep*, *Karma Police*, *High and Dry*, *No Surprises*)
   - Green Day (*Basket Case*, *Wake Me Up When September Ends*, *21 Guns*, *American Idiot*, *Boulevard of Broken Dreams*)
   - Blink-182 (*All The Small Things*, *I Miss You*, *What's My Age Again?*)
   - Bruno Mars (*Count on Me*, *Just the Way You Are*, *Treasure*, *Uptown Funk*, *Locked Out of Heaven*, *When I Was Your Man*)
   - Imagine Dragons, OneRepublic, Ed Sheeran, The Chainsmokers, dsb.
3. **Pencarian Real-Time & Paginasi UI**:
   - Filter Kategori: `[ ALL (100) ]`, `[ INDO (50) ]`, `[ WEST (50) ]`.
   - Text Search Box real-time matching judul, artis, dan tags.
   - Paginasi `< PREV` dan `NEXT >` dengan kartu miring bergaya diegetic.
   - Pemilihan kartu mengeksekusi otomatis MIDI Channel 4 patch change volatile, coarse transposition via RPN, pembaruan split zone, dan pengiriman broadcast `PLAYBACK_TICK` ke browser mobile.

---

## 6. Panduan Ringkas Operasional & Quickstart

1. **Koneksi Keyboard**: Sambungkan port USB Roland Go:Keys 3 ke laptop dengan kabel USB-B ke USB-A/C. Nyalakan keyboard.
2. **Jalankan Aplikasi**:
   - Di Godot 4.3 (.NET): Buka `project.godot`, tekan **F5**.
   - Atau via CLI: `dotnet run` (menjalankan harness verifikasi).
3. **Navigasi Setlist**:
   - Gunakan tombol filter `[ ALL (100) ]`, `[ INDO (50) ]`, atau `[ WEST (50) ]`.
   - Klik kartu lagu untuk langsung mengubah sound keyboard secara otomatis di Channel 4 tanpa menyentuh EEPROM flash.
4. **Tangkap Ide Melodi (Retroactive Scratchpad)**:
   - Tekan tombol `⚡ AMBIL IDE TADI` di bar bawah untuk mengekspor 5 menit terakhir rekaman ke file `.mid`.
5. **StageHub Mobile Remote**:
   - Sambungkan HP/tablet ke Wi-Fi / Hotspot laptop.
   - Buka browser di HP: `http://<HOST_IP>:8080`.
   - Tampilan chord 72pt raksasa, switch lagu, dan tombol Panic darurat aktif seketika.
6. **Panduan Lengkap**: Buka [USER_GUIDE.md](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/USER_GUIDE.md) untuk detail panduan panggung mendalam.

---

# PART II: DOKUMENTASI LENGKAP PROYEK (UNABRIDGED COMPILATION)

Di bawah ini adalah kompilasi lengkap tanpa singkatan dari seluruh berkas `.md` dalam repositori ini:

---

## File: `USER_GUIDE.md`

```markdown
# BUKU PANDUAN PENGGUNAAN LENGKAP & OPERASIONAL PANGGUNG
## ANANTANADA: Hybrid Workstation & Stage Console
### Dedicated Performance Engine & Stylized Diegetic Stage Console untuk Roland Go:Keys 3 (61-Key Synthesizer/Arranger)

> **Dokumen:** Panduan Pengguna & SOP Operasional Panggung  
> **Instrumen Target:** Roland Go:Keys 3 / Go:Keys 5  
> **Host Runtime:** Godot Engine 4.3+ (.NET 8 / C# 12)  
> **Companion Hub:** Embedded Local HTTP & WebSocket Server (`:8080`) + Web Remote (HTML5/Canvas)  
> **Repositori:** https://github.com/y7thangeru/AnantaNada

### 1. Pendahuluan & Konsep Utama
ANANTANADA adalah sistem workstation panggung diegetik hybrid yang dirancang khusus untuk musisi panggung yang menggunakan synthesizer/arranger Roland Go:Keys 3 (atau Go:Keys 5). Sistem ini menggabungkan Engine MIDI C# Zero-Allocation, Konsol Visual Panggung 1080p Berbasis Godot 4.3, Retroactive 5-Minute RAM Scratchpad ("Never-Lost Melody"), dan StageHub Mini-Server & Teleprompter Mobile.

### 2. Garansi Keselamatan Hardware Roland Go:Keys 3
- Zero Flash Writes: Pergantian sound dilakukan 100% di RAM volatil menggunakan pesan standar: Bank Select MSB (CC 00), Bank Select LSB (CC 32), dan Program Change (PC). Memori flash keyboard tidak pernah disentuh.
- Throttled SysEx FIFO Queue: Antrean SysEx diproteksi dengan jeda minimal 20 ms antar-paket untuk mencegah overflow pada prosesor internal Go:Keys 3.
- Hardware-Safe Global Panic Circuit: Mengirimkan broadcast simultan CC 120 (All Sound Off), CC 123 (All Notes Off), CC 64 (Sustain Off = 0), dan Note-Off ke seluruh 16 channel MIDI jika terjadi kabel tercabut atau lag koneksi.

### 3. Langkah Pemasangan & Koneksi Hardware
1. Hubungkan port USB Computer di bagian belakang Roland Go:Keys 3 ke port USB laptop Anda menggunakan kabel USB Type-B ke USB-A / USB-C.
2. Nyalakan Roland Go:Keys 3. Driver Windows (winmm.dll) akan mendeteksinya secara otomatis.
3. Di menu keyboard: pastikan MIDI Rx/Tx Channel aktif di Channel 1, dan Program Change Tx/Rx di-set ON.

### 4. Cara Menjalankan & Menguji Proyek
- Uji Mandiri CLI: `dotnet run` (menjalankan 7 automated tests).
- Menjalankan Melalui Godot: Buka Godot 4.3 .NET, impor `project.godot`, tekan F5.

### 5. Panduan Operasional Konsol Utama (Desktop Stage Console)
- Top Navigation Bar: Device Status, StageHub IP, BPM Metronome, Panic Button.
- Center Panel: Teleprompter lagu aktif, akord besar 72pt, akord antisipasi 32pt, lirik per birama, dan waterfall visualizer.
- Bottom Bar: 61-Key Virtual Keybed dengan pemisahan tangan kiri Amber dan tangan kanan Cyan, serta tombol "AMBIL IDE TADI (5-MIN RETROACTIVE)".

### 6. Panduan Operasional Mobile Remote (Tablet & Smartphone)
- Hubungkan HP/tablet ke Wi-Fi / Hotspot laptop yang sama.
- Buka browser di HP dan akses `http://<HOST_IP>:8080`.
- Layar teleprompter OLED hitam, chord raksasa 72pt, pemilih lagu sentuh, dan tombol darurat Panic siap digunakan dari stand mikrofon.

### 7. Panduan Kustomisasi Setlist & Sound ZEN-Core
- Mengedit daftar lagu di `setlists/master_100_repertoire.json` atau `src/UI/MainStageController.cs`.
- Menyesuaikan nilai Bank Select MSB (87/71), LSB (64-67/96), dan Program Change (0-127).

### 8. Panduan Troubleshooting & Failsafe Panggung
- Keyboard tidak terdeteksi: cabut dan colokkan kembali kabel USB.
- Nada macet / gantung: tekan tombol merah PANIC di desktop atau HP untuk membisukan seluruh channel MIDI seketika.
- HP tidak bisa akses: pastikan satu jaringan Wi-Fi / nyalakan Hotspot laptop dan beri izin di Windows Firewall.
```

---

## File: `README.md`

```markdown
# ANANTANADA: Hybrid Workstation & Stage Console
### Dedicated Performance Engine & Stylized Diegetic Stage Console for Roland Go:Keys 3 (61-Key Synthesizer/Arranger)

[![Runtime: Godot 4.x (.NET 8 / C# 12)](https://img.shields.io/badge/Runtime-Godot_4.3_.NET8_/_C%2312-478cbf?logo=godotengine&logoColor=white)](https://godotengine.org)
[![Target: Roland Go:Keys 3](https://img.shields.io/badge/Target-Roland_Go%3AKeys_3-ffb703?logo=roland&logoColor=black)](https://www.roland.com/global/products/gokeys_3/)
[![Architecture: Hardware--Safe](https://img.shields.io/badge/Architecture-Hardware--Safe_(Zero_Flash_Writes)-00f5d4)](#-hardware-safety-principles)
[![StageHub: Embedded HTTP/WS](https://img.shields.io/badge/StageHub-Embedded_HTTP_%26_WebSocket-ef233c)](#-embedded-stagehub-telemetry-server)
[![User Guide: Complete](https://img.shields.io/badge/User_Guide-Complete_SOP-success)](USER_GUIDE.md)

> **Design Paradigm:** Game-Grade Diegetic Stage Console (*Persona 5* / *Arknights* / *Hi-Fi Rush*)  
> **Host Runtime:** Godot Engine 4.3 (.NET 8 / C# 12)  
> **Companion Client:** Embedded Local HTTP & WebSocket Server + HTML5 / Canvas2D Remote  
> **Remote Repository:** https://github.com/y7thangeru/AnantaNada  
> **Buku Panduan Operasional Lengkap:** USER_GUIDE.md

---

## 🎨 Visual Stage Showcase

| Godot 4 Native Stage Console (1080p Forward+) | Stand-Mounted Mobile Teleprompter & Setlist Remote |
| :---: | :---: |
| ![Godot Stage HUD](captures/screenshot_godot_stage.png) | <img src="captures/screenshot_web_remote.png" width="340" alt="Mobile Remote HUD" /> |

---

## 🌟 Architecture & Core Guarantees

1. Zero Non-Volatile Flash Writes (`SelectPatchVolatile`)
2. Throttled SysEx FIFO Queue (`MidiConstants.SysExDebounceMs = 20ms`)
3. Global Panic Failsafe Circuit (CC 120, CC 123, CC 64 across 16 channels)
4. Zero-Allocation Hot Path (Value-type `MidiRawMessage`, pinned delegate)
5. 5-Minute Retroactive Memory Scratchpad (Lock-free circular buffer, SMF Type 0 exporter)
6. Embedded StageHub & Teleprompter (HTTP & WebSocket server on port 8080)
```

---

## File: `ANANTANADA_SPEC.md`

```markdown
# ANANTANADA ARCHITECTURAL SPECIFICATION & SYSTEM DESIGN

**Target Instrument:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
**Host Runtime:** Godot Engine 4.x (.NET / C# 12)  
**Companion Link:** Embedded Local WebSocket & HTTP Server + Web Remote (HTML5/Canvas)  
**Design Paradigm:** Stylized Diegetic Game HUD & Stage Performance Console (Persona 5 / Arknights / Hi-Fi Rush)

---

## 1. TOPOLOGI SISTEM & ARSITEKTUR RUNTIME

                    +-----------------------------------------+
                    |       ROLAND GO:KEYS 3 HARDWARE         |
                    +--------------------+--------------------+
                                         | (USB-MIDI / BLE MIDI)
                                         v
+---------------------------------------------------------------------------------+
| HOST ENGINE: GODOT 4.X (.NET / C# 12) - DESKTOP (WIN x64)                       |
|                                                                                 |
|  [Hardware-Safe MIDI Engine]      [Retroactive Circular RAM Buffer (5 Mins)]    |
|  - Zero-allocation hot path       - Lock-free ring buffer (NoteOn/Off, CC)      |
|  - Throttled SysEx FIFO queue     - 1-Click Standard MIDI File (.mid) exporter  |
|  - Global Panic Circuit (CC120/123)                                             |
|                                                                                 |
|  [Diegetic Game UI / Rendering]   [Multi-Stem Audio / Metronome Engine]        |
|  - Stylized 2D/3D Node Hierarchy  - Main Out: Backing track stereo              |
|  - Neon velocity-reactive shaders - Cue Out: Metronome click + audio vocal cues |
|  - Adaptive 60/120 FPS GPU pipeline                                             |
|                                                                                 |
|  [Embedded Local Mini-Server (Kestrel / HttpListener)]                          |
|  - Zero-install Web HUD server over LAN / Mobile Wi-Fi Hotspot                  |
|  - Bidirectional WebSocket State Bus (Teleprompter, Setlist triggers, Panic)    |
+----------------------------------------+----------------------------------------+
                                         | (Local Network WebSocket / HTTP)
                                         v
+---------------------------------------------------------------------------------+
| REMOTE CLIENT: STAND-MOUNTED MOBILE / TABLET (ANDROID / BROWSER)               |
|                                                                                 |
|  - High-contrast stage teleprompter (auto-scrolling chord sheet & lyrics)       |
|  - Giant touch-slot setlist switcher (Persona/Arknights inspired)               |
|  - Zero thermal throttling, lightweight Canvas2D execution                      |
+---------------------------------------------------------------------------------+

---

## 2. PRINSIP KEAMANAN HARDWARE (ROLAND GO:KEYS 3)

1. Zero Non-Volatile Flash Writes
2. Channel Voice Isolation
3. Throttled SysEx FIFO Pipeline (20ms debounce delay)
4. Panic Circuit (Failsafe Reset: CC 120, CC 123, CC 64 across 16 channels)

---

## 3. SPESIFIKASI DETAIL MODUL APLIKASI
- Modul 1: Pentas & Band Hub (Live Performance Console)
- Modul 2: Laras & Latihan (Piano Academy & Skill Tracker)
- Modul 3: Studio Kreasi (Songwriting & Arranging Workspace)
- Modul 4: Roland Tone Architect (Go:Keys 3 Ecosystem Manager)

---

## 4. SISTEM DESAIN UI/UX (GAME-GRADE & STYLIZED)
- Inspirasi visual: Persona 5, Arknights, Hi-Fi Rush.
- Palet Warna: Deep Obsidian (#0b0d10), Panel Grey (#161920), Electric Amber (#ffb703), Cyber Cyan (#00f5d4), Warning Crimson (#ef233c).

---

## 5. PROTOKOL TELEMETRI JARINGAN LOKAL (WEBSOCKET DATA CONTRACTS)
- Outbound: `PLAYBACK_TICK`
- Inbound: `TRIGGER_SETLIST_ITEM`
- Inbound: `TRIGGER_PANIC`
```

---

## File: `src/Core/Midi/README.md`

```markdown
# Core MIDI Subsystem (AnantaNada.Core.Midi)
Hardware-Safe Low-Level MIDI Engine & Retroactive Capture Buffer for Roland Go:Keys 3.
- Mathematical Foundations: Lock-free circular ring indexing ($i = (H-1) \ \& \ \text{0x3FFFF}$), SMF Type 0 VLQ delta-time algorithm, 61-key polyphony tracking.
- Roland Hardware Safety: Zero flash writes via volatile patch switching, throttled 20ms SysEx queue, 16-channel panic circuit.
- Complete API Reference: `NativeMidiEngine`, `RetroactiveBuffer`, `MidiRawMessage`, `MidiEventRecord`.
```

---

## File: `src/Network/README.md`

```markdown
# StageHub Network Subsystem (AnantaNada.Network)
Embedded Lightweight HTTP & WebSocket Server for Stand-Mounted Mobile Telemetry.
- Serves static web remote assets on port 8080.
- Bidirectional real-time JSON contracts: `PLAYBACK_TICK`, `TRIGGER_SETLIST_ITEM`, `TRIGGER_PANIC`.
- Thread-safe ConcurrentDictionary client registry and non-blocking asynchronous broadcasts.
```

---

## File: `src/Shaders/README.md`

```markdown
# High-Performance GPU Visualizer Shaders (src/Shaders)
Diegetic Game-Grade Shaders for 61-Key Interactive Keybed & Kinetic Waterfall.
- `piano_keybed.gdshader`: 61-key procedural subdivision (36 white, 25 black keys), dynamic Amber/Cyan neon glow, attack impact transients, and perspective slant.
- `waterfall_visualizer.gdshader`: 61-column grid lines, BPM-synchronized beat markers, falling note trail sampling, and strike beam bloom.
- `tactical_panel.gdshader`: Chamfered polygon corners, caution hazard stripes, and emergency glitch flashes.
```

---

## File: `src/UI/README.md`

```markdown
# UI Orchestration Subsystem (AnantaNada.UI)
Lead Scene Controller Linking Godot 4 Canvas to Core MIDI & Network Subsystems.
- `MainStageController.cs` lifecycle hooks (`_Ready`, `_Process`, `_Notification`).
- Zero-allocation 64x1 dynamic state texture stream.
- Event marshaling to Godot's main thread and remote setlist synchronization.
```

---

## File: `scenes/README.md`

```markdown
# Godot 4 Diegetic Stage Scene Hierarchy (scenes/)
Stage Console Scene Layout & Visual Component Structure.
- Scene Graph: TopBar, CenterPanel (Teleprompter + Waterfall), BottomBar (Keybed + Controls).
- Persona 5 / Arknights styling palette and responsive Full HD viewport scaling.
```

---

## File: `web/README.md`

```markdown
# Mobile Stage Remote Client (web/)
Zero-Install HTML5 / Canvas2D Stand-Mounted Teleprompter & Setlist Controller.
- OLED deep dark theme (#0b0d10), 72pt primary chord display readable from 2 meters.
- Self-healing WebSocket auto-reconnect backoff loop.
- Permanent emergency crimson Panic button.
```

---

## File: `Feature-ToBe-Implemented.md`

```markdown
# Feature Lifecycle Tracking: ANANTANADA Hybrid Workstation
Tracks completed and upcoming features across Core MIDI, Retroactive Buffer, StageHub Server, Shaders, Mobile Remote, and Documentation Architecture.
```

---

## File: `update_tracker.md`

```markdown
# Antigravity Update Tracker: ANANTANADA Hybrid Workstation
Chronological audit log recording every file creation, modification, and architectural rationale.
```
