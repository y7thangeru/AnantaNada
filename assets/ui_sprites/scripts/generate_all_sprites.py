"""
=============================================================================
ANANTANADA UI SPRITE & TEMPLATE ENGINE
Procedural Vector SVG & Pixel-Perfect PNG Generator
=============================================================================
Curated UI/UX Inspirations from 12 Industry Masters:
1.  Hi-Fi Rush                  (Comic Pop-Art Burst & Rank Badges)
2.  DJMAX RESPECT V             (Cyber-Arcade FEVER Gauge & Judgement Badges)
3.  NOSTALGIA Op. 2             (Classical Baroque Gilded Corners & Wax Seal)
4.  GRIS                        (Minimalist Watercolor Vignette & Celestial Stars)
5.  Moonlighter                 (Stitched Leather Tabs & Merchant Ledger Pouch)
6.  Stardew Valley              (Rustic Wood Trim & Golden Parchment Banners)
7.  Life is Strange: BtS        (Translucent Washi Tape & Polaroid Frame)
8.  Taiko no Tatsujin           (Don/Ka Rhythm Markers & Festive Firework Burst)
9.  Cadence of Hyrule           (16-Bit Pulsing Heart Beat Bar & Pixel Brackets)
10. Mini Metro                  (Transit Schematic Song Progression Map & Nodes)
11. Tricky Towers               (Gothic Masonry Spell Card & Waterline Gauge)
12. Snipperclips                (Papercraft Cutout Tags & Pastel Sticky Notes)

Licensing: 100% Free / Creative Commons CC0 (Public Domain Dedication)
"""

import os
import math
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont, ImageFilter

SCRIPT_DIR = Path(__file__).resolve().parent
ROOT_DIR = SCRIPT_DIR.parent
TEMPLATES_DIR = ROOT_DIR / "templates"
PNG_DIR = ROOT_DIR / "png"

TEMPLATES_DIR.mkdir(parents=True, exist_ok=True)
PNG_DIR.mkdir(parents=True, exist_ok=True)

# ---------------------------------------------------------------------------
# SVG GENERATION UTILITIES
# ---------------------------------------------------------------------------

def write_svg(filename: str, width: int, height: int, content: str):
    filepath = TEMPLATES_DIR / filename
    svg_code = f'''<?xml version="1.0" encoding="UTF-8"?>
<!-- 
  ANANTANADA UI SPRITE TEMPLATE
  Editable in Inkscape, Figma, Illustrator, or any vector editor.
  License: CC0 Public Domain
-->
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {width} {height}" width="{width}" height="{height}">
  <defs>
    <style>
      .bold-text {{ font-family: 'Arial Black', Impact, sans-serif; font-weight: bold; }}
      .serif-text {{ font-family: 'Georgia', 'Times New Roman', serif; }}
      .mono-text {{ font-family: 'Courier New', monospace; font-weight: bold; }}
      .hand-text {{ font-family: 'Comic Sans MS', 'Chalkboard', cursive; font-weight: bold; }}
    </style>
  </defs>
{content}
</svg>
'''
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(svg_code)
    print(f"  [SVG Template] -> {filename}")

# ---------------------------------------------------------------------------
# 1. HI-FI RUSH (Pop-Art Comic Burst & Slanted Rank Badge)
# ---------------------------------------------------------------------------

def generate_hifi_rush():
    # SVG 1: Comic Starburst Badge
    svg_burst = '''
  <defs>
    <radialGradient id="burstGrad" cx="50%" cy="50%" r="50%">
      <stop offset="0%" stop-color="#fff200" />
      <stop offset="70%" stop-color="#ff0055" />
      <stop offset="100%" stop-color="#111116" />
    </radialGradient>
    <filter id="shadow">
      <feDropShadow dx="4" dy="4" stdDeviation="0" flood-color="#000000" />
    </filter>
  </defs>
  <!-- Comic Starburst Rays -->
  <polygon points="128,10 155,60 215,30 195,85 250,105 200,140 240,195 180,190 170,245 128,210 86,245 76,190 16,195 56,140 6,105 61,85 41,30 101,60" 
           fill="url(#burstGrad)" stroke="#111116" stroke-width="8" filter="url(#shadow)" stroke-linejoin="miter" />
  <polygon points="128,28 149,70 195,45 180,90 225,108 185,138 215,180 170,175 160,218 128,190 96,218 86,175 41,180 71,138 31,108 76,90 61,45 107,70" 
           fill="#ffee00" stroke="#ff0055" stroke-width="3" />
  <!-- Center Text Badge -->
  <ellipse cx="128" cy="128" rx="60" ry="38" fill="#111116" transform="rotate(-6 128 128)" />
  <ellipse cx="128" cy="128" rx="55" ry="33" fill="#00f5d4" transform="rotate(-6 128 128)" />
  <text x="128" y="136" text-anchor="middle" font-size="28" fill="#111116" class="bold-text" transform="rotate(-6 128 128)">PERFECT!</text>
'''
    write_svg("badge_hifi_comic_burst.svg", 256, 256, svg_burst)

    # SVG 2: Slanted Rank SSS Badge
    svg_rank = '''
  <defs>
    <linearGradient id="rankGrad" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#ff0055" />
      <stop offset="50%" stop-color="#ff5400" />
      <stop offset="100%" stop-color="#ffbd00" />
    </linearGradient>
  </defs>
  <!-- Slanted Background -->
  <polygon points="10,60 270,10 290,140 30,190" fill="#111116" stroke="#00f5d4" stroke-width="6" />
  <polygon points="16,66 264,18 282,134 34,182" fill="url(#rankGrad)" />
  <!-- Halftone Accent Lines -->
  <line x1="30" y1="75" x2="120" y2="60" stroke="#ffffff" stroke-width="4" stroke-linecap="round" />
  <line x1="25" y1="90" x2="90" y2="80" stroke="#ffffff" stroke-width="3" stroke-linecap="round" />
  <!-- Rank Typography -->
  <text x="150" y="125" text-anchor="middle" font-size="64" fill="#111116" class="bold-text" transform="rotate(-10 150 120)">RANK SSS</text>
  <text x="148" y="122" text-anchor="middle" font-size="64" fill="#ffffff" class="bold-text" transform="rotate(-10 150 120)">RANK SSS</text>
  <!-- Speed Lines -->
  <polygon points="210,35 255,27 250,42" fill="#ffffff" />
'''
    write_svg("badge_hifi_rank_sss.svg", 300, 200, svg_rank)

