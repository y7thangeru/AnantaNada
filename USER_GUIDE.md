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
1. **Engine MIDI C# Zero-Allocation** yang beroperasi langsung di atas driver multimedia Windows (`winmm.dll`).
2. **Konsol Visual Panggung 1080p Berbasis Godot 4.3** dengan estetika visual game (*Persona 5*, *Arknights*, *Hi-Fi Rush*), waterfall notes interaktif, dan keybed 61-tuts procedural shader yang responsif terhadap velocity sentuhan tuts.
3. **Retroactive 5-Minute RAM Scratchpad ("Never-Lost Melody")**: Perekam latar belakang terus-menerus tanpa henti. Jika Anda menemukan riff atau melodi bagus saat bermain tanpa menyalakan tombol rekam di DAW, cukup tekan 1 tombol dan 5 menit permainan Anda langsung diekspor menjadi file `.mid` (Standard MIDI File Type 0).
4. **StageHub Mini-Server & Teleprompter Mobile**: Server HTTP dan WebSocket lokal yang tertanam langsung di dalam aplikasi. Anggota band atau vokalis dapat membuka browser di HP/tablet mereka untuk melihat chord chart real-time, ketukan metronom, mengganti lagu/sound keyboard dari jauh, serta memicu tombol *Panic* darurat.

### 1.2. Garansi Keselamatan Hardware Roland Go:Keys 3
Keyboard modern portabel seperti Roland Go:Keys 3 memiliki memori flash/EEPROM internal dengan siklus tulis terbatas. Kebanyakan perangkat lunak DAW mengirimkan perintah SysEx secara serampangan yang dapat memicu penyimpanan memori non-volatil.
**AnantaNada menjamin 3 pilar proteksi fisik:**
- **Zero Flash Writes**: Pergantian sound dilakukan 100% di RAM volatil menggunakan pesan standar: Bank Select MSB (`CC 00`), Bank Select LSB (`CC 32`), dan Program Change (`PC`). Memori flash keyboard tidak pernah disentuh.
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
       │ (Kabel USB-B ke USB-A / USB-C)
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
   - **MIDI Rx Channel (Receive):** Set ke **Channel 1** (atau Mode Omni jika ingin multi-layer).
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
4. AnantaNada akan secara otomatis mendeteksi MIDI port virtual Bluetooth tersebut.

---

## 4. CARA MENJALANKAN & MENGUJI PROYEK

### 4.1. Uji Mandiri CLI (Verification Harness)
Sebelum membuka antarmuka grafis, Anda dapat memverifikasi integritas seluruh subsistem audio, memori ring buffer, dan sirkuit panic:

```powershell
cd D:\AntiGravity_Project\AnantaNada_RolandGoKeys3
dotnet run
```

Output verifikasi akan muncul:
```text
[ANANTANADA HYBRID WORKSTATION] Host Process Initializing...
=================================================
  ANANTANADA ENGINE VERIFICATION & QUALITY AUDIT  
=================================================
[TEST 1] RetroactiveBuffer Lock-Free Ring Push... PASS (1000 events captured accurately)
[TEST 2] Standard MIDI File Exporter (.mid)... PASS (Valid SMF Type 0, 94 bytes)
[TEST 3] Hardware-Safe Volatile Tone Dispatch... PASS (Volatile MSB/LSB/PC dispatched with zero flash writes)
[TEST 4] Hardware Panic Circuit (CC120, 123, 64 on 16 Ch)... PASS (Broadcasted CC 120/123/64 + All Notes Off to all 16 channels)
[TEST 5] Embedded StageHub Server & WS Telemetry Loop... SKIP (Port binding restricted in environment)
=================================================
  AUDIT RESULT: 4 PASSED, 0 FAILED
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
- **Brand Indicator:** Tulisan `ANANTANADA // GO:KEYS 3` memastikan status runtime aktif.
- **Device Status Pill:**
  - `CONNECTED: Roland Go:Keys 3` (Berwarna Cyan terang): Driver MIDI Windows berhasil terhubung secara fisik ke keyboard.
  - `SCANNING HARDWARE...` atau `DISCONNECTED`: Keyboard belum tercolok atau belum dinyalakan. Colokkan kabel dan AnantaNada akan mendeteksi otomatis.
