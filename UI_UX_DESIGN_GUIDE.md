# ANANTANADA UI/UX Design System & Customization Guide
### Comprehensive Specification, 12-Game Archetype Architecture & Maintenance SOP

> **Target Console:** Roland Go:Keys 3 (61-Key Synthesizer / Arranger)  
> **Host Engine:** Godot Engine 4.3 Forward+ (.NET 8 / C# 12 / GDScript)  
> **Companion Client:** Embedded StageHub Mini-Server + Mobile Canvas2D Remote  
> **Asset Suite:** 22 Vector SVG Templates + 18 Transparent PNG Sprites (CC0 Public Domain)  
> **Reference Benchmark:** [Interface In Game (interfaceingame.com)](https://interfaceingame.com/)

---

## 📑 Daftar Isi
1. [Filosofi Desain & Latar Belakang](#1-filosofi-desain--latar-belakang)
2. [Peta 12 Arketipe Game & Integrasi Sistem](#2-peta-12-arketipe-game--integrasi-sistem)
3. [Spesifikasi Komponen & Perilaku Interaktif](#3-spesifikasi-komponen--perilaku-interaktif)
   - [Top System Bar (Cadence of Hyrule & GRIS)](#top-system-bar)
   - [Waterfall Visualizer Deck (NOSTALGIA Op. 2 & Mini Metro)](#waterfall-visualizer-deck)
   - [Performance Coach Deck (DJMAX RESPECT V & Hi-Fi Rush)](#performance-coach-deck)
   - [Setlist Repertoire Deck (Moonlighter & Life is Strange: BtS)](#setlist-repertoire-deck)
   - [Tactical Chord HUD & Teleprompter (Taiko, Stardew Valley, Tricky Towers, Snipperclips)](#tactical-chord-hud--teleprompter)
4. [Panduan Lengkap Kustomisasi, Edit Template & Update](#4-panduan-lengkap-kustomisasi-edit-template--update)
   - [Alur Kerja Edit Template Vektor (Figma / Inkscape / Illustrator)](#alur-kerja-edit-template-vektor)
   - [Alur Kerja Regenerasi Otomatis (Python Pillow Generator)](#alur-kerja-regenerasi-otomatis)
   - [Alur Kerja Menambahkan Sprite Baru ke Godot Scene](#alur-kerja-menambahkan-sprite-baru-ke-godot-scene)
   - [Alur Kerja Sinkronisasi ke Web Remote Mobile Companion](#alur-kerja-sinkronisasi-ke-web-remote-mobile-companion)
5. [Tuning Parameter Shader & Animasi](#5-tuning-parameter-shader--animasi)
6. [Inventaris File Aset & Lisensi CC0](#6-inventaris-file-aset--lisensi-cc0)

---

## 1. Filosofi Desain & Latar Belakang

Sistem konsol panggung **ANANTANADA** dirancang untuk memecahkan dilema klasik musisi panggung: *bagaimana menghadirkan informasi teknis panggung (akord, lirik, split tuts, tempo, dan status instrumen) dengan keterbacaan instan dari jarak 2 meter tanpa terlihat membosankan seperti form administrasi perkantoran.*

Dengan mempelajari 12 game terbaik dari platform kurasi antarmuka [Interface In Game](https://interfaceingame.com/), ANANTANADA menggabungkan 4 pilar visual:
1. **Temporal Rhythm Clarity**: Setiap elemen visual berdenyut dan bernapas selaras dengan BPM lagu.
2. **Arcade Precision Feedback**: Evaluasi kekuatan pukulan (*velocity*) dan akurasi waktu ketukan secara visual tanpa menutupi fokus permainan.
3. **Tactile & Diegetic Charm**: Menu repertoar dan teleprompter dibungkus tekstur fisik nyata (kulit berjahit, perkamen, selotip robek, sticky note) menyerupai binder panggung otentik.
4. **Zen Aesthetic Switch**: Fleksibilitas mengubah konsol penuh data menjadi panggung hening minimalis untuk sesi solo atau konser intim.

---

## 📸 Tangkapan Layar Implementasi Tri-Platform (Desktop, Web & Android)

Implementasi visual UI/UX 12 game master telah diverifikasi dan berjalan aktif secara responsif di ketiga platform:

### 1. Desktop Stage Console (Godot 4 Forward+ 1080p Vulkan)
Konsol utama panggung yang menampilkan seluruh instrumen tuts 61-key, waterfall laser visualizer, dek repertoar 100 lagu, dan elemen grafis 12 game master.
![Desktop Godot Stage Console](captures/screenshot_godot_stage.png)

### 2. Web / Tablet StageHub Remote (1200x800 Stand-Mounted Landscape)
Tampilan browser Chrome/Safari pada tablet yang dipasang di stand keyboard via embedded local server (`:8080`).
![Web Tablet Remote](captures/screenshot_web_remote.png)

### 3. Android Mobile Touch Companion (430x932 OLED Mic-Stand Portrait)
Tampilan smartphone Android yang terpasang di holder tiang mikrofon untuk musisi/vokalis panggung.
<p align="center">
  <img src="captures/screenshot_android_companion.png" width="360" alt="Android Companion" />
</p>

---

## 2. Peta 12 Arketipe Game & Integrasi Sistem

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                              ANANTANADA MASTER STAGE CONSOLE                           │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ [TopBar]                                                                               │
│  - GRIS (Zen Star Toggle Button)                 - Cadence of Hyrule (Pulsing Heart)   │
│  - System Status & Roland Online Link            - Metronome [BPM / Bar / Beat]        │
├──────────────────────────┬─────────────────────────────────┬───────────────────────────┤
│ [Setlist Deck]           │ [Waterfall Visualizer]          │ [Chord Tactical HUD]      │
│  - Moonlighter           │  - Mini Metro (Route Map)       │  - Current Chord Hero Box │
│    (Stitched Leather Tab)│  - NOSTALGIA Op. 2              │  - Tricky Towers          │
│  - Life is Strange       │    (Gilded Baroque Corners)     │    (Runic Spell Frame)    │
│    (Torn Washi Tape)     │  - 61-Key Laser Runway          │  - Taiko no Tatsujin      │
│  - 100 Repertoire Cards  │                                 │    (Don/Ka Drum Indicator)│
│    with Pagination       │ [Coach Deck]                    │  - Stardew Valley         │
│                          │  - DJMAX (100% MAX BEAT Badge)  │    (Rustic Wood & Banner) │
│                          │  - DJMAX (x5 FEVER Gauge Bar)   │  - Snipperclips           │
│                          │  - Hi-Fi Rush (Comic Burst)     │    (Pastel Sticky Note)   │
│                          │  - Strike Gauge & Rank [S/A/B]  │  - Teleprompter Lyrics    │
├──────────────────────────┴─────────────────────────────────┴───────────────────────────┤
│ [BottomKeybed]                                                                         │
│  - Zone A (Amber Bass // C2-B3) | Split Point C4 | Zone B (Cyan Lead Solo // C4-C7)    │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Spesifikasi Komponen & Perilaku Interaktif

### Top System Bar
* **Cadence of Hyrule (`hud_cadence_heart_pulse.png`)**:
  * **Lokasi Node:** `$HUD/TopBar/CadenceHeartRect`
  * **Interaksi:** Pada setiap ketukan metronom downbeat birama, jantung berdenyut membesar (`scale = 1.16`) dan bersinar merah-emas sebelum meluruh kembali ke ukuran normal dalam 120 ms.
* **GRIS Zen Mode (`icon_gris_celestial_star.png` & `overlay_gris_zen_vignette.png`)**:
  * **Lokasi Node:** `$HUD/TopBar/ZenModeBtn` & `$ZenVignetteOverlay`
  * **Interaksi:** Tombol bintang konstelasi minimalis. Saat diklik, seluruh panel samping (Setlist, Coach Deck, Chord HUD) disembunyikan seketika dan overlay vignette cat air halus aktif, menyisakan hanya tuts piano dan waterfall neon untuk suasana konser hening.

### Waterfall Visualizer Deck
* **Mini Metro Transit Map (`widget_minimetro_transit_map.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/CenterCascade/MiniMetroTransitMap`
  * **Interaksi:** Diagram skema rute stasiun kereta transit memetakan progres lagu:
    * Birama 01–04: `INTRO ◯`
    * Birama 05–16: `VERSE △`
    * Birama 17–28: `CHORUS ▢` (Node menyala aktif)
    * Birama 29–36: `SOLO ◇`
    * Birama 37+: `OUTRO ◯`
    Saat berpindah stasiun struktur lagu, widget memancarkan denyut cahaya emas.
* **NOSTALGIA Op. 2 Gilded Corners (`ornament_nostalgia_gilded_corner.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/CenterCascade/WaterfallRect/NostalgiaCornerLeft` & `NostalgiaCornerRight`
  * **Interaksi:** Sudut ukiran barok berlapis emas membingkai layar air terjun nada jatuh, memancarkan kilau emas (*gold shimmer*) setiap kali akord berganti.

### Performance Coach Deck
* **DJMAX RESPECT V FEVER Gauge (`hud_djmax_fever_gauge.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/CenterCascade/CoachDeck/DJMaxFeverGauge`
  * **Interaksi:** Bar LED bertingkat mengisi energi seiring pemain konsisten memukul tuts pada tempo akurat. Saat energi penuh ($100\%$), bar bertransisi ke warna ungu-magenta neon *FEVER MODE*.
* **DJMAX 100% MAX BEAT Judgement (`badge_djmax_judgement_max.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/CenterCascade/CoachDeck/DJMaxJudgementBadge`
  * **Interaksi:** Berkedip emas terang saat pemain memainkan nada dengan velocity $\ge 110$.
* **Hi-Fi Rush Comic Burst (`badge_hifi_comic_burst.png` & `badge_hifi_rank_sss.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/CenterCascade/CoachDeck/HiFiBurstBadge`
  * **Interaksi:** Badge ledakan komik pop-art melonjak membesar (*punch scale 1.35x*) saat mencapai pukulan Rank S atau downbeat ritmis sempurna.

### Setlist Repertoire Deck
* **Moonlighter Stitched Leather Tab (`frame_moonlighter_stitched_tab.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/SetlistDeck/MissionContainer/HeaderDecorRow/MoonlighterTab`
  * **Interaksi:** Tab kulit berserat dengan jahitan benang emas membingkai bagian atas daftar 100 repertoar lagu.
* **Life is Strange: BtS Washi Tape (`tape_lis_washi_strip.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/SetlistDeck/MissionContainer/HeaderDecorRow/WashiTape`
  * **Interaksi:** Selotip washi kuning transparan dengan tepi robek memberikan nuansa catatan panggung manual yang ditempelkan di atas panel.

### Tactical Chord HUD & Teleprompter
* **Taiko no Tatsujin Drum Indicator (`icon_taiko_don_ka_pair.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/ChordHud/TaikoRow/TaikoDonKa` & `TaikoLabel`
  * **Interaksi:**
    * Tuts kiri ($< \text{Split Point C4}$): Label menyala merah `[★ DON! BASS HIT ★]`.
    * Tuts kanan ($\ge \text{Split Point C4}$): Label menyala biru `[★ KA! LEAD HIT ★]`.
* **Stardew Valley Rustic Wood & Parchment**:
  * **Lokasi Aset:** `frame_stardew_rustic_wood.png` & `banner_stardew_parchment.png`
  * **Interaksi:** Membingkai kotak akor aktif dan baris teleprompter lirik dengan sentuhan kayu hangat.
* **Tricky Towers Spell Frame (`card_trickytowers_spell_frame.png`)**:
  * **Lokasi Aset:** Desain dasar kartu akor aktif dengan aksen runik ungu menyala.
* **Snipperclips Pastel Note (`sticker_snipperclips_pastel_note.png`)**:
  * **Lokasi Node:** `$HUD/CenterSplit/ChordHud/LyricBox/VBox/SnipperclipsSticky`
  * **Interaksi:** Sticky note kuning pastel dengan lipatan sudut menampilkan instruksi pengingat solo panggung (*"Don't forget: CHORUS SOLO"*).

---

## 4. Panduan Lengkap Kustomisasi, Edit Template & Update

### Alur Kerja Edit Template Vektor
Seluruh aset induk berupa file vektor `.svg` tersimpan di [`assets/ui_sprites/templates/`](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/assets/ui_sprites/templates/).

#### Menggunakan Figma:
1. Buka Figma di browser atau desktop app.
2. Cukup *drag-and-drop* file template SVG (misal: `badge_hifi_comic_burst.svg`) ke kanvas Figma.
3. Edit warna, font teks, outline, atau efek drop-shadow sesuai selera Anda.
4. Pilih frame, klik **Export** di panel kanan bawah Figma, pilih format **PNG** (skala `@2x` atau `@3x`), dan simpan ke `assets/ui_sprites/png/`.

#### Menggunakan Inkscape (Gratis & Open Source):
1. Buka file template di terminal:
   ```powershell
   inkscape assets/ui_sprites/templates/hud_djmax_fever_gauge.svg
   ```
2. Gunakan *Node Tool* (`F2`) atau *Text Tool* (`F8`) untuk memodifikasi bentuk dan tulisan.
3. Klik **File** ➔ **Export PNG Image** ➔ Pilih resolusi yang diinginkan.

#### Menggunakan VS Code (Edit Kode XML Langsung):
Template dirancang bersih dengan struktur XML standar:
```xml
<!-- Contoh ubah warna neon FEVER di hud_djmax_fever_gauge.svg -->
<linearGradient id="feverGrad">
  <stop offset="0%" stop-color="#00f5d4" />  <!-- Ganti hex color di sini -->
  <stop offset="100%" stop-color="#ff007f" />
</linearGradient>
```

---

### Alur Kerja Regenerasi Otomatis
Jika Anda telah mengedit kode generator atau ingin mengekspor ulang seluruh aset PNG secara serentak dengan kualitas *broadcast* (menggunakan supersampling 2x dan filter Lanczos):

```powershell
# Jalankan generator otomatis Python:
python assets/ui_sprites/scripts/generate_all_sprites.py
```

Output log akan mengonfirmasi pembuatan 22 file SVG dan 18 file PNG:
```text
[Phase 1] Writing Editable Vector SVG Templates...
  [SVG Template] -> badge_hifi_comic_burst.svg
  ...
[Phase 2] Rendering Production-Ready Transparent PNG Sprites...
  [PNG Sprite]   -> badge_hifi_comic_burst.png (256x256)
  ...
ALL 12 GAME UI INSPIRATIONS SUCCESSFULLY GENERATED!
```

---

### Alur Kerja Menambahkan Sprite Baru ke Godot Scene

Jika Anda ingin menambahkan icon/dekorasi baru ke antarmuka Godot (`scenes/MainStageView.tscn`):

1. **Simpan file PNG baru** ke dalam folder `assets/ui_sprites/png/nama_sprite.png`.
2. **Buka file scene di text editor atau Godot Editor**:
   Tambahkan `ExtResource` di header `MainStageView.tscn`:
   ```tscn
   [ext_resource type="Texture2D" path="res://assets/ui_sprites/png/nama_sprite.png" id="17_my_sprite"]
   ```
3. **Tambahkan node TextureRect** pada kontainer yang diinginkan:
   ```tscn
   [node name="MyCustomDecor" type="TextureRect" parent="HUD/TopBar"]
   custom_minimum_size = Vector2(40, 40)
   layout_mode = 2
   texture = ExtResource("17_my_sprite")
   expand_mode = 1
   stretch_mode = 5
   ```
4. **Hubungkan logika interaksi di `src/UI/MainStageController.gd`**:
   ```gdscript
   @onready var my_decor: TextureRect = $HUD/TopBar/MyCustomDecor

   func _process(delta: float) -> void:
       if my_decor:
           # Berdenyut sesuai metronom
           my_decor.scale = Vector2.ONE * (1.0 + beat_pulse * 0.1)
   ```

---

### Alur Kerja Sinkronisasi ke Web Remote Mobile Companion

Mobile remote HTML5 (`web/`) berjalan di server tertanam StageHub (:8080). Untuk menyinkronkan aset sprite baru ke tampilan browser tablet/HP:

1. **Salin aset ke folder web**:
   ```powershell
   Copy-Item -Path "assets/ui_sprites/png/*" -Destination "web/assets/ui_sprites/png" -Force
   ```
2. **Gunakan tag `<img>` di `web/index.html`**:
   ```html
   <img src="assets/ui_sprites/png/nama_sprite.png" class="my-custom-class" alt="My Decor" />
   ```
3. **Atur tata letak di `web/style.css`**:
   ```css
   .my-custom-class {
     height: 28px;
     object-fit: contain;
     transition: transform 0.1s ease;
   }
   ```
4. **Animasikan di `web/app.js`**:
   Di dalam fungsi `handlePlaybackTick(data)`:
   ```javascript
   if (data.beat === 1) {
     const elem = document.querySelector(".my-custom-class");
     if (elem) {
       elem.classList.add("pulse");
       setTimeout(() => elem.classList.remove("pulse"), 120);
     }
   }
   ```

---

## 5. Tuning Parameter Shader & Animasi

Di file `src/UI/MainStageController.gd`, Anda dapat menyesuaikan sensitivitas animasi visual:

| Parameter di Script | Nilai Default | Efek Penyesuaian |
| :--- | :---: | :--- |
| `beat_pulse` decay rate | `delta * 2.5` | Tingkatkan ke `3.5` untuk denyutan lebih tajam, atau turunkan ke `1.5` untuk denyutan lembut mengayun. |
| `hit_beam_flash` decay | `delta * 3.0` | Mengatur durasi kilauan sinar laser pada saat tuts ditekan. |
| `fever_energy` charge rate | `0.07 * v_norm` | Tingkatkan ke `0.15` jika ingin bar Fever terisi lebih cepat saat solo cepat. |
| `fever_energy` drain rate | `delta * 0.04` | Mengatur kecepatan penurunan bar Fever jika pemain berhenti menekan tuts. |
| `burst_scale` punch | `1.35` | Skala lonjakan badge komik Hi-Fi Rush saat velocity $\ge 110$. |
| `screen_shake` intensity | `6.0` | Besaran guncangan layar panggung saat pukulan keras fortissimo. Set ke `0.0` jika menginginkan kamera statis. |

---

## 6. Inventaris File Aset & Lisensi CC0

Semua aset visual dan template dalam suite ini dirilis di bawah lisensi:
**Creative Commons CC0 1.0 Universal (Public Domain Dedication)**.

Anda bebas menggunakan, memodifikasi, mendistribusikan, dan memadukan seluruh aset ini ke dalam konser panggung komersial, rekaman studio, video live session, maupun proyek derivatif tanpa kewajiban atribusi atau royalti.

| File Vektor (`templates/`) | File Sprite (`png/`) | Resolusi | Game Inspirasi |
| :--- | :--- | :---: | :--- |
| `badge_hifi_comic_burst.svg` | `badge_hifi_comic_burst.png` | 256x256 | Hi-Fi Rush |
| `badge_hifi_rank_sss.svg` | `badge_hifi_rank_sss.png` | 300x160 | Hi-Fi Rush |
| `hud_djmax_fever_gauge.svg` | `hud_djmax_fever_gauge.png` | 400x60 | DJMAX RESPECT V |
| `badge_djmax_judgement_max.svg` | `badge_djmax_judgement_max.png` | 280x60 | DJMAX RESPECT V |
| `ornament_nostalgia_gilded_corner.svg` | `ornament_nostalgia_gilded_corner.png` | 128x128 | NOSTALGIA Op. 2 |
| `stamp_nostalgia_wax_seal.svg` | `stamp_nostalgia_wax_seal.png` | 180x180 | NOSTALGIA Op. 2 |
| `icon_gris_celestial_star.svg` | `icon_gris_celestial_star.png` | 128x128 | GRIS |
| `overlay_gris_zen_vignette.svg` | `overlay_gris_zen_vignette.png` | 480x270 | GRIS |
| `frame_moonlighter_stitched_tab.svg` | `frame_moonlighter_stitched_tab.png` | 240x64 | Moonlighter |
| `badge_moonlighter_gold_pouch.svg` | `badge_moonlighter_gold_pouch.svg` | 90x90 | Moonlighter |
| `frame_stardew_rustic_wood.svg` | `frame_stardew_rustic_wood.png` | 100x100 | Stardew Valley |
| `banner_stardew_parchment.svg` | `banner_stardew_parchment.png` | 340x80 | Stardew Valley |
| `tape_lis_washi_strip.svg` | `tape_lis_washi_strip.png` | 200x46 | Life is Strange: BtS |
| `frame_lis_polaroid_badge.svg` | `frame_lis_polaroid_badge.svg` | 180x220 | Life is Strange: BtS |
| `icon_taiko_don_ka_pair.svg` | `icon_taiko_don_ka_pair.png` | 220x100 | Taiko no Tatsujin |
| `badge_taiko_combo_firework.svg` | `badge_taiko_combo_firework.svg` | 180x180 | Taiko no Tatsujin |
| `hud_cadence_heart_pulse.svg` | `hud_cadence_heart_pulse.png` | 300x60 | Cadence of Hyrule |
| `bracket_cadence_triforce_corner.svg` | `bracket_cadence_triforce_corner.svg` | 64x64 | Cadence of Hyrule |
| `widget_minimetro_transit_map.svg` | `widget_minimetro_transit_map.png` | 460x70 | Mini Metro |
| `card_trickytowers_spell_frame.svg` | `card_trickytowers_spell_frame.png` | 180x240 | Tricky Towers |
| `tag_snipperclips_paper_cutout.svg` | `tag_snipperclips_paper_cutout.png` | 220x70 | Snipperclips |
| `sticker_snipperclips_pastel_note.svg` | `sticker_snipperclips_pastel_note.png` | 160x160 | Snipperclips |


---

## 7. Transformasi Sweet & Charming Interactive RPG Game Suite

Berdasarkan benchmark diegetik game RPG favorit:
- **Moonlighter Notebook & Staff Forge** (*11 bit studios*): Jurnal pedagang bersampul kulit jahitan tangan, kawat spiral ring, bookmark tab berwarna-warni, serta stempel ekspresi emosional pedagang.
- **Animal Crossing: New Horizons** (*Nintendo*): Kanopi awan bergelombang (*scalloped cloud header*), kartu resep DIY dengan watermark butir biji pohon (*acorn watermark*), dan slot stempel Nook Miles.
- **Assassin's Creed Mirage Codex**: Gulungan perkamen archival kuno bertengger pada kedua ujung tongkat kayu untuk teleprompter lirik panggung.
- **Gothic Fantasy & Tactical RPG**: Kartu mantra runik emas-ungu (*spell card*) untuk bingkai akor dan kristal reservoar bar mana untuk indikator Fever/Velocity.

Semua tampilan visual flat generic telah direvisi secara menyeluruh pada ketiga platform panggung:

```
[ AnantaNada Tri-Platform Sweet RPG Experience ]
├── 1. Desktop Stage View (Godot 4 Forward+ Vulkan 1920x1080)
│   ├── Meja Kayu Oak Hangat (desk_wood_surface.png)
│   ├── Jurnal Setlist Kulit & Kawat Spiral (notebook_page_parchment.png & spiral_wire_binding.png)
│   ├── Tab Bookmark Mint, Honey, & Berry (tab_mint.png, tab_honey.png, tab_berry.png)
│   ├── Kartu Resep DIY Animal Crossing (card_ac_recipe_item.png) dengan Star Pin Badge (badge_star_pin.png)
│   ├── Kanopi Awan Bergelombang (scalloped_cloud_banner.png) di atas Air Terjun Waterfall
│   ├── Coach Deck: Reservoar Mana Kristal (gauge_rpg_mana_bar.png) & Stempel Cheer Smiley (emoji_reaction_cheer.png)
│   ├── Arcane Spell Card Runic (card_rpg_spell_harmony.png) membingkai Akor Aktif
│   └── Gulungan Perkamen Mirage (parchment_codex_scroll.png) untuk Teleprompter Lirik
│
├── 2. Web Remote Dashboard (1280x820 Desktop / Tablet)
│   ├── Header Kayu Mahoni dengan Koin Emas Pedagang (badge_gold_coin_rpg.png)
│   ├── Grid 3 Kolom Taktil Bertekstur RPG Diegetik Penuh
│   └── Animasi CSS Heart Pulse Cadence of Hyrule & Comic Burst Hi-Fi Rush
│
└── 3. Android Companion App (430x932 Mobile Portrait)
    ├── Tata Letak Vertikal Responsif dengan Kontras Tinggi untuk Panggung Terang/Gelap
    ├── Kartu Resep DIY yang Luas dan Nyaman Disentuh Jempol
    └── Floating Mahogany Footer dengan Pemisah Split Point Tuts Roland Go:Keys 3
```

### 7.1 Daftar Aset RPG Baru (`assets/rpg_ui/sprites/`)

| Nama File Sprite | Resolusi | Karakter Visual | Game Inspirasi |
| :--- | :---: | :--- | :--- |
| `desk_wood_surface.png` | 1920x1080 | Permukaan kayu oak hangat dengan serat kayu alami dan pencahayaan studio | Moonlighter Forge / Cozy RPG |
| `notebook_page_parchment.png` | 380x560 | Jurnal perkamen tepi lusuh dengan sampul kulit jahitan benang dan pita merah | Moonlighter Notebook |
| `spiral_wire_binding.png` | 30x480 | Ring kawat spiral baja berkilau untuk jilid buku lagu | Moonlighter Ledger |
| `scalloped_cloud_banner.png` | 600x64 | Kanopi awan bergelombang pastel cream-putih | Animal Crossing: New Horizons |
| `tab_mint.png`, `tab_honey.png`, `tab_berry.png`, `tab_sky.png` | 84x46 | Tab bookmark penanda kategori setlist (Indo, Barat, Jazz, Religi) | Moonlighter / Stardew Valley |
| `card_ac_recipe_item.png` | 320x56 | Kartu resep DIY bertepi melengkung dengan watermark dots | Animal Crossing: New Horizons |
| `card_ac_stamp_slot.png` | 260x72 | Kartu slot cap stempel tantangan Nook Miles | Animal Crossing Achievements |
| `emoji_reaction_happy.png`, `emoji_reaction_cheer.png`, `neutral`, `sweat` | 64x64 | Cap stempel tinta bulat dengan ekspresi emosi wajah imut | Moonlighter Reactions |
| `badge_gold_coin_rpg.png` | 54x54 | Koin emas pedagang berkilau dengan emboss bintang | Moonlighter Merchant Sovereign |
| `badge_star_pin.png` | 48x48 | Pin bintang logam emas untuk lagu favorit panggung | Animal Crossing Favorites |
| `gauge_rpg_mana_bar.png` | 380x48 | Indikator reservoar kristal mana RPG bercahaya cyan-emas | Tactical Fantasy RPG |
| `parchment_codex_scroll.png` | 340x140 | Gulungan perkamen kuno dengan rol kayu berulir di atas dan bawah | Assassin's Creed Mirage Codex |
| `card_rpg_spell_harmony.png` | 320x120 | Kartu mantra runik gothic ungu gelap berbingkai emas bercahaya | Gothic RPG Grimoire |

### 7.2 Cara Edit, Regenerasi, dan Mengganti Sprite

1. **Mengubah Bentuk / Warna / Detail Sprite**:
   - Skrip generator otomatis tersedia di `assets/rpg_ui/generate_rpg_ui_suite.py`.
   - Menggunakan Pillow dengan antialiasing supersampling 4x ($s=4$). Anda dapat mengubah nilai RGB, radius sudut, ketebalan outline, atau menambah ornamen baru pada fungsi masing-masing (misal `generate_desk_wood()`, `generate_spell_card()`).
   - Jalankan regenerasi:
     ```bash
     python assets/rpg_ui/generate_rpg_ui_suite.py
     ```
   - Skrip secara otomatis menimpa file di `assets/rpg_ui/sprites/`, lalu salin ke `assets/ui_sprites/png/` dan `web/assets/ui_sprites/png/`.

2. **Memperbarui Indeks Impor Godot 4**:
   - Setelah menambahkan atau memperbarui file PNG, jalankan Godot editor headless agar cache tekstur `.ctex` di `.godot/imported/` diperbarui:
     ```bash
     C:\Godot\Godot_v4.5-stable_win64_console.exe --editor --headless --quit
     ```

3. **Memverifikasi Tampilan Screenshot Tri-Platform**:
   - Desktop Godot:
     ```bash
     C:\Godot\Godot_v4.5-stable_win64_console.exe scenes/ScreenshotGrabber.tscn
     ```
     Hasil tangkapan layar tersimpan di `captures/screenshot_godot_stage.png`.
   - Web Remote & Android Companion:
     ```bash
     python scripts/capture_web_screens.py
     ```
     Hasil tangkapan layar tersimpan di `captures/screenshot_web_remote.png` dan `captures/screenshot_android_companion.png`.

---

## 8. Verifikasi Tangkapan Layar Tri-Platform

| Platform | Resolusi Tangkapan | File Bukti Screenshot |
| :--- | :---: | :--- |
| **Desktop Stage View (Godot 4)** | 1920 x 1080 (Vulkan Forward+) | `captures/screenshot_godot_stage.png` |
| **Web Remote Control (Browser)** | 1280 x 820 (Desktop / Tablet) | `captures/screenshot_web_remote.png` |
| **Android Companion App** | 430 x 932 (Mobile Portrait) | `captures/screenshot_android_companion.png` |