# ---------------------------------------------------------------------------
# 2. DJMAX RESPECT V (Fever Gauge & Judgement Badge)
# ---------------------------------------------------------------------------

def generate_djmax():
    # SVG 1: FEVER Gauge Bar
    svg_fever = '''
  <defs>
    <linearGradient id="feverGrad" x1="0%" y1="0%" x2="100%" y2="0%">
      <stop offset="0%" stop-color="#00f5d4" />
      <stop offset="50%" stop-color="#7b2cbf" />
      <stop offset="100%" stop-color="#ff007f" />
    </linearGradient>
    <filter id="neonGlow">
      <feGaussianBlur stdDeviation="3" result="blur" />
      <feMerge>
        <feMergeNode in="blur" />
        <feMergeNode in="SourceGraphic" />
      </feMerge>
    </filter>
  </defs>
  <!-- Outer Cyber Shell -->
  <rect x="10" y="10" width="380" height="48" rx="8" fill="#08090d" stroke="#00f5d4" stroke-width="3" />
  <!-- Segmented Grid Lines -->
  <rect x="16" y="16" width="368" height="36" rx="4" fill="#111520" />
  <rect x="16" y="16" width="310" height="36" rx="4" fill="url(#feverGrad)" filter="url(#neonGlow)" />
  <!-- Segments overlay -->
  <line x1="75" y1="16" x2="75" y2="52" stroke="#08090d" stroke-width="4" />
  <line x1="140" y1="16" x2="140" y2="52" stroke="#08090d" stroke-width="4" />
  <line x1="205" y1="16" x2="205" y2="52" stroke="#08090d" stroke-width="4" />
  <line x1="270" y1="16" x2="270" y2="52" stroke="#08090d" stroke-width="4" />
  <line x1="335" y1="16" x2="335" y2="52" stroke="#08090d" stroke-width="4" />
  <!-- Badge Text -->
  <rect x="290" y="8" width="100" height="52" rx="6" fill="#ff007f" stroke="#ffffff" stroke-width="2" />
  <text x="340" y="40" text-anchor="middle" font-size="22" fill="#ffffff" class="bold-text">x5 FEVER</text>
'''
    write_svg("hud_djmax_fever_gauge.svg", 400, 68, svg_fever)

    # SVG 2: Judgement Badge 100% MAX BEAT
    svg_judge = '''
  <defs>
    <linearGradient id="goldPlate" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#ffffff" />
      <stop offset="30%" stop-color="#ffd000" />
      <stop offset="70%" stop-color="#ff7b00" />
      <stop offset="100%" stop-color="#b34700" />
    </linearGradient>
  </defs>
  <polygon points="20,10 260,10 280,60 0,60" fill="#0a0c14" stroke="#ffd000" stroke-width="3" />
  <polygon points="26,16 254,16 270,54 10,54" fill="#141724" />
  <text x="140" y="38" text-anchor="middle" font-size="22" fill="url(#goldPlate)" class="bold-text" letter-spacing="2">100% MAX BEAT</text>
  <circle cx="28" cy="35" r="4" fill="#00f5d4" />
  <circle cx="252" cy="35" r="4" fill="#00f5d4" />
'''
    write_svg("badge_djmax_judgement_max.svg", 280, 70, svg_judge)

# ---------------------------------------------------------------------------
# 3. NOSTALGIA OP. 2 (Baroque Gilded Corner & Concert Wax Seal)
# ---------------------------------------------------------------------------

def generate_nostalgia():
    # SVG 1: Baroque Gilded Corner
    svg_corner = '''
  <defs>
    <linearGradient id="goldFiligree" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#fff5c0" />
      <stop offset="40%" stop-color="#d4af37" />
      <stop offset="70%" stop-color="#aa7c11" />
      <stop offset="100%" stop-color="#553a00" />
    </linearGradient>
  </defs>
  <!-- Ornate Corner Frame -->
  <path d="M 0,0 L 120,0 Q 80,20 60,60 Q 20,80 0,120 Z" fill="url(#goldFiligree)" />
  <path d="M 8,8 L 100,8 Q 65,25 48,58 Q 25,75 8,100 Z" fill="#151210" />
  <!-- Swirl Filigree Details -->
  <path d="M 15,15 Q 45,15 55,45 Q 65,15 95,15 Q 60,35 45,75 Q 35,45 15,15 Z" fill="url(#goldFiligree)" />
  <circle cx="35" cy="35" r="5" fill="#fff5c0" />
  <circle cx="65" cy="20" r="3" fill="#fff5c0" />
  <circle cx="20" cy="65" r="3" fill="#fff5c0" />
'''
    write_svg("ornament_nostalgia_gilded_corner.svg", 128, 128, svg_corner)

    # SVG 2: Classical Concert Wax Seal
    svg_seal = '''
  <defs>
    <radialGradient id="waxGrad" cx="45%" cy="40%" r="55%">
      <stop offset="0%" stop-color="#e63946" />
      <stop offset="70%" stop-color="#9d0208" />
      <stop offset="100%" stop-color="#370617" />
    </radialGradient>
  </defs>
  <!-- Scalloped Wax Droplets -->
  <circle cx="100" cy="100" r="85" fill="url(#waxGrad)" stroke="#370617" stroke-width="4" />
  <circle cx="100" cy="100" r="70" fill="none" stroke="#d4af37" stroke-width="2.5" stroke-dasharray="4,2" />
  <circle cx="100" cy="100" r="64" fill="none" stroke="#d4af37" stroke-width="1.5" />
  <!-- Inner Emblem (Treble Clef + Keys) -->
  <text x="100" y="85" text-anchor="middle" font-size="28" fill="#ffd700" class="serif-text">♪ ♫ ♪</text>
  <text x="100" y="115" text-anchor="middle" font-size="14" fill="#fff5c0" class="serif-text" letter-spacing="1">NOSTALGIA</text>
  <text x="100" y="135" text-anchor="middle" font-size="11" fill="#d4af37" class="serif-text">VIRTUOSO OP. 2</text>
'''
    write_svg("stamp_nostalgia_wax_seal.svg", 200, 200, svg_seal)

# ---------------------------------------------------------------------------
# 4. GRIS (Zen Watercolor Vignette & Celestial Star)
# ---------------------------------------------------------------------------