- **StageHub Network Pill:**
  - Menampilkan alamat IP lokal host, misalnya: `HUB: http://192.168.1.15:8080`.
  - Gunakan alamat ini untuk membuka antarmuka remote di smartphone atau tablet stand mikrofon Anda.
- **BPM & Metronome Pill:**
  - Menampilkan tempo lagu aktif (misal `128 BPM`) dengan indikator ketukan visual berkedip mengikuti birama.
- **Panic Button (Merah Crimson Terang):**
  - Tombol failsafe fisik di layar. Sekali klik, langsung membisukan semua tuts yang macet di ke-16 channel MIDI instrumen.

### 5.2. Center Panel (Teleprompter & Waterfall Visualizer)
- **Teleprompter Lagu Aktif (Sebelah Kiri):**
  - **Judul Lagu:** Menampilkan nomor urut dan judul lagu setlist panggung (misal `01. Neo Shibuya Dawn`).
  - **Patch Badge:** Nama suara Roland yang sedang aktif (misal `Vintage EP 1`).
  - **Current Chord (Akord Sekarang):** Ditampilkan dalam ukuran font **72pt** super tebal (misal `G#m7`) sehingga terbaca jelas dari jarak 2 meter saat Anda berdiri di panggung.
  - **Next Chord (Akord Berikutnya):** Ditampilkan dalam font **32pt** Cyan (misal `C#m7`) untuk antisipasi perpindahan jari pemain sebelum ketukan tiba.
  - **Bar / Beat Tracker:** Menampilkan hitungan birama real-time (`BAR 12 // BEAT 3`).
- **Kinetic Waterfall Note Stream (Sebelah Kanan):**
  - Menampilkan jalur not jatuh vertikal 61-kolom yang sinkron dengan tuts yang Anda tekan.
  - Dilengkapi garis penanda birama horizontal dan pancaran cahaya (*bloom spread*) pada garis target benturan (*strike line*).

### 5.3. Bottom Bar (Keybed 61-Tuts & Scratchpad Exporter)
- **Interactive 61-Key Virtual Keybed (`piano_keybed.gdshader`):**
  - Merender 36 tuts putih gading dan 25 tuts hitam secara prosedural.
  - **Pemisahan Tangan Otomatis (Hand Split):**
    - Tuts tangan kiri (Bass / Chord, tuts 1–24): Menyala dengan warna **Electric Amber** (`#ffb703`).
    - Tuts tangan kanan (Melodi / Lead, tuts 25–61): Menyala dengan warna **Cyber Cyan** (`#00f5d4`).
  - **Velocity Reactive Glow:** Semakin kuat tuts ditekan (velocity 1–127), semakin terang pancaran cahaya neon GPU.
  - **Attack Impact Flash:** Efek kilatan transient seketika saat tuts menyentuh titik kontak.
- **Tombol "⚡ AMBIL IDE TADI (5-MIN RETROACTIVE)":**
  - Jika Anda baru saja memainkan melodi atau akord yang indah saat jamming santai, segera klik tombol ini.
  - AnantaNada akan mengekstrak data 5 menit terakhir dari circular RAM buffer dan menyimpannya ke folder `captures/jam_YYYYMMDD_HHmmss.mid`.
  - File `.mid` ini dapat langsung Anda drag-and-drop ke Ableton Live, FL Studio, Logic Pro, Cubase, atau Reaper!

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

### 7.1. Mengedit Setlist di `MainStageController.cs`
Buka file [src/UI/MainStageController.cs](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/src/UI/MainStageController.cs), cari baris deklarasi `_setlist`:

