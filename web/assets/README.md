# ANANTANADA UI Sprites & Design Templates
### 12-Game Master UI/UX Archetype Suite (CC0 Public Domain)

Direktori ini berisi koleksi aset visual dan template vektor siap pakai yang diekstraksi dari **12 game berperingkat tinggi di [Interface In Game](https://interfaceingame.com/)**. Seluruh aset dirancang khusus untuk memperkaya konsol panggung pementasan langsung (*live stage performance console*) Roland Go:Keys 3 pada sistem **ANANTANADA**.

---

## 📁 Struktur Direktori

```
assets/ui_sprites/
├── templates/                     # 22 Editable Vector SVG Templates (Figma, Inkscape, Illustrator)
│   ├── badge_hifi_comic_burst.svg
│   ├── badge_hifi_rank_sss.svg
│   ├── hud_djmax_fever_gauge.svg
│   ├── badge_djmax_judgement_max.svg
│   ├── ornament_nostalgia_gilded_corner.svg
│   ├── stamp_nostalgia_wax_seal.svg
│   ├── icon_gris_celestial_star.svg
│   ├── overlay_gris_zen_vignette.svg
│   ├── frame_moonlighter_stitched_tab.svg
│   ├── badge_moonlighter_gold_pouch.svg
│   ├── frame_stardew_rustic_wood.svg
│   ├── banner_stardew_parchment.svg
│   ├── tape_lis_washi_strip.svg
│   ├── frame_lis_polaroid_badge.svg
│   ├── icon_taiko_don_ka_pair.svg
│   ├── badge_taiko_combo_firework.svg
│   ├── hud_cadence_heart_pulse.svg
│   ├── bracket_cadence_triforce_corner.svg
│   ├── widget_minimetro_transit_map.svg
│   ├── card_trickytowers_spell_frame.svg
│   ├── tag_snipperclips_paper_cutout.svg
│   └── sticker_snipperclips_pastel_note.svg
│
├── png/                           # Production-Ready Transparent PNG Sprites (Godot 4 & Web Remote)
│   ├── badge_hifi_comic_burst.png       (256x256, Pop-Art Starburst Accent)
│   ├── badge_hifi_rank_sss.png          (300x160, Slanted Action Rank Badge)
│   ├── hud_djmax_fever_gauge.png        (400x60, Segmented Neon FEVER Bar)
│   ├── badge_djmax_judgement_max.png    (280x60, 100% MAX BEAT Precision Badge)
│   ├── ornament_nostalgia_gilded_corner.png (128x128, Baroque Gilded Filigree)
│   ├── stamp_nostalgia_wax_seal.png     (180x180, Concert Hall Crimson Wax Seal)
│   ├── icon_gris_celestial_star.png     (128x128, Minimalist Zen Star Icon)
│   ├── frame_moonlighter_stitched_tab.png (240x64, Stitched Leather Ledger Tab)
│   ├── frame_stardew_rustic_wood.png    (100x100, Rustic Carved Wood Corner)
│   ├── tape_lis_washi_strip.png         (200x46, Translucent Yellow Washi Tape)
│   ├── icon_taiko_don_ka_pair.png       (220x100, Don Red & Ka Blue Drum Pair)
│   ├── hud_cadence_heart_pulse.png      (300x60, 16-Bit Heart Beat Bar)
│   ├── widget_minimetro_transit_map.png (460x70, Transit Route Progression Map)
│   ├── card_trickytowers_spell_frame.png (180x240, Gothic Masonry Spell Card)
│   ├── tag_snipperclips_paper_cutout.png (220x70, Perforated Papercraft Tag)
│   └── sticker_snipperclips_pastel_note.png (160x160, Curled Pastel Sticky Note)
│
└── scripts/
    └── generate_all_sprites.py    # Generator otomatis Python (Pillow + Vector Engine)
```

---

## 🎮 Pemetaan Inspirasi Desain dari 12 Game

| No | Game Asal | Aset Vektor & Sprite | Peran UI/UX dalam ANANTANADA |
| :-: | :--- | :--- | :--- |
| **1** | **Hi-Fi Rush** | `badge_hifi_comic_burst`<br>`badge_hifi_rank_sss` | Aksen visual komik pop-art ketika pukulan tuts piano mencapai tempo akurat (S/SSS Rank feedback). |
| **2** | **DJMAX RESPECT V** | `hud_djmax_fever_gauge`<br>`badge_djmax_judgement_max` | Bar *FEVER x5* pengukur intensitas permainan panggung & badge akurasi *100% MAX BEAT*. |
| **3** | **NOSTALGIA Op. 2** | `ornament_nostalgia_gilded_corner`<br>`stamp_nostalgia_wax_seal` | Ornamen sudut barok berlapis emas untuk frame tuts panggung piano & stempel segel lilin kualitas konser. |
| **4** | **GRIS** | `icon_gris_celestial_star`<br>`overlay_gris_zen_vignette` | Ikon bintang konstelasi minimalis untuk tombol peralihan mode fokus panggung hening (*Zen Mode*). |
| **5** | **Moonlighter** | `frame_moonlighter_stitched_tab`<br>`badge_moonlighter_gold_pouch` | Tab buku jurnal bersampul kulit untuk mengelompokkan katalog setlist lagu dan daftar nomor lagu. |
| **6** | **Stardew Valley** | `frame_stardew_rustic_wood`<br>`banner_stardew_parchment` | Bingkai kayu rustic hangat pada modul chord serta banner perkamen emas untuk teleprompter lirik. |
| **7** | **Life is Strange: BtS** | `tape_lis_washi_strip`<br>`frame_lis_polaroid_badge` | Potongan selotip washi kuning semi-transparan untuk menempelkan catatan panggung / stiker chord dadakan. |
| **8** | **Taiko no Tatsujin** | `icon_taiko_don_ka_pair`<br>`badge_taiko_combo_firework` | Indikator lingkaran ritme Merah Don (tangan kiri / bass chord) dan Biru Ka (tangan kanan / melody riff). |
| **9** | **Cadence of Hyrule** | `hud_cadence_heart_pulse`<br>`bracket_cadence_triforce_corner` | Bar detak jantung piksel 16-bit berdenyut selaras dengan BPM lagu di bagian atas konsol. |
| **10**| **Mini Metro** | `widget_minimetro_transit_map` | Diagram skema rute stasiun kereta untuk melacak struktur lagu (`Intro ◯` ➔ `Verse △` ➔ `Chorus ▢` ➔ `Solo ◇` ➔ `Outro ◯`). |
| **11**| **Tricky Towers** | `card_trickytowers_spell_frame` | Kartu mantra berbingkai batu runik ungu menyala untuk membingkai akor aktif yang sedang dimainkan. |
| **12**| **Snipperclips** | `tag_snipperclips_paper_cutout`<br>`sticker_snipperclips_pastel_note` | Tag kertas bergaris putus-putus dan sticky note kuning pastel untuk catatan panggung fleksibel. |

---

## 🛠️ Cara Mengedit & Mengupdate Template

1. **Buka file SVG** di dalam folder `assets/ui_sprites/templates/` menggunakan aplikasi vektor favorit Anda:
   * **Figma** (Cukup *drag-and-drop* file `.svg` ke kanvas Figma)
   * **Inkscape** (Gratis & Open Source: `inkscape template.svg`)
   * **Adobe Illustrator** / **CorelDraw**
   * **VS Code** (Bisa diedit langsung kode XML-nya)
2. Lakukan kustomisasi (ubah warna hex, teks, bentuk, atau ketebalan garis sesuai kebutuhan panggung Anda).
3. Simpan perubahan ke file `.svg` atau ekspor langsung sebagai `.png` ke folder `assets/ui_sprites/png/`.
4. Untuk meregenerasi seluruh sprite secara otomatis dengan supersampling anti-aliasing:
   ```powershell
   python assets/ui_sprites/scripts/generate_all_sprites.py
   ```

---

## 📜 Lisensi & Atribusi Bebas (Creative Commons CC0)
Semua template vektor dan sprite dalam paket ini dirilis di bawah lisensi **Creative Commons CC0 1.0 Universal (Public Domain Dedication)**. Bebas digunakan, dimodifikasi, dan didistribusikan untuk kebutuhan panggung komersial maupun pribadi tanpa kewajiban atribusi.
