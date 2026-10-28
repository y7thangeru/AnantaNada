# ANANTANADA ARCHITECTURAL SPECIFICATION & SYSTEM DESIGN

**Target Instrument:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
**Host Runtime:** Godot Engine 4.x (.NET / C# 12)  
**Companion Link:** Embedded Local WebSocket & HTTP Server + Web Remote (HTML5/Canvas)  
**Design Paradigm:** Stylized Diegetic Game HUD & Stage Performance Console (Persona 5 / Arknights / Hi-Fi Rush)

---

## 1. TOPOLOGI SISTEM & ARSITEKTUR RUNTIME

```text
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
```

---

## 2. PRINSIP KEAMANAN HARDWARE (ROLAND GO:KEYS 3)

1. **Zero Non-Volatile Flash Writes**:
   - Dilarang memicu perintah penyimpanan memori internal/EEPROM keyboard.
   - Semua setlist, nama kustom lagu, dan konfigurasi split disimpan di storage lokal host (SQLite/JSON).
   - Penggantian tone hanya menggunakan pesan in-RAM volatil: Bank Select MSB (CC 00), Bank Select LSB (CC 32), dan Program Change (PC).

2. **Channel Voice Isolation**:
   - Komunikasi utama dibatasi pada paket standar MIDI 1.0 (Note On `0x90`, Note Off `0x80`, Control Change `0xB0`, Program Change `0xC0`, Pitch Bend `0xE0`).

3. **Throttled SysEx FIFO Pipeline**:
   - Setiap paket SysEx Roland wajib melewati antrean dengan debounce rate-limiter minimal 20 ms antar paket guna mencegah prosesor Go:Keys 3 mengalami buffer overflow.

4. **Panic Circuit (Failsafe Reset)**:
   - Tombol darurat persisten di UI Desktop dan Remote Mobile yang mengeksekusi pesan reset berikut merata di ke-16 MIDI channel saat kabel terlepas mendadak atau koneksi Bluetooth terputus:
     - CC 120 (All Sound Off)
     - CC 123 (All Notes Off)
     - CC 64 (Sustain Off = 0)

---

## 3. SPESIFIKASI DETAIL MODUL APLIKASI

### Modul 1: Pentas & Band Hub (Live Performance Console)
- **One-Touch Setlist Automation**: Tampilan song-card interaktif. Sekali tap mengirimkan Bank Select dan PC untuk mengubah tone Roland Go:Keys 3, split point, volume balance, dan tempo metronom secara instan.
- **Split Cue Stem Audio Player**: Pemutar audio multi-track. Jalur instrumen dan musik latar stereo dikirim ke Output Utama (PA System / Ampli panggung). Jalur metronom click dan aba-aba vokal ("1, 2, 3, go!") disalurkan terpisah khusus ke headphone/IEM pemain.
- **Visual Teleprompter & Auto-Scroll Lead Sheet**: Menampilkan lirik dan bagan akord dengan kecepatan gulir yang terkunci presisi pada tempo (BPM) dan penanda birama lagu.
- **Instant Stage Transpose**: Fitur coarse tuning instan per lagu via sinyal MIDI. Bila vokalis band meminta perubahan kunci dasar, keyboard bertransposisi otomatis sementara pemain tetap memainkan posisi jari akord yang dipelajarinya.
- **Stage Lock Mode (Zen Mode)**: Mengunci tombol navigasi sensitif untuk mencegah salah sentuh di panggung, memaksimalkan ukuran teks akord dengan kontras tinggi yang terbaca jelas dari jarak 2 meter.

### Modul 2: Laras & Latihan (Piano Academy & Skill Tracker)
- **Hybrid Waterfall to Dual-Staff View**: Visualisasi tuts jatuh vertikal interaktif yang dapat diubah secara instan menjadi partitur not balok ganda (grand staff: treble dan bass clef).
- **Interactive Wait Mode**: Membaca file MIDI latihan; playback berhenti otomatis dan menunggu tuts fisik ditekan dengan nada yang tepat sebelum melangkah ke ketukan berikutnya.
- **Dynamic Velocity Visualizer**: Grafik real-time pemantau dinamika kekuatan jari (nilai 0–127), melatih transisi ekspresi bermain dari *pianissimo* hingga *fortissimo*.
- **Metronome Timing Drift Coach**: Detektor deviasi mikro-ritmis real-time yang memetakan apakah jari cenderung terburu-buru (*rushing*) atau terlambat (*dragging*) terhadap beat metronom.
- **Hand Isolation Mode**: Mode latihan pemisahan tangan: melatih tangan kiri (bassline/akor) dan tangan kanan (melodi) secara independen.

### Modul 3: Studio Kreasi (Songwriting & Arranging Workspace)
- **Retroactive MIDI Scratchpad ("Never-Lost Melody")**: Circular RAM buffer 5 menit yang terus merekam di latar belakang. Saat pemain menemukan riff atau melodi bagus saat jamming santai tanpa tombol record menyala, tombol "Ambil Ide Tadi" langsung mengekspor buffer ke file `.mid` draf.
- **Smart Chord Engine & Harmonizer**: Membaca akord fisik yang ditekan di keyboard secara real-time dan menyajikan saran akord substitusi harmonis (7th, 9th, sus4) untuk memancing eksplorasi komposisi baru.
- **Quick Loop & Layer Sketchpad**: Kanvas pembuatan sketsa lagu berbasis pattern berulang (drum beat, bassline, pad, lead synth) untuk menyusun draf aransemen band sekolah.
- **Integrated Lyric & Bar Notebook**: Editor catatan lirik yang dapat dipatok langsung ke birama tertentu pada timeline lagu.

### Modul 4: Roland Tone Architect (Go:Keys 3 Ecosystem Manager)
- **ZEN-Core Tone Library Explorer**: Database visual katalog ribuan sound preset Roland Go:Keys 3 dengan bookmark favorit, filter genre, dan tag personal.
- **Visual 61-Key Split & Layer Mapper**: Diagram visual 61 tuts keyboard untuk mengatur pembagian zona tangan kiri dan kanan secara intuitif sebelum disuntikkan ke runtime instrumen.
- **Bidirectional MIDI Monitor**: Panel inspeksi data MIDI real-time (Hex dan Desimal) untuk validasi transfer paket via USB maupun Bluetooth MIDI.

---

## 4. SISTEM DESAIN UI/UX (GAME-GRADE & STYLIZED)
- **Visual Inspirations**: Persona 5 / Metaphor: ReFantazio (tipografi kinetik dan panel miring), Arknights (frame antarmuka teknologi futuristik), Hi-Fi Rush (pulsa visual reaktif terhadap ritme).
- **Palet Warna**:
  - Warna Dasar: Deep Obsidian (`#0b0d10`), Panel Grey (`#161920`)
  - Aksen Kinetik: Electric Amber (`#ffb703`), Cyber Cyan (`#00f5d4`), Warning Crimson (`#ef233c`)
- **Pipeline Partikel & Shader**:
  - Menekan tuts memicu burst partikel GPU dengan radius dan intensitas cahaya proporsional terhadap velocity MIDI Note On.
  - Tuts aktif menggunakan neon gradient shader dengan chromatic glow lembut.

---

## 5. PROTOKOL TELEMETRI JARINGAN LOKAL (WEBSOCKET DATA CONTRACTS)

Host mengabstraksikan event real-time ke format JSON terstruktur via WebSocket:

**Event: Status Pemutaran Lagu & Akord (Host -> Client)**
```json
{
  "type": "PLAYBACK_TICK",
  "data": {
    "songId": "song_042",
    "bar": 12,
    "beat": 3,
    "bpm": 128,
    "currentChord": "G#m7",
    "nextChord": "C#m7",
    "activePatch": "Vintage EP 1"
  }
}
```

**Command: Pindah Song dari Remote Mobile (Client -> Host)**
```json
{
  "command": "TRIGGER_SETLIST_ITEM",
  "payload": {
    "songIndex": 3,
    "programChange": 14,
    "bankMsb": 87,
    "bankLsb": 64,
    "transpose": 0
  }
}
```

**Emergency Failsafe (Client -> Host)**
```json
{
  "command": "TRIGGER_PANIC"
}
```
