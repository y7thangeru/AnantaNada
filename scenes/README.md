# Godot 4 Diegetic Stage Scene Hierarchy (`scenes/`)

### Stage Console Scene Layout & 3-Column Sweet RPG Atelier Structure

> **Engine:** Godot Engine 4.x (.NET & GDScript Bridge)  
> **Target Display Resolution:** Full HD ($1920 \times 1080$), Responsive Scaling (`canvas_items`, `keep_height`)  
> **Root Scene File:** `res://scenes/MainStageView.tscn`  
> **Attached Controller:** `src/UI/MainStageController.cs` (C#) / `src/UI/MainStageController.gd` (GDScript Bridge)  
> **Visual Showcase Artifacts:** `captures/screenshot_godot_stage.png`, `captures/screenshot_web_remote.png`, `captures/screenshot_android_companion.png`, `captures/ui_setlist_deck.png`, `captures/ui_waterfall_coach.png`, `captures/ui_chord_hud.png`, `captures/ui_keybed_61.png`

---

## 1. Visual Cockpit Architecture

The scene is designed as a diegetic gaming cockpit combining **Animal Crossing** DIY recipe cards & cloud canopy, **Stardew Valley** rustic wood joinery, **Tricky Towers** runic spell card frames, and **Hi-Fi Rush / DJMAX** rhythmic mana feedback.

![Godot Stage HUD](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/screenshot_godot_stage.png)

```mermaid
graph TD
    Root["MainStageView (Control, MainStageController)"]
    Bg["DeskWoodSurface (TextureRect - Warm Oak Wood)"]
    ZenOverlay["GrisZenVignette (TextureRect - Minimalist Vignette)"]
    HUD["HUD (VBoxContainer - Full Margin 20px)"]

    TopBar["TopBar (HBoxContainer, 52px)"]
    BrandPill["BrandPill (Star Pin + '// SYS_STATUS: OPERATIONAL')"]
    LinkPill["LinkStatusPill ('> LINK: ROLAND GO:KEYS 3 [ONLINE]')"]
    MemPill["MemStatusPill ('> MEMORY: IN-RAM SAFE')"]
    BpmPill["BpmPill ('[BPM: 128 /// 4/4] [BEAT: 03/16]')"]
    GoldCoin["GoldCoinRect (Moonlighter Sovereign Icon)"]
    PanicBtn["PanicButton ('[ ! PANIC PURGE ! ]')"]

    CenterSplit["CenterSplit (HBoxContainer, Flex Expand)"]

    subgraph Col1["Column 1: SetlistDeck (Notebook Parchment, 360px)"]
        SpiralWire["SpiralWireBinding (Silver Ring Wire)"]
        TabRow["BookmarkTabs: [ ALL ] [ INDO ] [ WEST ] [ FAV ]"]
        SearchBox["SearchBox (Animal Crossing Style LineEdit)"]
        ActiveCard["ActiveMissionCard (Curled Card with Gold Star)"]
        NextCards["NextCards 1-6 (DIY Craft Recipe Cards)"]
        Pagination["PaginationBar: [ < PREV ] [ PAGE X/Y ] [ NEXT > ]"]
    end

    subgraph Col2["Column 2: CenterCascade (Waterfall & Mana Coach, Flex)"]
        CloudCanopy["ScallopedCloudBanner (Animal Crossing Cloud)"]
        TransitMap["MiniMetroTransitMap (Song Section Progression Route)"]
        WaterfallRect["WaterfallRect (waterfall_visualizer.gdshader, 18° Runway)"]
        NostalgiaCorners["Baroque Gilded Corner Brackets (NOSTALGIA Op. 2)"]
        CoachDeck["CoachDeck: TimingVerdict + ManaBar + EmojiReactions"]
    end

    subgraph Col3["Column 3: ChordHud (Gothic Rune & Codex Scroll, 380px)"]
        SpellCard["GothicRuneSpellCard (48pt Chord + Transpose Indicator)"]
        TaikoDonKa["TaikoDonKaPair (Don Left Red / Ka Right Blue)"]
        RecipeNext["RecipeCardNextChord (Next Chord -> C#m7)"]
        CodexScroll["CodexParchmentScroll (Bar-Synchronized Lyric Prompter)"]
    end

    subgraph Bottom["BottomKeybed (Keybed & Controls, 160px)"]
        ZoneRow["ZoneRow: Zone A (Amber) | [ SPLIT POINT: C4 ] | Zone B (Cyan)"]
        KeybedContainer["KeybedContainer (piano_keybed.gdshader, 36 White, 25 Black 3D)"]
        ControlsBar["ControlsBar: Wax Seal + '⚡ AMBIL IDE TADI (5-MIN RETROACTIVE)'"]
    end

    Root --> Bg
    Root --> ZenOverlay
    Root --> HUD
    HUD --> TopBar
    TopBar --> BrandPill
    TopBar --> LinkPill
    TopBar --> MemPill
    TopBar --> BpmPill
    TopBar --> GoldCoin
    TopBar --> PanicBtn

    HUD --> CenterSplit
    CenterSplit --> Col1
    CenterSplit --> Col2
    CenterSplit --> Col3

    HUD --> Bottom
```

---

## 2. Component Visual Breakdown

### 2.1. Column 1: Setlist Codex Deck (Spiral Notebook & Recipe Cards)
![Setlist Deck](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/ui_setlist_deck.png)
- **Node:** `HUD/CenterSplit/SetlistDeck`
- **Textures:** `assets/rpg_ui/sprites/notebook_page_parchment.png`, `assets/rpg_ui/sprites/spiral_wire_binding.png`, `assets/rpg_ui/sprites/card_ac_recipe_item.png`.
- **Features:** Bookmark tabs pastel (`[ ALL ]`, `[ INDO ]`, `[ WEST ]`), kotak pencarian instan, dan 100 kartu lagu resep DIY.

### 2.2. Column 2: Cyber-Cascade Waterfall & Performance Coach Deck
![Waterfall & Coach](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/ui_waterfall_coach.png)
- **Node:** `HUD/CenterSplit/CenterCascade`
- **Textures & Shaders:** `scalloped_cloud_banner.png`, `widget_minimetro_transit_map.png`, `gauge_rpg_mana_bar.png`, `emoji_reaction_happy.png`, `waterfall_visualizer.gdshader`.
- **Features:** Hiasan kanopi awan, peta transit seksi lagu, runway 3D 18° dengan hit beam bloom, meteran akurasi metronom, dan reservoar mana kristal.

### 2.3. Column 3: Runic Harmony HUD & Codex Lyric Scroll
![Tactical Chord HUD](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/ui_chord_hud.png)
- **Node:** `HUD/CenterSplit/ChordHud`
- **Textures:** `card_rpg_spell_harmony.png`, `icon_taiko_don_ka_pair.png`, `card_ac_recipe_item.png`, `parchment_codex_scroll.png`.
- **Features:** Kartu mantra batu runik menaungi akord aktif 48pt, indikator drum Taiko Don/Ka, kartu resep akord berikutnya, dan gulungan perkamen naskah kuno lirik.

### 2.4. Bottom Bar: Synth-Runner 61-Key Keybed & Never-Lost Melody
![61-Key Keybed](file:///d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/ui_keybed_61.png)
- **Node:** `HUD/BottomKeybed`
- **Shader:** `src/Shaders/piano_keybed.gdshader` (36 tuts putih gading, 25 tuts hitam 3D terangkat, garis pemisah Split Point C4 merah, dual neon Amber/Cyan glow).
- **Controls:** Tombol ekspor rekaman 5 menit *"⚡ AMBIL IDE TADI"* dengan stempel segel lilin merah (*wax seal*).

---

## 3. Responsive Scaling & Projection
In `project.godot`:
- Viewport Size: $1920 \times 1080$ Full HD.
- Stretch Mode: `canvas_items`, Aspect: `keep_height`.
- Rendering Driver: Forward+ (Vulkan) / Compatibility.