def generate_gris():
    # SVG 1: Celestial Minimalist Star Icon
    svg_star = '''
  <defs>
    <linearGradient id="grisSilver" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#ffffff" />
      <stop offset="50%" stop-color="#cfd8dc" />
      <stop offset="100%" stop-color="#78909c" />
    </linearGradient>
  </defs>
  <!-- Concentric Fine Geometry -->
  <circle cx="64" cy="64" r="54" fill="none" stroke="#cfd8dc" stroke-width="1.2" stroke-dasharray="2,3" />
  <circle cx="64" cy="64" r="42" fill="none" stroke="#eceff1" stroke-width="1.5" />
  <circle cx="64" cy="64" r="28" fill="none" stroke="#90a4ae" stroke-width="0.8" />
  <!-- 8-Point Diamond Star -->
  <polygon points="64,12 68,52 108,56 72,64 80,104 64,72 48,104 56,64 20,56 60,52" fill="url(#grisSilver)" />
  <circle cx="64" cy="64" r="4" fill="#ffffff" />
'''
    write_svg("icon_gris_celestial_star.svg", 128, 128, svg_star)

    # SVG 2: Zen Mode Frame
    svg_vignette = '''
  <defs>
    <radialGradient id="zenMist" cx="50%" cy="50%" r="50%">
      <stop offset="0%" stop-color="#263238" stop-opacity="0" />
      <stop offset="70%" stop-color="#1a2327" stop-opacity="0.4" />
      <stop offset="100%" stop-color="#0b0e11" stop-opacity="0.95" />
    </radialGradient>
  </defs>
  <rect x="0" y="0" width="400" height="240" fill="url(#zenMist)" />
  <rect x="15" y="15" width="370" height="210" fill="none" stroke="#90a4ae" stroke-width="1" stroke-opacity="0.4" />
  <circle cx="200" cy="120" r="3" fill="#cfd8dc" fill-opacity="0.8" />
'''
    write_svg("overlay_gris_zen_vignette.svg", 400, 240, svg_vignette)

# ---------------------------------------------------------------------------
# 5. MOONLIGHTER (Stitched Leather Tab & Merchant Gold Pouch)
# ---------------------------------------------------------------------------

def generate_moonlighter():
    # SVG 1: Stitched Leather Tab
    svg_tab = '''
  <defs>
    <linearGradient id="leatherGrad" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#8d5b4c" />
      <stop offset="50%" stop-color="#6a382c" />
      <stop offset="100%" stop-color="#4a2218" />
    </linearGradient>
  </defs>
  <!-- Leather Body -->
  <path d="M 0,0 L 220,0 C 235,0 240,15 240,30 L 240,45 C 240,60 230,70 215,70 L 0,70 Z" fill="url(#leatherGrad)" stroke="#2b110a" stroke-width="3" />
  <!-- Stitched Edges -->
  <path d="M 6,6 L 214,6 C 226,6 234,14 234,26 L 234,44 C 234,56 226,64 214,64 L 6,64" fill="none" stroke="#f4a261" stroke-width="2" stroke-dasharray="5,4" />
  <!-- Brass Rivet -->
  <circle cx="28" cy="35" r="9" fill="#e9c46a" stroke="#8d5b4c" stroke-width="2" />
  <circle cx="28" cy="35" r="3" fill="#2b110a" />
  <!-- Label Text -->
  <text x="130" y="43" text-anchor="middle" font-size="18" fill="#f4e8c1" class="serif-text" letter-spacing="1">SETLIST LEDGER</text>
'''
    write_svg("frame_moonlighter_stitched_tab.svg", 250, 75, svg_tab)

    # SVG 2: Merchant Gold Pouch Badge
    svg_pouch = '''
  <circle cx="45" cy="50" r="36" fill="#e76f51" stroke="#264653" stroke-width="3" />
  <circle cx="45" cy="50" r="30" fill="#f4a261" />
  <polygon points="45,14 36,26 54,26" fill="#e76f51" stroke="#264653" stroke-width="2" />
  <!-- Coin Stack -->
  <circle cx="45" cy="48" r="16" fill="#e9c46a" stroke="#8d5b4c" stroke-width="2" />
  <text x="45" y="55" text-anchor="middle" font-size="20" fill="#78350f" class="bold-text">G</text>
'''
    write_svg("badge_moonlighter_gold_pouch.svg", 90, 90, svg_pouch)

# ---------------------------------------------------------------------------
# 6. STARDEW VALLEY (Rustic Wood Trim & Golden Parchment)
# ---------------------------------------------------------------------------

def generate_stardew():
    # SVG 1: Rustic Wood Corner
    svg_wood = '''
  <defs>
    <linearGradient id="oakWood" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#b07d4f" />
      <stop offset="40%" stop-color="#804e27" />
      <stop offset="80%" stop-color="#542e12" />
      <stop offset="100%" stop-color="#361a07" />
    </linearGradient>
  </defs>
  <!-- Wood Plank Frame -->
  <rect x="0" y="0" width="120" height="30" fill="url(#oakWood)" stroke="#221105" stroke-width="3" />
  <rect x="0" y="0" width="30" height="120" fill="url(#oakWood)" stroke="#221105" stroke-width="3" />
  <!-- Cast Iron Corner Bracket -->
  <polygon points="0,0 50,0 50,20 20,20 20,50 0,50" fill="#444951" stroke="#1f2226" stroke-width="2" />
  <!-- Brass Screws -->
  <circle cx="10" cy="10" r="3.5" fill="#d4af37" stroke="#1f2226" stroke-width="1" />
  <circle cx="35" cy="10" r="3.5" fill="#d4af37" stroke="#1f2226" stroke-width="1" />
  <circle cx="10" cy="35" r="3.5" fill="#d4af37" stroke="#1f2226" stroke-width="1" />
'''
    write_svg("frame_stardew_rustic_wood.svg", 120, 120, svg_wood)

    # SVG 2: Golden Parchment Banner
    svg_parchment = '''
  <defs>
    <linearGradient id="paperGrad" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#fdf0d5" />
      <stop offset="60%" stop-color="#ebd5a3" />
      <stop offset="100%" stop-color="#c9ad73" />
    </linearGradient>
  </defs>
  <!-- Banner Body -->
  <path d="M 30,10 L 310,10 L 295,45 L 310,80 L 30,80 L 45,45 Z" fill="url(#paperGrad)" stroke="#785928" stroke-width="3" />
  <text x="170" y="52" text-anchor="middle" font-size="20" fill="#432818" class="serif-text">STAGE REPERTOIRE</text>
'''
    write_svg("banner_stardew_parchment.svg", 340, 90, svg_parchment)

# ---------------------------------------------------------------------------
# 7. LIFE IS STRANGE: BTS (Washi Tape Strip & Polaroid Frame)
# ---------------------------------------------------------------------------

