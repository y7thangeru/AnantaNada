# Godot 4 Diegetic Stage Scene Hierarchy (`scenes/`)

### Stage Console Scene Layout & Visual Component Structure

> **Engine:** Godot Engine 4.x (.NET Edition)  
> **Target Display Resolution:** Full HD ($1920 \times 1080$), Responsive Scaling (`canvas_items`, `keep_height`)  
> **Root Scene File:** `res://scenes/MainStageView.tscn`  
> **Attached Controller:** `src/UI/MainStageController.cs`

---

## 1. Scene Graph & Node Hierarchy

The main stage view is structured as a layered diegetic HUD inspired by *Persona 5* slanted kinetic cards, *Arknights* tactical HUD elements, and *Hi-Fi Rush* rhythmic meters.

```mermaid
graph TD
    Root["MainStageView (Control, MainStageController.cs)"]
    Bg["Background (ColorRect - #0b0d10)"]
    HUD["HUD (VBoxContainer - Full Margin 24px)"]

    TopBar["TopBar (HBoxContainer, 64px)"]
    BrandPill["BrandPill (PanelContainer: 'ANANTANADA // GO:KEYS 3')"]
    DevicePill["DevicePill (StatusLabel: MIDI Hardware Status)"]
    NetworkPill["NetworkPill (IpLabel: StageHub LAN IP)"]
    BpmPill["BpmPill (BpmLabel: 128 BPM / Metronome)"]
    PanicBtn["PanicButton (Button: High-Contrast Crimson Failsafe)"]

    CenterPanel["CenterPanel (HBoxContainer, Flex Expand)"]
    Teleprompter["Teleprompter (VBoxContainer, 380px)"]
    SongCard["SongCard (SongTitle, PatchName, KeyBadge)"]
    ChordDisplay["ChordDisplay (CurrentChord: 72pt, NextChord: 32pt)"]
    BarBeat["BarBeatLabel (Bar 12 // Beat 3)"]

    WaterfallContainer["WaterfallContainer (PanelContainer, Flex 1)"]
    WaterfallRect["WaterfallRect (ColorRect with waterfall_visualizer.gdshader)"]

    BottomBar["BottomBar (VBoxContainer, 230px)"]
    KeybedContainer["KeybedContainer (PanelContainer, 170px)"]
    KeybedRect["KeybedRect (ColorRect with piano_keybed.gdshader)"]
    ButtonBar["ButtonBar (HBoxContainer, 44px)"]
    SaveBtn["NeverLostMelodyBtn ('⚡ AMBIL IDE TADI (5-MIN RETROACTIVE)')"]

    Root --> Bg
    Root --> HUD
    HUD --> TopBar
    TopBar --> BrandPill
    TopBar --> DevicePill
    TopBar --> NetworkPill
    TopBar --> BpmPill
    TopBar --> PanicBtn

    HUD --> CenterPanel
    CenterPanel --> Teleprompter
    Teleprompter --> SongCard
    Teleprompter --> ChordDisplay
    Teleprompter --> BarBeat
    CenterPanel --> WaterfallContainer
    WaterfallContainer --> WaterfallRect

    HUD --> BottomBar
    BottomBar --> KeybedContainer
    KeybedContainer --> KeybedRect
    BottomBar --> ButtonBar
    ButtonBar --> SaveBtn
```

---

## 2. Visual Style & Color Design System

| Color Token | Hex Code | Purpose |
| :--- | :--- | :--- |
| **Deep Obsidian** | `#0b0d10` | Stage background canvas backing. |
| **Panel Charcoal** | `#161920` | Secondary container frames and cards. |
| **Electric Amber** | `#ffb703` | Left-hand key splits, metronome ticks, warm accents. |
| **Cyber Cyan** | `#00f5d4` | Right-hand key splits, strike beam, active chords. |
| **Warning Crimson**| `#ef233c` | Emergency hardware Panic button. |
| **Ivory White** | `#e5e8f0` | High-contrast readable typography for stage viewing. |

---

## 3. Responsive Scaling & Projection

In `project.godot`:
- `display/window/size/viewport_width = 1920`
- `display/window/size/viewport_height = 1080`
- `display/window/stretch/mode = "canvas_items"`
- `display/window/stretch/aspect = "keep_height"`

This configuration ensures crisp 2D shader execution and legible typography regardless of whether the console is projected on a high-DPI laptop screen, a $4\text{K}$ stage monitor, or an ultrawide performance display.