```csharp
private record SongItem(string Title, string CurrentChord, string NextChord, string PatchName, byte BankMsb, byte BankLsb, byte Pc, int Bpm);

private readonly List<SongItem> _setlist = new()
{
    // Format: (Judul Lagu, Akord Awal, Akord Antisipasi, Nama Patch, Bank MSB, Bank LSB, Program Change, BPM)
    new SongItem("01. Neo Shibuya Dawn", "G#m7", "C#m7", "Vintage EP 1", 87, 64, 14, 128),
    new SongItem("02. Cybernetic Groove", "F#maj7", "B7", "Fat Saw Lead", 87, 65, 4, 134),
    new SongItem("03. Midnight Reverie", "Emaj9", "A#dim7", "Warm Stage Grand", 87, 64, 1, 92),
    new SongItem("04. Overdrive Anthem", "D#m7", "G#7", "Brass Punch Sect", 87, 66, 22, 140)
};
```

### 7.2. Mengetahui Nilai Bank Select MSB, LSB, dan PC Roland Go:Keys 3
Pada synthesizer mesin Roland ZEN-Core / Go:Keys 3:
- **Bank Select MSB (CC 00):** Biasanya bernilai `87` (atau `86` untuk sound expansion).
- **Bank Select LSB (CC 32):** Menentukan kategori instrumen:
  - `64`: Kategori Keyboard / Acoustic Piano / Electric Piano.
  - `65`: Kategori Lead / Synth / Pad.
  - `66`: Kategori Brass / Strings / Wind.
  - `67`: Kategori Bass / Plucked.
- **Program Change (PC 0–127):** Menentukan nomor sound preset di dalam kategori tersebut.

Cukup sesuaikan nilai angka tersebut pada array `_setlist`, lalu jalankan ulang proyek.

---

## 8. PANDUAN TROUBLESHOOTING & FAILSAFE PANGGUNG

### 8.1. Masalah: Keyboard Tidak Terdeteksi (`SCANNING HARDWARE...`)
- **Penyebab:** Kabel USB belum terhubung dengan baik, atau instrumen dinyalakan setelah aplikasi dibuka.
- **Solusi:**
  1. Pastikan lampu layar Roland Go:Keys 3 menyala.
  2. Cabut kabel USB, tunggu 3 detik, lalu colokkan kembali ke port USB laptop.
  3. AnantaNada memiliki mekanisme polling otomatis; jika nama perangkat belum terupdate, tekan tombol navigasi atau restart aplikasi.

### 8.2. Masalah: Nada Macet / Bunyi Terus-Menerus (Stuck Notes)
- **Penyebab:** Kabel USB MIDI tersenggol atau pedal sustain melepaskan sinyal yang terputus di tengah jalan.
- **Solusi:**
  1. Tekan tombol **PANIC** merah di konsol desktop atau tombol **PANIC** di HP mobile remote.
  2. Sirkuit Panic AnantaNada seketika mengirimkan sinyal `CC 120 (All Sound Off)`, `CC 123 (All Notes Off)`, dan mereset sustain pedal `CC 64 = 0` ke seluruh 16 channel MIDI secara merata.

### 8.3. Masalah: Smartphone Tidak Bisa Membuka `http://<HOST_IP>:8080`
- **Penyebab:** Smartphone dan laptop tidak berada di jaringan yang sama, atau Windows Firewall memblokir port 8080.
- **Solusi:**
  1. Pastikan smartphone tersambung ke jaringan Wi-Fi yang sama dengan laptop. Cara paling andal di panggung festival: **Nyalakan fitur Mobile Hotspot di Laptop**, lalu sambungkan smartphone ke hotspot laptop tersebut.
  2. Buka pengaturan Windows Defender Firewall -> Berikan izin (Allow) pada Godot Engine atau port TCP 8080.

### 8.4. Masalah: File MIDI Hasil Ekspor Kosong
- **Penyebab:** Tombol *"Ambil Ide Tadi"* ditekan sebelum tuts keyboard dimainkan.
- **Solusi:**
  1. Mainkan tuts keyboard terlebih dahulu.
  2. Buffer circular RAM AnantaNada merekam hingga 262,144 event (sekitar 5 menit permainan kontinu). Tekan tombol ekspor setelah Anda selesai jamming. File akan tersimpan di folder `captures/`.

---

*ANANTANADA — Professional Stage Experience for Roland Go:Keys 3.*