def generate_lis():
    # SVG 1: Translucent Washi Tape Strip
    svg_tape = '''
  <defs>
    <linearGradient id="tapeGrad" x1="0%" y1="0%" x2="100%" y2="0%">
      <stop offset="0%" stop-color="#ffe494" stop-opacity="0.85" />
      <stop offset="50%" stop-color="#fff1c5" stop-opacity="0.9" />
      <stop offset="100%" stop-color="#ffe494" stop-opacity="0.85" />
    </linearGradient>
  </defs>
  <!-- Torn Ends -->
  <polygon points="5,8 15,2 190,4 200,9 196,44 188,48 10,46 4,40" fill="url(#tapeGrad)" stroke="#e0b852" stroke-width="1.2" />
  <line x1="8" y1="12" x2="8" y2="38" stroke="#cca138" stroke-width="1" stroke-dasharray="2,2" />
  <line x1="192" y1="12" x2="192" y2="38" stroke="#cca138" stroke-width="1" stroke-dasharray="2,2" />
'''
    write_svg("tape_lis_washi_strip.svg", 210, 52, svg_tape)

    # SVG 2: Polaroid Photo Frame with Doodle
    svg_polaroid = '''
  <defs>
    <filter id="cardShadow">
      <feDropShadow dx="3" dy="5" stdDeviation="2" flood-color="#000000" flood-opacity="0.3" />
    </filter>
  </defs>
  <rect x="10" y="10" width="160" height="200" rx="3" fill="#faf8f5" stroke="#dedbd5" stroke-width="1.5" filter="url(#cardShadow)" />
  <rect x="20" y="20" width="140" height="130" fill="#1e232a" />
  <!-- Waveform Doodle Inside Picture -->
  <path d="M 25,85 Q 50,40 75,85 T 125,85 T 155,85" fill="none" stroke="#00f5d4" stroke-width="3" />
  <!-- Handwritten Note -->
  <text x="90" y="180" text-anchor="middle" font-size="16" fill="#2d3142" class="hand-text">BAND REHEARSAL</text>
'''
    write_svg("frame_lis_polaroid_badge.svg", 180, 220, svg_polaroid)

# ---------------------------------------------------------------------------
# 8. TAIKO NO TATSUJIN (Don / Ka Dual Drum Markers & Festive Fireworks)
# ---------------------------------------------------------------------------

def generate_taiko():
    # SVG 1: Don / Ka Drum Marker Pair
    svg_don_ka = '''
  <!-- Don Drum (Red - Center Hit) -->
  <g transform="translate(10, 10)">
    <circle cx="50" cy="50" r="46" fill="#ff2a42" stroke="#ffffff" stroke-width="4" />
    <circle cx="50" cy="50" r="36" fill="#ffccd2" />
    <circle cx="38" cy="45" r="4" fill="#111116" />
    <circle cx="62" cy="45" r="4" fill="#111116" />
    <ellipse cx="50" cy="58" rx="8" ry="5" fill="#ff2a42" />
    <text x="50" y="82" text-anchor="middle" font-size="14" fill="#111116" class="bold-text">DON (C)</text>
  </g>
  <!-- Ka Drum (Blue - Rim Hit) -->
  <g transform="translate(130, 10)">
    <circle cx="50" cy="50" r="46" fill="#00b4d8" stroke="#ffffff" stroke-width="4" />
    <circle cx="50" cy="50" r="36" fill="#caf0f8" />
    <circle cx="38" cy="45" r="4" fill="#111116" />
    <circle cx="62" cy="45" r="4" fill="#111116" />
    <ellipse cx="50" cy="58" rx="8" ry="5" fill="#00b4d8" />
    <text x="50" y="82" text-anchor="middle" font-size="14" fill="#111116" class="bold-text">KA (K)</text>
  </g>
'''
    write_svg("icon_taiko_don_ka_pair.svg", 240, 110, svg_don_ka)

    # SVG 2: Festive Fireworks Combo Badge
    svg_fireworks = '''
  <defs>
    <radialGradient id="sunburst" cx="50%" cy="50%" r="50%">
      <stop offset="0%" stop-color="#fff" />
      <stop offset="30%" stop-color="#ffd166" />
      <stop offset="70%" stop-color="#ef476f" />
      <stop offset="100%" stop-color="#118ab2" />
    </radialGradient>
  </defs>
  <circle cx="90" cy="90" r="75" fill="url(#sunburst)" stroke="#ffffff" stroke-width="3" />
  <text x="90" y="85" text-anchor="middle" font-size="28" fill="#ffffff" class="bold-text">100</text>
  <text x="90" y="112" text-anchor="middle" font-size="14" fill="#ffd166" class="bold-text">COMBO!!</text>
'''
    write_svg("badge_taiko_combo_firework.svg", 180, 180, svg_fireworks)

# ---------------------------------------------------------------------------
# 9. CADENCE OF HYRULE (16-Bit Heart Beat Pulse Bar & Pixel Corners)
# ---------------------------------------------------------------------------

def generate_cadence():
    # SVG 1: Pulsing Heart Beat Bar
    svg_heart = '''
  <defs>
    <filter id="pixelDrop">
      <feDropShadow dx="2" dy="2" stdDeviation="0" flood-color="#000" />
    </filter>
  </defs>
  <rect x="5" y="15" width="310" height="42" fill="#1a1423" stroke="#ffd166" stroke-width="3" filter="url(#pixelDrop)" />
  <!-- Converging Beat Chevrons -->
  <path d="M 40,24 L 60,36 L 40,48 M 80,24 L 100,36 L 80,48 M 120,24 L 140,36 L 120,48" fill="none" stroke="#00f5d4" stroke-width="4" stroke-linecap="square" />
  <path d="M 280,24 L 260,36 L 280,48 M 240,24 L 220,36 L 240,48 M 200,24 L 180,36 L 200,48" fill="none" stroke="#00f5d4" stroke-width="4" stroke-linecap="square" />
  <!-- Center 16-Bit Heart -->
  <polygon points="160,24 168,18 174,24 174,32 160,46 146,32 146,24 152,18" fill="#e63946" stroke="#ffffff" stroke-width="2" />
'''
    write_svg("hud_cadence_heart_pulse.svg", 320, 72, svg_heart)

    # SVG 2: Pixel Triforce Corner
    svg_bracket = '''
  <polygon points="0,0 64,0 64,16 16,16 16,64 0,64" fill="#ffd700" stroke="#000" stroke-width="2" />
  <polygon points="20,20 36,20 28,36" fill="#ffd700" stroke="#b8860b" stroke-width="1.5" />
'''
    write_svg("bracket_cadence_triforce_corner.svg", 64, 64, svg_bracket)

