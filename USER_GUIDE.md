# BUKU PANDUAN PENGGUNAAN LENGKAP & OPERASIONAL PANGGUNG
## ANANTANADA: Hybrid Workstation & Stage Console
### Dedicated Performance Engine & Stylized Diegetic Stage Console untuk Roland Go:Keys 3 (61-Key Synthesizer/Arranger)

> **Dokumen:** Panduan Pengguna & SOP Operasional Panggung  
> **Instrumen Target:** Roland Go:Keys 3 / Go:Keys 5  
> **Host Runtime:** Godot Engine 4.3+ (.NET 8 / C# 12)  
> **Companion Hub:** Embedded Local HTTP & WebSocket Server (`:8080`) + Web Remote (HTML5/Canvas)  
> **Repositori:** [https://github.com/y7thangeru/AnantaNada](https://github.com/y7thangeru/AnantaNada)

---

## DAFTAR ISI
1. [Pendahuluan & Konsep Utama](#1-pendahuluan--konsep-utama)
2. [Kebutuhan Sistem & Persiapan Perangkat](#2-kebutuhan-sistem--persiapan-perangkat)
3. [Langkah Pemasangan & Koneksi Hardware (Roland Go:Keys 3)](#3-langkah-pemasangan--koneksi-hardware-roland-gokeys-3)
4. [Cara Menjalankan & Menguji Proyek](#4-cara-menjalankan--menguji-proyek)
5. [Panduan Operasional Konsol Utama (Desktop Stage Console)](#5-panduan-operasional-konsol-utama-desktop-stage-console)
6. [Panduan Operasional Mobile Remote (Tablet & Smartphone)](#6-panduan-operasional-mobile-remote-tablet--smartphone)
7. [Panduan Kustomisasi Setlist & Sound ZEN-Core](#7-panduan-kustomisasi-setlist--sound-zen-core)
8. [Panduan Troubleshooting & Failsafe Panggung](#8-panduan-troubleshooting--failsafe-panggung)

---

## 1. PENDAHULUAN & KONSEP UTAMA

### 1.1. Apa Itu AnantaNada?
**ANANTANADA** adalah sistem workstation panggung diegetik hybrid yang dirancang khusus untuk musisi, keyboardis, dan band panggung yang menggunakan synthesizer/arranger **Roland Go:Keys 3** (atau Go:Keys 5). Sistem ini menggabungkan:
1. **Engine MIDI C# Zero-Allocation** yang beroperasi langsung di atas driver multimedia Windows (`winmm.dll`) dan Windows Runtime Bluetooth LE MIDI (`Windows.Devices.Midi`).
2. **Konsol Visual Panggung 1080p Berbasis Godot 4.3** dengan tata letak 3-Kolom bergaya game modern (*Persona 5*, *Arknights*, *Hi-Fi Rush*, *Armored Core*), waterfall notes 3D interaktif, dan keybed 61-tuts procedural shader yang responsif terhadap velocity sentuhan tuts.
3. **Retroactive 5-Minute RAM Scratchpad ("Never-Lost Melody")**: Perekam latar belakang terus-menerus tanpa henti ke dalam ring buffer 262,144 slot (~4 MB RAM). Jika Anda menemukan riff atau melodi bagus saat bermain tanpa menyalakan tombol rekam di DAW, cukup tekan 1 tombol dan 5 menit permainan Anda langsung diekspor menjadi file `.mid` (Standard MIDI File Type 0).
4. **StageHub Mini-Server & Teleprompter Mobile**: Server HTTP dan WebSocket lokal yang tertanam langsung di dalam aplikasi. Anggota band atau vokalis dapat membuka browser di HP/tablet mereka untuk melihat chord chart real-time, ketukan metronom, mengganti lagu/sound keyboard dari jauh, serta memicu tombol *Panic* darurat.

### 1.2. Garansi Keselamatan Hardware Roland Go:Keys 3
Keyboard modern portabel seperti Roland Go:Keys 3 memiliki memori flash/EEPROM internal dengan siklus tulis terbatas. Kebanyakan perangkat lunak DAW mengirimkan perintah SysEx secara serampangan yang dapat memicu penyimpanan memori non-volatil.
**AnantaNada menjamin 3 pilar proteksi fisik:**
- **Zero Flash Writes**: Pergantian sound dilakukan 100% di RAM volatil menggunakan pesan standar di Channel 4: Bank Select MSB (`CC 00`), Bank Select LSB (`CC 32`), dan Program Change (`PC`). Memori flash keyboard tidak pernah disentuh.
- **Throttled SysEx FIFO Queue**: Antrean SysEx diproteksi dengan jeda minimal 20 ms antar-paket untuk mencegah overflow pada prosesor internal Go:Keys 3.
- **Hardware-Safe Global Panic Circuit**: Mengirimkan broadcast simultan `CC 120` (All Sound Off), `CC 123` (All Notes Off), `CC 64` (Sustain Off = 0), dan Note-Off ke seluruh 16 channel MIDI jika terjadi kabel tercabut atau lag koneksi.

---

## 2. KEBUTUHAN SISTEM & PERSIAPAN PERANGKAT

### 2.1. Perangkat Keras (Hardware)
- **Synthesizer/Keyboard:** Roland Go:Keys 3 (atau Go:Keys 5) dengan 61 tuts velocity-sensitive.
- **Kabel Penghubung:** Kabel USB Type-A to Type-B (kabel printer standar) atau USB-C to USB-B tergantung port laptop Anda.
- **Komputer/Laptop Host:**
  - OS: Windows 10 / Windows 11 (64-bit).
  - CPU: Intel Core i3 / AMD Ryzen 3 ke atas.
  - RAM: Minimal 4 GB (AnantaNada hanya mengonsumsi ~45 MB RAM).
  - GPU: Mendukung Vulkan Forward+ / Direct3D 12 / OpenGL 3 (Graphics terintegrasi Intel UHD/Iris atau diskrit Nvidia/AMD).
- **Perangkat Remote Tambahan (Opsional):** Smartphone (Android/iOS) atau Tablet (iPad/Android Tablet) yang terhubung ke jaringan Wi-Fi/Hotspot yang sama.

### 2.2. Perangkat Lunak (Software)
- **.NET 8 SDK:** Diperlukan untuk kompilasi C# 12 (`dotnet --version` minimal `8.0.x`).
- **Godot Engine 4.3+ (.NET / Mono Edition):** Pastikan mengunduh edisi bertanda **.NET** (bukan edisi standard).

---

## 3. LANGKAH PEMASANGAN & KONEKSI HARDWARE (ROLAND GO:KEYS 3)

```text
[Roland Go:Keys 3] 
       │ (Kabel USB-B ke USB-A / USB-C atau Bluetooth LE MIDI)
       ▼
[Laptop / PC Windows Host] ── (Jaringan Wi-Fi / Hotspot Lokal) ──▶ [Smartphone / Tablet]
(Godot 4.3 + AnantaNada)                                          (Browser: http://<IP>:8080)
```

### 3.1. Koneksi Fisik USB-MIDI
1. Nyalakan Roland Go:Keys 3 Anda.
2. Tancapkan kabel USB dari port **USB Computer** di bagian belakang Roland Go:Keys 3 ke port USB di laptop Anda.
3. Windows akan secara otomatis mendeteksi perangkat sebagai **Roland GO:KEYS 3** atau **USB Audio Device / MIDI Device** tanpa perlu driver khusus (Class-Compliant).
4. *Verifikasi:* Buka Device Manager di Windows -> cari kelompok *Sound, video and game controllers* -> pastikan nama perangkat Roland terdaftar.

### 3.2. Pengaturan Menu MIDI di Roland Go:Keys 3
Untuk performa optimal panggung bersama AnantaNada:
1. Tekan tombol **[MENU]** di panel Roland Go:Keys 3.
2. Navigasikan ke submenu **MIDI**:
   - **MIDI Rx Channel (Receive):** Set ke **Channel 1** atau **Channel 4** (Jalur C AnantaNada menargetkan Channel 4 / wire index 3).
   - **MIDI Tx Channel (Transmit):** Set ke **Channel 1**.
   - **Program Change Tx/Rx:** Set ke **ON** (memungkinkan AnantaNada mengubah sound instrumen secara otomatis).
   - **Bank Select Tx/Rx:** Set ke **ON**.
   - **Local Control:** 
     - Biarkan **ON** jika ingin speaker keyboard tetap berbunyi mandiri langsung dari tuts fisik.
     - Ubah ke **OFF** jika audio dihasilkan melalui VST host di PC untuk mencegah triggering ganda (*double note-trigger*).

### 3.3. Koneksi Nirkabel Bluetooth LE MIDI (Opsional)
Jika Anda menggunakan fitur Bluetooth nirkabel Roland Go:Keys 3:
1. Aktifkan Bluetooth di Roland Go:Keys 3 melalui menu `Bluetooth -> MIDI On`.
2. Buka menu Windows: *Settings -> Bluetooth & devices -> Add device -> Bluetooth*.
3. Pilih **GO:KEYS 3 MIDI** untuk menghubungkan.
4. AnantaNada menggunakan API WinRT modern (`Windows.Devices.Midi`) yang secara otomatis mendeteksi port BLE MIDI `GO:KEYS 3 MIDI (Bluetooth MIDI IN/OUT)`.

---

## 4. CARA MENJALANKAN & MENGUJI PROYEK

### 4.1. Uji Mandiri CLI (Verification Harness)
Sebelum membuka antarmuka grafis, Anda dapat memverifikasi integritas seluruh subsistem audio, memori ring buffer, katalog tone Jalur C, dan repertoar 100 lagu:

```powershell
cd D:\AntiGravity_Project\AnantaNada_RolandGoKeys3
dotnet run
```

Output verifikasi akan muncul:
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

### 4.2. Menjalankan Melalui Godot Engine 4.3 (.NET Edition)
1. Buka aplikasi **Godot Engine 4.3 .NET**.
2. Di Project Manager, klik **Import** -> Arahkan ke folder:
   `D:\AntiGravity_Project\AnantaNada_RolandGoKeys3\project.godot`
3. Klik **Import & Edit**.
4. Di panel editor Godot, tekan tombol **F5** (atau klik tombol **Play Project** di pojok kanan atas).
5. Layar konsol panggung 1080p native akan terbuka seketika!

---

## 5. PANDUAN OPERASIONAL KONSOL UTAMA (DESKTOP STAGE CONSOLE)

![Godot Stage HUD](captures/screenshot_godot_stage.png)

### 5.1. Top Navigation Bar (Status Bar Panggung)
- **System Status Indicator:** Teks `// SYS_STATUS: OPERATIONAL [ANANTANADA V1.0]` memastikan runtime host berjalan stabil.
- **Device Status & Link:**
  - `> LINK: ROLAND GO:KEYS 3 [ONLINE]` (Cyan terang): Menandakan hardware terhubung fisik via USB / Bluetooth LE MIDI.
  - `> MEMORY: IN-RAM SAFE`: Mengonfirmasi proteksi nol tulis flash aktif.
- **BPM & Metronome Pill:**
  - Menampilkan tempo lagu aktif (misal `[BPM: 128 /// 4/4] [BEAT: 03/16]`) dengan indikator denyut birama yang sinkron.
- **Emergency Button (Merah Crimson Terang):**
  - Tombol **`[ ! EMERGENCY PURGE / PANIC ! ]`** bergaya sakelar militer taktis. Sekali klik langsung memicu pembersihan nada macet di seluruh 16 channel MIDI instrumen.

---

### 5.2. Kolom 1: Tactical Setlist Deck (Repertoar 100 Lagu)

![Setlist Deck](captures/ui_setlist_deck.png)

- **Filter Kategori Sekali Sentuh:**
  - `[ ALL (100) ]`: Menampilkan seluruh 100 repertoar lagu panggung.
  - `[ INDO (50) ]`: Menyaring khusus 50 lagu Pop/Rock Indonesia (Sheila on 7, Dewa 19, Nidji, The Adams, Diskoria, Hindia, .Feast, Glenn Fredly, dsb.).
  - `[ WEST (50) ]`: Menyaring khusus 50 lagu Pop/Rock Barat (Oasis, Coldplay, Maroon 5, Bon Jovi, Radiohead, Green Day, Blink-182, Bruno Mars, dsb.).
- **Kotak Pencarian Cepat (Search Box):** Ketik sebagian judul lagu atau nama artis untuk menyaring instan tanpa latensi.
- **Kartu Misi Miring (Slanted Mission Cards):**
  - Setiap kartu menampilkan: Nomor urut, Judul lagu, Nama Artis, Nada Dasar / Akord Awal, dan Patch Suara Roland Go:Keys 3 yang direkomendasikan.
  - **Aksi Klik Kartu**: Memilih kartu akan seketika mengirimkan perintah MIDI Channel 4 Bank Select MSB/LSB dan Program Change, mengatur transpose RPN, menyinkronkan tempo metronom, dan memancarkan data lagu ke HP mobile remote.
- **Paginasi Taktis:** Tombol `[ < PREV ]` dan `[ NEXT > ]` untuk berpindah halaman lagu dengan mulus saat berada di panggung.

---

### 5.3. Kolom 2: Cyber-Cascade Waterfall & Performance Coach

![Waterfall & Coach](captures/ui_waterfall_coach.png)

- **18° 3D Trapezoidal Waterfall Runway (`waterfall_visualizer.gdshader`):**
  - Jalur tuts jatuh vertikal dengan sudut perspektif tiga dimensi berkecepatan tinggi yang mengalir tepat ke tuts keyboard Anda.
  - 61 garis kolom pemisah dan penanda birama horizontal kuning amber yang bergerak mengikuti tempo lagu.
- **Hit Beam Target Line:**
  - Garis benturan neon Cyan dengan pancaran cahaya lembut (*bloom spread*) di titik kontak saat tuts mencapai bawah.
- **Rhythm Accuracy Coach (*Hi-Fi Rush Style*):**
  - Mengevaluasi presisi ketukan jari pemain terhadap beat metronom secara real-time:
    - `[ PERFECT! ]` (Warna Cyan Terang): Deviasi ritmis $< 25\text{ ms}$.
    - `[ GREAT! ]` (Warna Hijau): Deviasi ritmis $25 \text{--} 50\text{ ms}$.
    - `[ RUSHING! ]` (Warna Kuning): Jari menekan tuts terlalu cepat mendahului ketukan.
    - `[ DRAGGING! ]` (Warna Merah): Jari menekan tuts terlambat dari ketukan.
- **Dynamic Strike Gauge (*Armored Core Style*):**
  - Mengukur kekuatan velocity sentuhan jari secara dinamis dengan bar meter visual dan indikator Rank:
    - `RANK [S]`: Velocity 110–127 (*fortississimo* `fff`). Memicu efek guncangan layar (*screen shake*) halus.
    - `RANK [A]`: Velocity 85–109 (*forte* `f`).
    - `RANK [B]`: Velocity 50–84 (*mezzo-forte* `mf`).
    - `RANK [C]`: Velocity 1–49 (*piano* `p`).
- **Status Retroactive Buffer:**
  - Label `>> RETRO-BUFFER: [ 5m CAPTURE ARMED ]` memastikan sistem perekam latar belakang 5 menit aktif merekam.

---

### 5.4. Kolom 3: Tactical Chord HUD & Lyric Prompter

![Tactical Chord HUD](captures/ui_chord_hud.png)

- **Current Harmony Panel (Panel Akord Aktif):**
  - Menggunakan frame poligon Arknights dengan sudut terpotong (*chamfered*).
  - Menampilkan akord yang sedang dimainkan dalam font raksasa **72pt** tebal (misal `G#m7`), terbaca sangat jelas dari jarak 2 meter saat Anda berdiri di panggung.
- **Transpose Status:**
  - Menampilkan status transpose hardware keyboard (misal `TRANSPOSE: [ +0 ]` atau `[ +2 ]`).
- **Incoming Transition (Akord Berikutnya):**
  - Menampilkan akord berikutnya (misal `NEXT CHORD -> C#m7`) dalam font 32pt berdenyut lembut menjelang pergantian birama, sehingga jari Anda dapat bersiap sebelum ketukan tiba.
- **Lyric Teleprompter Sinkron Birama:**
  - Menampilkan baris lirik lagu yang bergulir secara otomatis mengikuti birama dan ketukan lagu, memastikan vokalis dan keyboardis selalu berada di bait yang tepat.

---

### 5.5. Bottom Bar: Synth-Runner 61-Key Keybed & Never-Lost Melody

![61-Key Keybed](captures/ui_keybed_61.png)

- **Interactive 61-Key Virtual Keybed (`piano_keybed.gdshader`):**
  - Merender secara akurat 36 tuts putih gading dan 25 tuts hitam yang terangkat secara tiga dimensi dengan bayangan jatuh realistis (*ambient occlusion dropshadows*).
  - **Split Point C4 Divider (Garis Pemisah Merah):**
    - Memisahkan secara visual tuts oktaf rendah ($C_2 \text{--} B_3$) dan tuts oktaf tinggi ($C_4 \text{--} C_7$).
  - **Pemisahan Zona Warna Tangan (Dual Neon Hand Split):**
    - **Zone A: Sub-Bass / Akord (Tuts 1–24):** Menyala dengan pendaran **Amber Pulse** (`#ffb703`).
    - **Zone B: Neo-Solo Lead (Tuts 25–61):** Menyala dengan pendaran **Cyan Overdrive** (`#00f5d4`).
  - **Attack Impact Transient Flash:** Menghasilkan kilatan cahaya putih instan di ujung tuts saat disentuh.
- **Tombol "⚡ AMBIL IDE TADI (5-MIN RETROACTIVE)":**
  - Klik tombol ini kapan saja saat Anda menemukan melodi atau arpeggio menarik saat bermain santai.
  - Data permainan 5 menit terakhir langsung diserialisasi ke format Standard MIDI File Type 0 di folder `captures/jam_YYYYMMDD_HHmmss.mid`.
  - Muncul notifikasi toast emas selama 3 detik di layar mengonfirmasi jumlah event dan birama yang tersimpan!

---

## 6. PANDUAN OPERASIONAL MOBILE REMOTE (TABLET & SMARTPHONE)

<img src="captures/screenshot_web_remote.png" width="360" alt="Mobile Remote HUD" />

### 6.1. Menghubungkan Tablet / Smartphone
1. Pastikan smartphone atau tablet terhubung ke **jaringan Wi-Fi yang sama** dengan laptop host, atau hubungkan ke **Mobile Hotspot** laptop host.
2. Buka aplikasi browser favorit Anda (Google Chrome di Android, Safari di iOS/iPadOS).
3. Ketikkan alamat IP yang tertera pada *Network Pill* di konsol desktop, contoh:
   ```text
   http://192.168.1.15:8080
   ```
4. Layar teleprompter panggung mobile akan langsung muncul seketika tanpa perlu login atau instalasi apapun!

### 6.2. Fitur Antarmuka Mobile Remote
1. **Header & Metronome Dots:**
   - Indikator `LINKED // STAGE HUB` (Titik hijau berdenyut) menandakan koneksi WebSocket stabil.
   - 4 titik metronom berdenyut bergantian mengikuti ketukan birama 1-2-3-4.
2. **Display Akord Raksasa:**
   - Menampilkan akord aktif saat ini dalam ukuran besar dengan latar belakang hitam murni OLED `#0b0d10` (menghemat baterai perangkat dan tidak menyilaukan panggung gelap).
3. **Selector Setlist Sentuh (Giant Touch-Slots):**
   - Menampilkan daftar lagu setlist (misal: `01. Neo Shibuya Dawn`, `02. Cybernetic Groove`, dll).
   - Tapping pada salah satu lagu akan:
     - Mengubah sound keyboard Roland Go:Keys 3 secara instan.
     - Mengubah tempo metronom host.
     - Mengubah lead sheet dan akord yang ditampilkan di layar monitor pemain.
4. **Tombol Failsafe Darurat (Panic Button):**
   - Terletak di bagian paling bawah layar dengan warna merah menyala.
   - Tekan kapanpun jika terjadi nada macet untuk memulihkan keheningan panggung dalam 1 milidetik.

---

## 7. PANDUAN KUSTOMISASI SETLIST & SOUND ZEN-CORE

Anda dapat menambahkan daftar lagu sendiri dan memetakan sound Roland Go:Keys 3 sesuai repertoar band Anda.

### 7.1. Mengedit Setlist di `setlists/master_100_repertoire.json`
Buka file `setlists/master_100_repertoire.json`, Anda akan melihat struktur data JSON:

```json
{
  "id": "indo_001",
  "title": "Dan",
  "artist": "Sheila on 7",
  "category": "Indo Pop/Rock",
  "bpm": 115,
  "keySignature": "E Major",
  "transpose": 0,
  "patchName": "Concert Grand",
  "bankMsb": 71,
  "bankLsb": 96,
  "programChange": 1,
  "tags": ["nostalgia", "singalong", "ballad"]
}
```

Cukup tambahkan entri lagu baru dengan format yang sama, dan lagu tersebut akan otomatis muncul pada pencarian dan filter di konsol panggung!

---

## 8. PANDUAN TROUBLESHOOTING & FAILSAFE PANGGUNG

### 8.1. Masalah: Keyboard Tidak Terdeteksi (`SCANNING HARDWARE...`)
- **Penyebab:** Kabel USB belum terhubung dengan baik, atau instrumen dinyalakan setelah aplikasi dibuka.
- **Solusi:**
  1. Pastikan lampu layar Roland Go:Keys 3 menyala.
  2. Cabut kabel USB, tunggu 3 detik, lalu colokkan kembali ke port USB laptop.
  3. Tekan hotkey `R` pada jendela konsol host C# untuk memicu pemindaian ulang instrumen secara instan.

### 8.2. Masalah: Nada Macet / Bunyi Terus-Menerus (Stuck Notes)
- **Penyebab:** Kabel USB MIDI tersenggol atau pedal sustain melepaskan sinyal yang terputus di tengah jalan.
- **Solusi:**
  1. Tekan tombol **`[ ! EMERGENCY PURGE / PANIC ! ]`** di konsol desktop atau tombol **PANIC** di HP mobile remote.
  2. Sirkuit Panic AnantaNada seketika mengirimkan sinyal `CC 120 (All Sound Off)`, `CC 123 (All Notes Off)`, dan mereset sustain pedal `CC 64 = 0` ke seluruh 16 channel MIDI secara merata.

### 8.3. Masalah: Smartphone Tidak Bisa Membuka `http://<HOST_IP>:8080`
- **Penyebab:** Smartphone dan laptop tidak berada di jaringan yang sama, atau Windows Firewall memblokir port 8080.
- **Solusi:**
  1. Pastikan smartphone tersambung ke jaringan Wi-Fi yang sama dengan laptop. Cara paling andal di panggung festival: **Nyalakan fitur Mobile Hotspot di Laptop**, lalu sambungkan smartphone ke hotspot laptop tersebut.
  2. Buka pengaturan Windows Defender Firewall -> Berikan izin (Allow) pada Godot Engine atau port TCP 8080.

---

*ANANTANADA — Professional Stage Experience for Roland Go:Keys 3.*
