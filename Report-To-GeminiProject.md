# CONSOLIDATED MASTER REPORT: ANANTANADA HYBRID WORKSTATION
### Dedicated Performance Engine & Stylized Diegetic Stage Console for Roland Go:Keys 3

**Target Instrument:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
**Host Runtime:** Godot Engine 4.x (.NET 8 / C# 12)  
**Companion Link:** Embedded Local Mini-Server (HTTP/WebSocket) + Mobile Stage Remote (Canvas2D)  
**Design Paradigm:** Stylized Diegetic Game HUD & Stage Performance Console (*Persona 5* / *Arknights* / *Hi-Fi Rush* / *Armored Core* / *12 Interface In Game Archetypes*)  
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
5. **12 Game Archetypes (Interface In Game Suite)**:
   - Sentuhan estetika DJMAX Respect V, NOSTALGIA Op. 2, GRIS, Moonlighter, Stardew Valley, Life is Strange, Taiko no Tatsujin, Cadence of Hyrule, Mini Metro, Tricky Towers, dan Snipperclips.

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
1. **50 Lagu Band Pop / Rock Indonesia**: Sheila on 7, Dewa 19, Andra and the Backbone, Nidji, The Adams, Diskoria, Hindia, .Feast, RAN, Glenn Fredly, Ari Lasso, Fourtwnty, Iwan Fals, Pamungkas, dsb.
2. **50 Lagu Band Pop / Rock Mancanegara**: Oasis, Coldplay, Maroon 5, Bon Jovi, Radiohead, Green Day, Blink-182, Bruno Mars, Imagine Dragons, OneRepublic, Ed Sheeran, The Chainsmokers, dsb.
3. **Pencarian Real-Time & Paginasi UI**: Filter Kategori `[ ALL (100) ]`, `[ INDO (50) ]`, `[ WEST (50) ]`, pencarian teks instan, paginasi taktis, dan diseminasi otomatis ke Roland Go:Keys 3 Channel 4.

---

## 6. Galeri Visual UI/UX & Artefak Tangkapan Layar Lengkap

| Modul Konsol | File Artefak | Resolusi & Penjelasan Visual |
| :--- | :--- | :--- |
| **Cockpit Utama 1080p** | `captures/screenshot_godot_stage.png` | 1920x1055: Layar penuh konsol panggung Forward+ Full HD. |
| **Mobile Remote Teleprompter** | `captures/screenshot_web_remote.png` | 900x1100: Tampilan mobile Canvas2D hitam OLED dengan akord raksasa dan tombol panic. |
| **Tactical Setlist Deck (100 Songs)** | `captures/ui_setlist_deck.png` | 530x710: Close-up panel kiri: kotak pencarian, filter kategori, kartu miring Atlus style, dan paginasi. |
| **Cyber-Cascade Waterfall & Timing Coach**| `captures/ui_waterfall_coach.png` | 810x710: Close-up panel tengah: 18° 3D runway, focused hit beam, rhythm accuracy meter, dan strike gauge Rank S. |
| **Tactical Chord HUD & Bar Lyric Prompter**| `captures/ui_chord_hud.png` | 530x710: Close-up panel kanan: box akord 72pt Arknights chamfered panel, status transpose, next chord transition, dan lirik. |
| **Synth-Runner 61-Key Keybed** | `captures/ui_keybed_61.png` | 1880x210: Close-up bar bawah: 36 tuts putih gading & 25 tuts hitam 3D, Split Point C4 merah, dan dual Amber/Cyan glow. |

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
- Zero Flash Writes: Pergantian sound dilakukan 100% di RAM volatil menggunakan pesan standar di Channel 4: Bank Select MSB (CC 00), Bank Select LSB (CC 32), dan Program Change (PC). Memori flash keyboard tidak pernah disentuh.
- Throttled SysEx FIFO Queue: Antrean SysEx diproteksi dengan jeda minimal 20 ms antar-paket untuk mencegah overflow pada prosesor internal Go:Keys 3.
- Hardware-Safe Global Panic Circuit: Mengirimkan broadcast simultan CC 120 (All Sound Off), CC 123 (All Notes Off), CC 64 (Sustain Off = 0), dan Note-Off ke seluruh 16 channel MIDI jika terjadi kabel tercabut atau lag koneksi.

### 3. Langkah Pemasangan & Koneksi Hardware
1. Hubungkan port USB Computer di bagian belakang Roland Go:Keys 3 ke port USB laptop Anda menggunakan kabel USB Type-B ke USB-A / USB-C (atau aktifkan Bluetooth LE MIDI).
2. Nyalakan Roland Go:Keys 3. Driver Windows (winmm.dll / Windows.Devices.Midi) akan mendeteksinya secara otomatis.
3. Di menu keyboard: pastikan MIDI Rx/Tx Channel aktif di Channel 1 atau Channel 4, dan Program Change Tx/Rx di-set ON.

### 4. Cara Menjalankan & Menguji Proyek
- Uji Mandiri CLI: `dotnet run` (menjalankan 7 automated tests).
- Menjalankan Melalui Godot: Buka Godot 4.3 .NET, impor `project.godot`, tekan F5.

### 5. Panduan Operasional Konsol Utama (Desktop Stage Console)
- Top Navigation Bar: Device Status, StageHub IP, BPM Metronome, Panic Button.
- Kolom 1 (Setlist Deck): Filter Kategori [ALL], [INDO], [WEST], Kotak Pencarian, Kartu Misi Miring Atlus Style, Paginasi.
- Kolom 2 (Cyber-Cascade Waterfall & Performance Coach): 18° 3D Runway, Focused Hit Beam, Rhythm Timing Accuracy Coach (PERFECT!, GREAT!, RUSHING, DRAGGING), Dynamic Strike Gauge Rank S/A/B.
- Kolom 3 (Tactical Chord HUD & Lyric Prompter): Frame Akord 72pt Arknights chamfered panel, status transpose, next chord transition, lirik per birama.
- Bottom Bar: Synth-Runner 61-Key Keybed dengan Split Point C4 garis merah, zona Amber/Cyan, dan tombol "AMBIL IDE TADI (5-MIN RETROACTIVE)".

### 6. Panduan Operasional Mobile Remote (Tablet & Smartphone)
- Hubungkan HP/tablet ke Wi-Fi / Hotspot laptop yang sama.
- Buka browser di HP dan akses `http://<HOST_IP>:8080`.
- Layar teleprompter OLED hitam, chord raksasa 72pt, pemilih lagu sentuh, dan tombol darurat Panic siap digunakan dari stand mikrofon.
```

---

## File: `UI_UX_DESIGN_GUIDE.md`

```markdown
# ANANTANADA UI/UX Design System & Customization Guide
### Comprehensive Specification, 12-Game Archetype Architecture & Maintenance SOP

> **Target Console:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
> **Host Engine:** Godot Engine 4.3 Forward+ (.NET 8 / C# 12 / GDScript)  
> **Companion Client:** Embedded StageHub Mini-Server + Mobile Canvas2D Remote  
> **Asset Suite:** 22 Vector SVG Templates + 18 Transparent PNG Sprites (CC0 Public Domain)  
> **Reference Benchmark:** Interface In Game (interfaceingame.com)

### 12-Game Archetype Architecture
- **Hi-Fi Rush**: Comic burst badges & slanted SSS rank animations
- **DJMAX RESPECT V**: Segmented FEVER x5 gauges & 100% MAX BEAT precision judgement
- **NOSTALGIA Op. 2**: Baroque gilded gold filigree & concert hall crimson wax seal
- **GRIS**: Minimalist celestial star icons & watercolor vignette Zen mode
- **Moonlighter**: Stitched leather ledger tabs & gold pouch badges
- **Stardew Valley**: Rustic carved wood corner frames & parchment banners
- **Life is Strange: BtS**: Semi-translucent torn washi tape strips & polaroid photo frames
- **Taiko no Tatsujin**: Don (Bass) & Ka (Treble) drum pairs & combo fireworks
- **Cadence of Hyrule**: 16-Bit heart pulse meter & triforce corner brackets
- **Mini Metro**: Transit schematic song section progression map
- **Tricky Towers**: Gothic masonry stone spell frames
- **Snipperclips**: Perforated papercraft cutout tags & pastel yellow sticky notes
```

---

## File: `assets/ui_sprites/README.md`

```markdown
# ANANTANADA UI Sprites & Design Templates
### 12-Game Master UI/UX Archetype Suite (CC0 Public Domain)

Direktori ini berisi koleksi 22 Vector SVG Templates dan 18 Transparent PNG Sprites siap pakai yang diekstraksi dari 12 game berperingkat tinggi di Interface In Game untuk live performance konsol Roland Go:Keys 3.
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

### Visual Stage Showcase:
- Desktop 1080p Cockpit: `captures/screenshot_godot_stage.png`
- Stand-Mounted Mobile Remote: `captures/screenshot_web_remote.png`
- Detailed Modules: `captures/ui_setlist_deck.png`, `captures/ui_waterfall_coach.png`, `captures/ui_chord_hud.png`, `captures/ui_keybed_61.png`
```

---

## File: `scenes/README.md`

```markdown
# Godot 4 Diegetic Stage Scene Hierarchy (scenes/)
Stage Console Scene Layout & 3-Column Cockpit Component Structure.
- Scene Graph: TopBar, CockpitBody (SetlistDeck + CenterCockpit + RightTacticalHUD), BottomBar (SynthRunnerKeybed + Controls).
- Component Visual Artifacts: `screenshot_godot_stage.png`, `ui_setlist_deck.png`, `ui_waterfall_coach.png`, `ui_chord_hud.png`, `ui_keybed_61.png`.
```

---

## File: `src/Core/Midi/README.md`

```markdown
# Core MIDI Subsystem (AnantaNada.Core.Midi)
Hardware-Safe Low-Level MIDI Engine & Retroactive Capture Buffer for Roland Go:Keys 3.
- Mathematical Foundations: Lock-free circular ring indexing ($i = (H-1) \ \& \ \text{0x3FFFF}$), SMF Type 0 VLQ delta-time algorithm, 61-key polyphony tracking.
- Roland Hardware Safety: Zero flash writes via volatile patch switching, throttled 20ms SysEx queue, 16-channel panic circuit.
- Modern Windows BLE MIDI: `Windows.Devices.Midi` WinRT direct GATT communication.
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
- `piano_keybed.gdshader`: 36 white keys, 25 elevated 3D black keys, Split Point C4 red divider, Amber/Cyan dual neon glow.
- `waterfall_visualizer.gdshader`: 18° trapezoidal 3D runway, 61 column grid lines, BPM beat markers, focused hit beam bloom.
- `tactical_panel.gdshader`: Chamfered polygon corners, caution hazard stripes, and emergency glitch flashes.
```

---

## File: `src/UI/README.md`

```markdown
# UI Orchestration Subsystem (AnantaNada.UI)
Lead Scene Controller Linking Godot 4 Canvas to Core MIDI & Network Subsystems.
- `MainStageController.cs` (C#) & `MainStageController.gd` (GDScript Bridge).
- Zero-allocation 64x1 dynamic state texture stream.
- Event marshaling to Godot's main thread, 100-song repertoire filtering, and remote setlist synchronization.
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
Tracks completed and upcoming features across Core MIDI, Retroactive Buffer, StageHub Server, Shaders, Mobile Remote, Tone Catalog (Jalur C), 100-Song Repertoire, 12-Game UI/UX Archetypes, and Documentation Architecture.
```

---

## File: `update_tracker.md`

```markdown
# Antigravity Update Tracker: ANANTANADA Hybrid Workstation
Chronological audit log recording every file creation, modification, and architectural rationale.
```

---

## File: `assets/ui_sprites/README.md`

```markdown
# ANANTANADA UI Sprites & Design Templates
### 12-Game Master UI/UX Archetype Suite (CC0 Public Domain)

Direktori ini berisi koleksi aset visual dan template vektor siap pakai yang diekstraksi dari 12 game berperingkat tinggi di Interface In Game:
- Hi-Fi Rush, DJMAX RESPECT V, NOSTALGIA Op. 2, GRIS, Moonlighter, Stardew Valley, Life is Strange: BtS, Taiko no Tatsujin, Cadence of Hyrule, Mini Metro, Tricky Towers, Snipperclips.
- 22 Editable Vector SVG Templates (`assets/ui_sprites/templates/`).
- 18 Broadcast-ready transparent PNG sprites (`assets/ui_sprites/png/`).
- Procedural Python/Pillow generator script (`assets/ui_sprites/scripts/generate_all_sprites.py`).
- Lisensi 100% Free / Creative Commons CC0 1.0 Universal (Public Domain Dedication).
```

---

## File: `UI_UX_DESIGN_GUIDE.md`

```markdown
# ANANTANADA UI/UX Design System & Customization Guide
Panduan komprehensif arsitektur antarmuka 12-game master, spesifikasi interaktif node Godot & Web Remote, SOP edit template SVG di Figma/Inkscape/Illustrator, regenerasi Python Pillow, dan tuning animasi.
Menyertakan screenshot implementasi nyata dari 3 platform (Desktop 1080p Vulkan, Web/Tablet Landscape Stand-Mounted, dan Android Mobile Portrait OLED).
```


---

## File: `assets/rpg_ui/generate_rpg_ui_suite.py` & Sweet RPG Game Suite

```markdown
# Sweet & Charming Interactive RPG Game UI/UX Suite
Pembaruan menyeluruh mengubah konsol dari techwear gelap menjadi antarmuka RPG diegetik yang manis, hangat, dan taktil:
- Aset Baru: 14 sprite supersampled (desk_wood_surface, notebook_page_parchment, spiral_wire_binding, scalloped_cloud_banner, tab_mint/honey/berry/sky, card_ac_recipe_item, card_ac_stamp_slot, emoji_reactions, badge_gold_coin_rpg, badge_star_pin, gauge_rpg_mana_bar, parchment_codex_scroll, card_rpg_spell_harmony).
- Layout & Precision Alignment Fix (Resolusi Teks Tabrakan & Overflow):
  - Sprite `card_ac_recipe_item.png` dibersihkan dari kotak statis bawaan, memungkinkan tata letak 2 kolom fleksibel (badge box di kiri + teks judul/patch di kanan).
  - Margin notebook leather-bound di Godot dibatasi via `NotebookMargin` sehingga kotak pencarian dan filter genre tidak melebar melewati halaman.
  - Kartu mantra `CurrentChordBox` diperbesar menjadi 340x136px dan indikator transpose diberi latar belakang kapsul (pill) elegan sehingga tidak terpotong garis emas rune.
- Desktop 1080p: Godot 4 Forward+ Vulkan (`captures/screenshot_godot_stage.png`).
- Web Remote (Desktop/Tablet): 1280x820 (`captures/screenshot_web_remote.png`).
- Android Companion: 430x932 Mobile Portrait (`captures/screenshot_android_companion.png`).
- Lisensi: 100% Free / Creative Commons CC0 1.0 Universal.
```