# ---------------------------------------------------------------------------
# 10. MINI METRO (Transit Progression Schematic Route Map)
# ---------------------------------------------------------------------------

def generate_minimetro():
    # SVG 1: Song Structure Transit Route Map
    svg_transit = '''
  <rect x="0" y="0" width="480" height="90" rx="8" fill="#161a22" stroke="#30363d" stroke-width="2" />
  <!-- Subway Track Lines (Cyan Line) -->
  <line x1="50" y1="45" x2="430" y2="45" stroke="#00f5d4" stroke-width="6" stroke-linecap="round" />
  <!-- Station 1: Intro (Circle) -->
  <circle cx="60" cy="45" r="14" fill="#ffffff" stroke="#161a22" stroke-width="4" />
  <text x="60" y="75" text-anchor="middle" font-size="11" fill="#8b949e" class="bold-text">INTRO</text>
  <!-- Station 2: Verse (Triangle) -->
  <polygon points="150,33 162,55 138,55" fill="#ffffff" stroke="#161a22" stroke-width="3" />
  <text x="150" y="75" text-anchor="middle" font-size="11" fill="#8b949e" class="bold-text">VERSE</text>
  <!-- Station 3: Chorus (Square) ACTIVE -->
  <rect x="230" y="35" width="20" height="20" fill="#ffd166" stroke="#161a22" stroke-width="3" />
  <circle cx="240" cy="45" r="16" fill="none" stroke="#ffd166" stroke-width="2" stroke-dasharray="3,3" />
  <text x="240" y="75" text-anchor="middle" font-size="11" fill="#ffd166" class="bold-text">CHORUS</text>
  <!-- Station 4: Solo (Diamond) -->
  <polygon points="330,35 340,45 330,55 320,45" fill="#ffffff" stroke="#161a22" stroke-width="3" />
  <text x="330" y="75" text-anchor="middle" font-size="11" fill="#8b949e" class="bold-text">SOLO</text>
  <!-- Station 5: Outro (Circle) -->
  <circle cx="420" cy="45" r="14" fill="#ffffff" stroke="#161a22" stroke-width="4" />
  <text x="420" y="75" text-anchor="middle" font-size="11" fill="#8b949e" class="bold-text">OUTRO</text>
'''
    write_svg("widget_minimetro_transit_map.svg", 480, 90, svg_transit)

# ---------------------------------------------------------------------------
# 11. TRICKY TOWERS (Gothic Masonry Spell Card Frame)
# ---------------------------------------------------------------------------

def generate_trickytowers():
    # SVG 1: Gothic Masonry Spell Card
    svg_card = '''
  <defs>
    <linearGradient id="stoneGrad" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#4a5568" />
      <stop offset="100%" stop-color="#1a202c" />
    </linearGradient>
    <filter id="runeGlow">
      <feGaussianBlur stdDeviation="2" result="blur" />
      <feMerge>
        <feMergeNode in="blur" />
        <feMergeNode in="SourceGraphic" />
      </feMerge>
    </filter>
  </defs>
  <!-- Masonry Stone Borders -->
  <rect x="10" y="10" width="180" height="240" rx="10" fill="url(#stoneGrad)" stroke="#718096" stroke-width="4" />
  <rect x="20" y="20" width="160" height="220" rx="6" fill="#171923" stroke="#9f7aea" stroke-width="2" filter="url(#runeGlow)" />
  <!-- Runic Header -->
  <circle cx="100" cy="50" r="22" fill="#6b46c1" stroke="#e9d8fd" stroke-width="2" />
  <polygon points="100,34 112,50 100,66 88,50" fill="#00f5d4" />
  <!-- Card Text -->
  <text x="100" y="105" text-anchor="middle" font-size="14" fill="#e9d8fd" class="bold-text">MAGIC HARMONY</text>
  <text x="100" y="130" text-anchor="middle" font-size="28" fill="#00f5d4" class="bold-text">G#m7</text>
  <text x="100" y="210" text-anchor="middle" font-size="11" fill="#a0aec0" class="mono-text">[TRANSP: +0]</text>
'''
    write_svg("card_trickytowers_spell_frame.svg", 200, 260, svg_card)

# ---------------------------------------------------------------------------
# 12. SNIPPERCLIPS (Papercraft Cutout Tag & Pastel Sticky Note)
# ---------------------------------------------------------------------------

def generate_snipperclips():
    # SVG 1: Perforated Papercraft Tag
    svg_cutout = '''
  <defs>
    <filter id="paperShadow">
      <feDropShadow dx="2" dy="3" stdDeviation="1" flood-color="#000" flood-opacity="0.25" />
    </filter>
  </defs>
  <rect x="10" y="10" width="220" height="70" rx="6" fill="#ffffff" stroke="#fb8500" stroke-width="3" filter="url(#paperShadow)" />
  <!-- Scissor Cut Dashed Line -->
  <line x1="20" y1="20" x2="220" y2="20" stroke="#023047" stroke-width="2" stroke-dasharray="6,4" />
  <!-- Hole punch -->
  <circle cx="35" cy="45" r="7" fill="#e2e8f0" stroke="#023047" stroke-width="2" />
  <text x="130" y="52" text-anchor="middle" font-size="16" fill="#023047" class="bold-text">SNIP &amp; CLIP NOTE</text>
'''
    write_svg("tag_snipperclips_paper_cutout.svg", 240, 90, svg_cutout)

    # SVG 2: Pastel Yellow Sticky Note
    svg_sticky = '''
  <defs>
    <filter id="stickyDrop">
      <feDropShadow dx="3" dy="4" stdDeviation="2" flood-color="#000" flood-opacity="0.2" />
    </filter>
  </defs>
  <!-- Curled Sticky Note Body -->
  <polygon points="10,10 170,10 170,140 140,170 10,170" fill="#ffefa6" stroke="#e0c75c" stroke-width="1.5" filter="url(#stickyDrop)" />
  <polygon points="140,140 170,140 140,170" fill="#e0c75c" />
  <text x="90" y="60" text-anchor="middle" font-size="15" fill="#4a421b" class="hand-text">Don't forget:</text>
  <text x="90" y="90" text-anchor="middle" font-size="18" fill="#d90429" class="hand-text">CHORUS SOLO</text>
  <text x="90" y="120" text-anchor="middle" font-size="13" fill="#4a421b" class="hand-text">(Bar 032 - 048)</text>
'''
    write_svg("sticker_snipperclips_pastel_note.svg", 180, 180, svg_sticky)

