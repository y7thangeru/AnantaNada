# ANANTANADA x OHPE UI/UX DESIGN SPECIFICATION
### Industrial Design Synthesis: Teenage Engineering OP-1 / OP-XY & Dieter Rams Modernism
**Engineered for Roland Go:Keys 3 Live Stage Workstation**

---

## 1. Executive Summary & Design Vision

Berdasarkan referensi desain dari proyek open-source terkemuka [**OHPE by groovex**](https://github.com/thegroovex/Ohpe) serta instrumen legendaris *Teenage Engineering OP-1*, *OP-XY*, dan prinsip *Dieter Rams (Ten Principles of Good Design)*, antarmuka AnantaNada telah ditransformasikan secara menyeluruh ke dalam **paradigma perangkat keras industri modern (Modern Hardware Synthesizer Aesthetic)**.

Karakteristik utama dari transformasi ini:
1. **Unibody Anodized Matte Cream Chassis**: Casing perangkat keras unibody berwarna krem/light-stone hangat (`#eae7df`) dengan sudut lengkung lembut (*chamfered corners*), aksen sekrup mikro di empat sudut, dan bayangan bevel taktil.
2. **Keteraturan Grid Simetris 3 Kolom**:
   - **Left Rail (Engine / Category)**: 8 tombol ubin taktil (2 kolom × 4 baris) untuk pemilihan kategori Roland Zen-Core (Piano, E.Piano, Organ, Strings, Brass, Synth, Pad, Sampler).
   - **Center Stage (Recessed OLED Screen)**: Layar kaca gelap matte berbingkai bezel timbul (`#0e0d11`) yang memuat *Cyber-Cascade Waterfall*, *Live Chord Tactical Prompter*, navigasi section lagu (`Intro`, `Verse`, `Chorus`, `Solo`, `Outro`), dan 4 macro sliders.
   - **Right Rail (Mode Switcher)**: 8 tombol ubin taktil (2 kolom × 4 baris) untuk navigasi mode panggung (`Stage`, `Waterfall`, `Chords`, `Lyrics`, `Arranger`, `Sequencer`, `Tape Rec`, `Drum`).
3. **Mid-Console Control Deck (4 Giant Hardware Encoders)**:
   - 4 kenop enkoder putar fisik berukuran besar dengan busur warna radial khas Ohpe:
     - 🔴 **Knob 1 (Red / `#ef4444`)**: `SPLIT BAL` (Keseimbangan volume Zone A Bass vs Zone B Lead).
     - 🟡 **Knob 2 (Yellow / `#f59e0b`)**: `TRANSPOSE` (Transposisi semitone instan -12 s/d +12).
     - 🟣 **Knob 3 (Purple / `#8b5cf6`)**: `CUTOFF` (Filter brightness Roland Zen-Core).
     - 🌸 **Knob 4 (Pink / `#ec4899`)**: `RESONANCE` (Resonansi / Reverb Ambience).
   - Klaster transport mini di sebelah kiri (mini LCD + tombol `START`, `STOP`, `PLAY`, `REC`, `LOOP`).
   - Klaster oktav dan grid di sebelah kanan (`OCT -`, `OCT +`, `GRID`, `MIDI CH4`).
4. **Bottom Physical Keybed & Wheels**:
   - Roda pitch bend dan modulasi vertikal dengan tekstur karet beralur (*ribbed grip*) dan indikator strip kuning di sebelah kiri.
   - Tuts tuts fisik piano 61-key lengkap dengan penanda titik split dinamis (*Split Point C4*), highlight *Zone A Amber* vs *Zone B Cyan*, dan titik indikator merah pada tuts aktif.
   - Sakelar mekanik portamento rocker (`AUTO`, `LEGATO`, `ALWAYS`) dan kenop putar `GLIDE` di sebelah kanan.
5. **Fullscreen Repertoire Librarian Modal (Inspired by Ohpe Library View)**:
   - Tampilan pustaka repertoar 100 lagu bergaya Ohpe yang rapi dengan kolom kategori filter, bilah pencarian real-time, tabel lagu berperingkat bintang, dan kartu detail akord.

---

## 2. Design Tokens & Color Palette

| Token UI | Kode Hex / Nilai | Peruntukan & Filosofi |
| :--- | :--- | :--- |
| `--chassis-bg` | `#eae7df` | Casing unibody aluminium anodized matte |
| `--chassis-border`| `#d4cfc1` | Garis tepi presisi CNC unibody |
| `--panel-bg` | `#f8f6f0` | Permukaan ubin tombol taktil timbul |
| `--panel-border`| `#d5d0c2` | Garis tepi tombol perangkat keras |
| `--screen-bg` | `#0e0d11` | Layar display OLED matte hitam pekat |
| `--screen-border`| `#222028` | Bezel bingkai layar kaca pelindung |
| `--text-dark` | `#2c2930` | Tipografi primer casing (Swiss Neo-Grotesque) |
| `--text-muted` | `#858079` | Label sekunder dan unit parameter |
| `--macro-red` | `#ef4444` | Enkoder 1 & Slider Shape / Split Balance |
| `--macro-yellow`| `#f59e0b` | Enkoder 2 & Slider Detune / Transpose |
| `--macro-purple`| `#8b5cf6` | Enkoder 3 & Slider Cutoff / Filter Brightness |
| `--macro-pink` | `#ec4899` | Enkoder 4 & Slider Resonance / Reverb Depth |
| `--zone-amber` | `#f59e0b` | Zone A Sub-Bass (C2-B3) Highlight |
| `--zone-cyan` | `#06b6d4` | Zone B Stage Lead (C4-C7) Highlight |

---

## 3. Komparasi Tangkapan Layar: Ohpe Asli vs AnantaNada

| Antarmuka | Resolusi | Karakteristik Utama |
| :--- | :---: | :--- |
| **Ohpe Asli (thegroovex)** | Berbagai | Sintesis OP-1 unibody, 4 encoders, 2x4 side tiles, recessed screen |
| **AnantaNada Web Remote** | 1280×820 | Implementasi presisi Ohpe layout, 61-key piano, 4 encoders, chord HUD |
| **AnantaNada Android Companion** | 430×932 | Tata letak vertikal responsif, encoder 4-kolom, touch-safe |
| **AnantaNada Godot Desktop** | 1920×1080 | Konsol Forward+ Vulkan dengan unibody chassis & OLED bezel |

---

## 4. Status Integrasi & Lisensi
- Seluruh aset sprite procedural (`assets/ohpe_ui/generate_ohpe_ui_suite.py`) dirilis di bawah **Creative Commons CC0 1.0 Universal (Public Domain)**.
- Kode dan layout telah terpasang di Web Remote (`web/index.html`, `web/style.css`, `web/app.js`) dan Godot Desktop (`scenes/MainStageView.tscn`).