# ---------------------------------------------------------------------------
# HIGH-RES PIXEL-PERFECT PNG RENDERER USING PILLOW
# ---------------------------------------------------------------------------

def render_png_assets():
    """Renders broadcast-ready transparent PNG sprites with Pillow supersampling."""
    print("\n--- Rendering Broadcast-Ready PNG Sprites ---")

    def create_canvas(w, h, scale=2):
        img = Image.new("RGBA", (w * scale, h * scale), (0, 0, 0, 0))
        draw = ImageDraw.Draw(img)
        return img, draw, scale

    def save_canvas(img, filename, scale=2):
        if scale > 1:
            w, h = img.size
            img = img.resize((w // scale, h // scale), Image.Resampling.LANCZOS)
        out_path = PNG_DIR / filename
        img.save(out_path, "PNG")
        print(f"  [PNG Sprite]   -> {filename} ({img.size[0]}x{img.size[1]})")

    # 1. Hi-Fi Rush Comic Burst
    img, draw, s = create_canvas(256, 256)
    cx, cy = 128 * s, 128 * s
    pts = []
    n_spikes = 16
    for i in range(n_spikes * 2):
        angle = i * (math.pi / n_spikes)
        r = (115 * s) if (i % 2 == 0) else (65 * s)
        pts.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    # Outer black outline
    draw.polygon(pts, fill=(17, 17, 22, 255))
    # Inset burst
    inner_pts = []
    for i in range(n_spikes * 2):
        angle = i * (math.pi / n_spikes)
        r = (105 * s) if (i % 2 == 0) else (58 * s)
        inner_pts.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    draw.polygon(inner_pts, fill=(255, 238, 0, 255))
    # Center badge
    draw.ellipse([cx - 55 * s, cy - 35 * s, cx + 55 * s, cy + 35 * s], fill=(17, 17, 22, 255))
    draw.ellipse([cx - 50 * s, cy - 30 * s, cx + 50 * s, cy + 30 * s], fill=(0, 245, 212, 255))
    # Halftone accent dots
    for dot_x in range(int(cx - 30 * s), int(cx + 30 * s), 12 * s):
        draw.ellipse([dot_x, cy + 18 * s, dot_x + 4 * s, cy + 22 * s], fill=(17, 17, 22, 180))
    save_canvas(img, "badge_hifi_comic_burst.png", s)

    # 2. Hi-Fi Rush Rank SSS
    img, draw, s = create_canvas(300, 160)
    poly = [(15 * s, 45 * s), (280 * s, 10 * s), (265 * s, 140 * s), (10 * s, 150 * s)]
    draw.polygon(poly, fill=(17, 17, 22, 255))
    poly_in = [(20 * s, 48 * s), (275 * s, 15 * s), (260 * s, 135 * s), (15 * s, 145 * s)]
    draw.polygon(poly_in, fill=(255, 0, 85, 255))
    # Speedlines
    for ly in [35, 55, 75]:
        draw.line([(30 * s, ly * s), (90 * s, (ly - 6) * s)], fill=(255, 255, 255, 200), width=3 * s)
    # Highlight accent bar
    draw.polygon([(180 * s, 25 * s), (250 * s, 18 * s), (235 * s, 60 * s), (170 * s, 65 * s)], fill=(255, 189, 0, 255))
    save_canvas(img, "badge_hifi_rank_sss.png", s)

    # 3. DJMAX Fever Gauge Bar
    img, draw, s = create_canvas(400, 60)
    draw.rounded_rectangle([10 * s, 10 * s, 390 * s, 50 * s], radius=8 * s, fill=(8, 9, 13, 255), outline=(0, 245, 212, 255), width=3 * s)
    # Segmented LEDs
    led_colors = [
        (0, 245, 212, 255), (0, 210, 230, 255), (50, 150, 240, 255),
        (123, 44, 191, 255), (190, 20, 160, 255), (255, 0, 127, 255)
    ]
    seg_w = 42 * s
    for i, col in enumerate(led_colors):
        x1 = (20 + i * 46) * s
        draw.rounded_rectangle([x1, 16 * s, x1 + seg_w, 44 * s], radius=4 * s, fill=col)
    # Fever multiplier badge
    draw.rounded_rectangle([300 * s, 8 * s, 392 * s, 52 * s], radius=6 * s, fill=(255, 0, 127, 255), outline=(255, 255, 255, 255), width=2 * s)
    save_canvas(img, "hud_djmax_fever_gauge.png", s)

    # 4. DJMAX 100% Max Beat Badge
    img, draw, s = create_canvas(280, 60)
    draw.polygon([(20 * s, 8 * s), (260 * s, 8 * s), (275 * s, 52 * s), (5 * s, 52 * s)], fill=(10, 12, 20, 255))
    draw.polygon([(24 * s, 12 * s), (256 * s, 12 * s), (269 * s, 48 * s), (11 * s, 48 * s)], fill=(20, 23, 36, 255), outline=(255, 208, 0, 255), width=2 * s)
    draw.ellipse([26 * s, 26 * s, 34 * s, 34 * s], fill=(0, 245, 212, 255))
    draw.ellipse([246 * s, 26 * s, 254 * s, 34 * s], fill=(0, 245, 212, 255))
    save_canvas(img, "badge_djmax_judgement_max.png", s)

    # 5. NOSTALGIA Op. 2 Gilded Corner
    img, draw, s = create_canvas(128, 128)
    draw.polygon([(0, 0), (128 * s, 0), (64 * s, 40 * s), (40 * s, 64 * s), (0, 128 * s)], fill=(212, 175, 55, 255))
    draw.polygon([(8 * s, 8 * s), (105 * s, 8 * s), (50 * s, 38 * s), (38 * s, 50 * s), (8 * s, 105 * s)], fill=(21, 18, 16, 255))
    draw.ellipse([30 * s, 30 * s, 44 * s, 44 * s], fill=(255, 245, 192, 255))
    save_canvas(img, "ornament_nostalgia_gilded_corner.png", s)

    # 6. NOSTALGIA Op. 2 Wax Seal Stamp
    img, draw, s = create_canvas(180, 180)
    cx, cy = 90 * s, 90 * s
    draw.ellipse([cx - 75 * s, cy - 75 * s, cx + 75 * s, cy + 75 * s], fill=(157, 2, 8, 255), outline=(55, 6, 23, 255), width=4 * s)
    draw.ellipse([cx - 60 * s, cy - 60 * s, cx + 60 * s, cy + 60 * s], outline=(212, 175, 55, 255), width=2 * s)
    draw.ellipse([cx - 54 * s, cy - 54 * s, cx + 54 * s, cy + 54 * s], outline=(212, 175, 55, 255), width=1 * s)
    # Gilded star center
    draw.polygon([(cx, cy - 25 * s), (cx + 8 * s, cy - 8 * s), (cx + 25 * s, cy), (cx + 8 * s, cy + 8 * s),
                  (cx, cy + 25 * s), (cx - 8 * s, cy + 8 * s), (cx - 25 * s, cy), (cx - 8 * s, cy - 8 * s)],
                 fill=(255, 215, 0, 255))
    save_canvas(img, "stamp_nostalgia_wax_seal.png", s)

    # 7. GRIS Celestial Star Icon & Vignette
    img, draw, s = create_canvas(128, 128)
    cx, cy = 64 * s, 64 * s
    draw.ellipse([cx - 50 * s, cy - 50 * s, cx + 50 * s, cy + 50 * s], outline=(207, 216, 220, 180), width=1 * s)
    draw.ellipse([cx - 36 * s, cy - 36 * s, cx + 36 * s, cy + 36 * s], outline=(236, 239, 241, 230), width=2 * s)
    # Diamond star
    draw.polygon([(cx, cy - 40 * s), (cx + 7 * s, cy - 7 * s), (cx + 40 * s, cy), (cx + 7 * s, cy + 7 * s),
                  (cx, cy + 40 * s), (cx - 7 * s, cy + 7 * s), (cx - 40 * s, cy), (cx - 7 * s, cy - 7 * s)],
                 fill=(255, 255, 255, 240))
    draw.ellipse([cx - 4 * s, cy - 4 * s, cx + 4 * s, cy + 4 * s], fill=(255, 255, 255, 255))
    save_canvas(img, "icon_gris_celestial_star.png", s)

    # 7b. GRIS Zen Vignette
    img, draw, s = create_canvas(480, 270)
    w_px, h_px = 480 * s, 270 * s
    max_steps = 28
    for r_idx in range(max_steps):
        alpha = int((float(r_idx) / max_steps) ** 2 * 210)
        x_m = r_idx * 6 * s
        y_m = r_idx * 4 * s
        if (w_px - x_m) > x_m and (h_px - y_m) > y_m:
            draw.rectangle([x_m, y_m, w_px - x_m, h_px - y_m], outline=(11, 14, 17, alpha), width=4 * s)
    save_canvas(img, "overlay_gris_zen_vignette.png", s)

    # 7c. Stardew Parchment Banner
    img, draw, s = create_canvas(340, 80)
    draw.polygon([(25 * s, 10 * s), (315 * s, 10 * s), (300 * s, 40 * s), (315 * s, 70 * s), (25 * s, 70 * s), (40 * s, 40 * s)],
                 fill=(245, 228, 180, 255), outline=(120, 89, 40, 255), width=3 * s)
    save_canvas(img, "banner_stardew_parchment.png", s)

    # 8. Moonlighter Stitched Leather Tab
    img, draw, s = create_canvas(240, 64)
    draw.rounded_rectangle([0, 0, 230 * s, 60 * s], radius=12 * s, fill=(106, 56, 44, 255), outline=(43, 17, 10, 255), width=3 * s)
    # Stitches
    for sx in range(12 * s, 220 * s, 10 * s):
        draw.line([(sx, 6 * s), (sx + 5 * s, 6 * s)], fill=(244, 162, 97, 255), width=2 * s)
        draw.line([(sx, 54 * s), (sx + 5 * s, 54 * s)], fill=(244, 162, 97, 255), width=2 * s)
    # Brass rivet
    draw.ellipse([20 * s, 20 * s, 36 * s, 36 * s], fill=(233, 196, 106, 255), outline=(141, 91, 76, 255), width=2 * s)
    save_canvas(img, "frame_moonlighter_stitched_tab.png", s)

    # 9. Stardew Valley Rustic Wood Corner
    img, draw, s = create_canvas(100, 100)
    draw.rectangle([0, 0, 100 * s, 24 * s], fill=(128, 78, 39, 255), outline=(34, 17, 5, 255), width=2 * s)
    draw.rectangle([0, 0, 24 * s, 100 * s], fill=(128, 78, 39, 255), outline=(34, 17, 5, 255), width=2 * s)
    # Metal Corner joiner
    draw.polygon([(0, 0), (40 * s, 0), (40 * s, 16 * s), (16 * s, 16 * s), (16 * s, 40 * s), (0, 40 * s)], fill=(68, 73, 81, 255))
    draw.ellipse([7 * s, 7 * s, 13 * s, 13 * s], fill=(212, 175, 55, 255))
    save_canvas(img, "frame_stardew_rustic_wood.png", s)

    # 10. Life is Strange Washi Tape Strip
    img, draw, s = create_canvas(200, 46)
    draw.polygon([(6 * s, 4 * s), (194 * s, 2 * s), (190 * s, 42 * s), (4 * s, 44 * s)], fill=(255, 228, 148, 220), outline=(224, 184, 82, 255), width=1 * s)
    # Perforated cut ends
    draw.line([(8 * s, 6 * s), (8 * s, 40 * s)], fill=(204, 161, 56, 180), width=2 * s)
    draw.line([(188 * s, 6 * s), (188 * s, 40 * s)], fill=(204, 161, 56, 180), width=2 * s)
    save_canvas(img, "tape_lis_washi_strip.png", s)

    # 11. Taiko Don & Ka Dual Drum Icons
    img, draw, s = create_canvas(220, 100)
    # Don
    draw.ellipse([10 * s, 10 * s, 90 * s, 90 * s], fill=(255, 42, 66, 255), outline=(255, 255, 255, 255), width=4 * s)
    draw.ellipse([22 * s, 22 * s, 78 * s, 78 * s], fill=(255, 204, 210, 255))
    draw.ellipse([38 * s, 42 * s, 44 * s, 48 * s], fill=(17, 17, 22, 255))
    draw.ellipse([56 * s, 42 * s, 62 * s, 48 * s], fill=(17, 17, 22, 255))
    # Ka
    draw.ellipse([120 * s, 10 * s, 200 * s, 90 * s], fill=(0, 180, 216, 255), outline=(255, 255, 255, 255), width=4 * s)
    draw.ellipse([132 * s, 22 * s, 188 * s, 78 * s], fill=(202, 240, 248, 255))
    draw.ellipse([148 * s, 42 * s, 154 * s, 48 * s], fill=(17, 17, 22, 255))
    draw.ellipse([166 * s, 42 * s, 172 * s, 48 * s], fill=(17, 17, 22, 255))
    save_canvas(img, "icon_taiko_don_ka_pair.png", s)

    # 12. Cadence of Hyrule Heart Beat Pulse Bar
    img, draw, s = create_canvas(300, 60)
    draw.rectangle([8 * s, 10 * s, 292 * s, 50 * s], fill=(26, 20, 35, 255), outline=(255, 209, 102, 255), width=3 * s)
    # Beat Chevrons
    for x in [35, 65, 95]:
        draw.line([(x * s, 20 * s), ((x + 12) * s, 30 * s), (x * s, 40 * s)], fill=(0, 245, 212, 255), width=3 * s)
    for x in [265, 235, 205]:
        draw.line([(x * s, 20 * s), ((x - 12) * s, 30 * s), (x * s, 40 * s)], fill=(0, 245, 212, 255), width=3 * s)
    # Center Pixel Heart
    draw.polygon([(150 * s, 18 * s), (160 * s, 26 * s), (150 * s, 44 * s), (140 * s, 26 * s)], fill=(230, 57, 70, 255), outline=(255, 255, 255, 255), width=2 * s)
    save_canvas(img, "hud_cadence_heart_pulse.png", s)

    # 13. Mini Metro Song Structure Transit Map
    img, draw, s = create_canvas(460, 70)
    draw.rounded_rectangle([0, 0, 460 * s, 70 * s], radius=8 * s, fill=(22, 26, 34, 255), outline=(48, 54, 61, 255), width=2 * s)
    # Subway track line
    draw.line([(40 * s, 35 * s), (420 * s, 35 * s)], fill=(0, 245, 212, 255), width=6 * s)
    # Station nodes:
    # 1: Circle
    draw.ellipse([45 * s, 23 * s, 69 * s, 47 * s], fill=(255, 255, 255, 255), outline=(22, 26, 34, 255), width=3 * s)
    # 2: Triangle
    draw.polygon([(135 * s, 22 * s), (147 * s, 46 * s), (123 * s, 46 * s)], fill=(255, 255, 255, 255), outline=(22, 26, 34, 255), width=3 * s)
    # 3: Square (Active)
    draw.rectangle([215 * s, 23 * s, 239 * s, 47 * s], fill=(255, 209, 102, 255), outline=(22, 26, 34, 255), width=3 * s)
    draw.ellipse([210 * s, 18 * s, 244 * s, 52 * s], outline=(255, 209, 102, 255), width=2 * s)
    # 4: Diamond
    draw.polygon([(315 * s, 22 * s), (327 * s, 35 * s), (315 * s, 48 * s), (303 * s, 35 * s)], fill=(255, 255, 255, 255), outline=(22, 26, 34, 255), width=3 * s)
    # 5: Circle
    draw.ellipse([395 * s, 23 * s, 419 * s, 47 * s], fill=(255, 255, 255, 255), outline=(22, 26, 34, 255), width=3 * s)
    save_canvas(img, "widget_minimetro_transit_map.png", s)

    # 14. Tricky Towers Masonry Spell Card Frame
    img, draw, s = create_canvas(180, 240)
    draw.rounded_rectangle([10 * s, 10 * s, 170 * s, 230 * s], radius=10 * s, fill=(45, 55, 72, 255), outline=(113, 128, 150, 255), width=4 * s)
    draw.rounded_rectangle([20 * s, 20 * s, 160 * s, 220 * s], radius=6 * s, fill=(23, 25, 35, 255), outline=(159, 122, 234, 255), width=2 * s)
    # Gemstone top
    draw.ellipse([78 * s, 36 * s, 102 * s, 60 * s], fill=(107, 70, 193, 255), outline=(233, 216, 253, 255), width=2 * s)
    draw.polygon([(90 * s, 40 * s), (97 * s, 48 * s), (90 * s, 56 * s), (83 * s, 48 * s)], fill=(0, 245, 212, 255))
    save_canvas(img, "card_trickytowers_spell_frame.png", s)

    # 15. Snipperclips Papercraft Cutout Tag
    img, draw, s = create_canvas(220, 70)
    draw.rounded_rectangle([6 * s, 6 * s, 214 * s, 64 * s], radius=6 * s, fill=(255, 255, 255, 255), outline=(251, 133, 0, 255), width=3 * s)
    # Perforation dashed line
    for dx in range(16 * s, 200 * s, 12 * s):
        draw.line([(dx, 16 * s), (dx + 6 * s, 16 * s)], fill=(2, 48, 71, 255), width=2 * s)
    draw.ellipse([20 * s, 30 * s, 34 * s, 44 * s], fill=(226, 232, 240, 255), outline=(2, 48, 71, 255), width=2 * s)
    save_canvas(img, "tag_snipperclips_paper_cutout.png", s)

    # 16. Snipperclips Pastel Yellow Sticky Note
    img, draw, s = create_canvas(160, 160)
    poly = [(10 * s, 10 * s), (150 * s, 10 * s), (150 * s, 125 * s), (125 * s, 150 * s), (10 * s, 150 * s)]
    draw.polygon(poly, fill=(255, 239, 166, 255), outline=(224, 199, 92, 255), width=2 * s)
    # Folded bottom corner
    draw.polygon([(125 * s, 125 * s), (150 * s, 125 * s), (125 * s, 150 * s)], fill=(224, 199, 92, 255))
    save_canvas(img, "sticker_snipperclips_pastel_note.png", s)

# ---------------------------------------------------------------------------
# MAIN EXECUTION ENTRYPOINT
# ---------------------------------------------------------------------------

def main():
    print("=" * 70)
    print("ANANTANADA: GENERATING UI SPRITES & TEMPLATES FROM 12 MASTER GAMES")
    print("=" * 70)
    print("\n[Phase 1] Writing Editable Vector SVG Templates...")
    generate_hifi_rush()
    generate_djmax()
    generate_nostalgia()
    generate_gris()
    generate_moonlighter()
    generate_stardew()
    generate_lis()
    generate_taiko()
    generate_cadence()
    generate_minimetro()
    generate_trickytowers()
    generate_snipperclips()

    print("\n[Phase 2] Rendering Production-Ready Transparent PNG Sprites...")
    render_png_assets()

    print("\n" + "=" * 70)
    print("ALL 12 GAME UI INSPIRATIONS SUCCESSFULLY GENERATED!")
    print(f"Templates Path: {TEMPLATES_DIR}")
    print(f"PNG Sprites:    {PNG_DIR}")
    print("=" * 70)

if __name__ == "__main__":
    main()
