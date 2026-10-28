"""
=============================================================================
ANANTANADA SWEET RPG UI/UX SUITE GENERATOR
Generates charming, tactile, diegetic RPG assets inspired by:
- Moonlighter (Open Notebook, Spiral Binding, Reaction Faces, Price Tags)
- Animal Crossing: New Horizons (Scalloped Cloud Bar, Recipe Cards, Stamp Books)
- NieR:Automata (Tactical Diegetic Grid & Audio Meters)
- Bad North (Clean Commander Portrait Frames & Pastel Ribbons)
- Assassin's Creed Mirage Codex (Historical Parchment & Archival Mounts)
- Overwatch 2 (Clean Bento Category Badges)
=============================================================================
"""

import math
from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter

SCRIPT_DIR = Path(__file__).resolve().parent
SPRITES_DIR = SCRIPT_DIR / "sprites"
TEMPLATES_DIR = SCRIPT_DIR / "templates"
SPRITES_DIR.mkdir(parents=True, exist_ok=True)
TEMPLATES_DIR.mkdir(parents=True, exist_ok=True)

def canvas(w, h, scale=2):
    img = Image.new("RGBA", (w * scale, h * scale), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    return img, draw, scale

def save(img, filename, scale=2):
    if scale > 1:
        w, h = img.size
        img = img.resize((w // scale, h // scale), Image.Resampling.LANCZOS)
    out_path = SPRITES_DIR / filename
    img.save(out_path, "PNG")
    print(f"  [RPG Sprite] -> {filename} ({img.size[0]}x{img.size[1]})")

# ---------------------------------------------------------------------------
# 1. COZY WOODEN DESK SURFACE (1920x1080 Full HD Background)
# ---------------------------------------------------------------------------
def generate_desk_wood():
    print("Generating Cozy Wooden Desk Surface...")
    w, h = 1920, 1080
    img = Image.new("RGBA", (w, h), (44, 28, 18, 255))
    draw = ImageDraw.Draw(img)

    # Horizontal wood planks
    plank_h = 135
    for y in range(0, h, plank_h):
        # Plank base shade variations
        shade = 32 + (y * 13) % 20
        draw.rectangle([0, y, w, y + plank_h], fill=(shade + 18, shade + 8, shade, 255))
        # Plank seams
        draw.line([(0, y), (w, y)], fill=(18, 10, 6, 255), width=3)
        draw.line([(0, y + 2), (w, y + 2)], fill=(85, 55, 35, 120), width=1)
        # Subtle wood grain streaks
        for x in range(0, w, 40):
            gy = y + (x * 7) % (plank_h - 10)
            gw = 30 + (x * 11) % 80
            draw.line([(x, gy), (x + gw, gy)], fill=(shade + 26, shade + 14, shade + 5, 80), width=1)

    # Ambient vignette (darker edges, warm light in center)
    vignette = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    vdraw = ImageDraw.Draw(vignette)
    cx, cy = w // 2, h // 2
    max_r = math.sqrt(cx * cx + cy * cy)
    for r in range(0, int(max_r), 18):
        alpha = int((r / max_r) ** 2.2 * 180)
        vdraw.ellipse([cx - r, cy - r, cx + r, cy + r], outline=(10, 6, 4, alpha), width=20)
    
    img = Image.alpha_composite(img, vignette)
    save(img, "desk_wood_surface.png", 1)

# ---------------------------------------------------------------------------
# 2. MOONLIGHTER OPEN NOTEBOOK / PARCHMENT GRIMOIRE CONTAINER
# ---------------------------------------------------------------------------
def generate_notebook_page():
    print("Generating Moonlighter Notebook Pages...")
    # Notebook page container for Left & Right decks
    img, draw, s = canvas(380, 560)
    w, h = 380 * s, 560 * s

    # Outer Leather Cover Border (Brown Leather with stitches)
    draw.rounded_rectangle([4 * s, 4 * s, w - 4 * s, h - 4 * s], radius=16 * s, fill=(92, 53, 37, 255), outline=(42, 22, 14, 255), width=3 * s)
    # Stitches along leather
    for sx in range(16 * s, w - 16 * s, 12 * s):
        draw.line([(sx, 8 * s), (sx + 6 * s, 8 * s)], fill=(210, 160, 100, 200), width=2 * s)
        draw.line([(sx, h - 8 * s), (sx + 6 * s, h - 8 * s)], fill=(210, 160, 100, 200), width=2 * s)

    # Cream Parchment Page Body
    draw.rounded_rectangle([14 * s, 14 * s, w - 14 * s, h - 14 * s], radius=10 * s, fill=(247, 241, 227, 255), outline=(218, 205, 178, 255), width=2 * s)

    # Delicate Page Inner Margin
    draw.rounded_rectangle([20 * s, 20 * s, w - 20 * s, h - 20 * s], radius=6 * s, outline=(232, 220, 196, 255), width=1 * s)

    # Notebook lined paper lines (subtle ruled lines)
    for ly in range(90 * s, h - 40 * s, 36 * s):
        draw.line([(24 * s, ly), (w - 24 * s, ly)], fill=(215, 205, 185, 120), width=1 * s)

    # Red bookmark ribbon peeking at bottom
    draw.polygon([(w - 70 * s, h - 14 * s), (w - 30 * s, h - 14 * s), (w - 30 * s, h + 18 * s), (w - 50 * s, h + 6 * s), (w - 70 * s, h + 18 * s)], fill=(217, 4, 41, 255), outline=(140, 2, 26, 255), width=2 * s)

    save(img, "notebook_page_parchment.png", s)

# ---------------------------------------------------------------------------
# 3. MOONLIGHTER SPIRAL WIRE RINGS
# ---------------------------------------------------------------------------
def generate_spiral_binding():
    print("Generating Spiral Wire Rings...")
    img, draw, s = canvas(30, 480)
    w, h = 30 * s, 480 * s
    # Series of metal coil loops
    for y in range(20 * s, h - 20 * s, 32 * s):
        # Shadow
        draw.ellipse([2 * s, y + 2 * s, 24 * s, y + 16 * s], fill=(40, 30, 25, 160))
        # Coil metal
        draw.arc([4 * s, y, 26 * s, y + 14 * s], start=30, end=330, fill=(180, 190, 205, 255), width=4 * s)
        draw.arc([6 * s, y + 2 * s, 24 * s, y + 12 * s], start=45, end=315, fill=(240, 245, 255, 255), width=2 * s)
    save(img, "spiral_wire_binding.png", s)

# ---------------------------------------------------------------------------
# 4. ANIMAL CROSSING SCALLOPED CLOUD HEADER
# ---------------------------------------------------------------------------
def generate_ac_scalloped_header():
    print("Generating Animal Crossing Scalloped Cloud Header...")
    img, draw, s = canvas(600, 64)
    w, h = 600 * s, 64 * s

    # Cloud scallops along top
    r = 20 * s
    pts = []
    # Bottom flat line
    pts.append((0, h))
    pts.append((w, h))
    pts.append((w, 36 * s))
    # Scallops from right to left
    step = 40 * s
    for x in range(w, 0, -step):
        pts.append((x - step // 2, 8 * s))
        pts.append((x - step, 36 * s))
    pts.append((0, h))

    # Cloud fill
    draw.polygon(pts, fill=(255, 253, 245, 255), outline=(235, 226, 200, 255))
    # Cloud soft dropshadow
    draw.line([(0, h - 2 * s), (w, h - 2 * s)], fill=(215, 200, 165, 200), width=3 * s)

    save(img, "scalloped_cloud_banner.png", s)

# ---------------------------------------------------------------------------
# 5. MOONLIGHTER COLORFUL BOOKMARK TABS (Mint, Honey, Berry, Sky)
# ---------------------------------------------------------------------------
def generate_bookmark_tabs():
    print("Generating Moonlighter Bookmark Tabs...")
    tabs = [
        ("tab_mint.png", (42, 157, 143), (28, 110, 100), "ALL"),
        ("tab_honey.png", (244, 162, 97), (180, 110, 60), "INDO"),
        ("tab_berry.png", (231, 111, 81), (160, 65, 45), "WEST"),
        ("tab_sky.png", (0, 180, 216), (0, 120, 150), "STAR")
    ]
    for filename, bg, border, label in tabs:
        img, draw, s = canvas(84, 46)
        w, h = 84 * s, 46 * s
        # Rounded protruding tab
        draw.rounded_rectangle([2 * s, 2 * s, w - 2 * s, h + 10 * s], radius=8 * s, fill=bg, outline=border, width=3 * s)
        # Inner stitch highlight
        draw.line([(6 * s, 6 * s), (w - 6 * s, 6 * s)], fill=(255, 255, 255, 120), width=2 * s)
        # Dot rivet
        draw.ellipse([w // 2 - 4 * s, 14 * s, w // 2 + 4 * s, 22 * s], fill=(255, 255, 255, 220), outline=border, width=1 * s)
        save(img, filename, s)

# ---------------------------------------------------------------------------
# 6. ANIMAL CROSSING RECIPE ITEM CARDS & STAMP CARDS
# ---------------------------------------------------------------------------
def generate_ac_cards():
    print("Generating Animal Crossing Recipe & Stamp Cards...")
    # Recipe Card
    img, draw, s = canvas(320, 56)
    w, h = 320 * s, 56 * s
    # Warm butter cream background
    draw.rounded_rectangle([2 * s, 2 * s, w - 2 * s, h - 2 * s], radius=10 * s, fill=(255, 250, 230, 255), outline=(228, 214, 176, 255), width=2 * s)
    # Acorn / Dot watermark pattern
    for x in range(16 * s, w - 20 * s, 28 * s):
        draw.ellipse([x, 14 * s, x + 4 * s, 18 * s], fill=(240, 230, 200, 160))
        draw.ellipse([x + 14 * s, 36 * s, x + 18 * s, 40 * s], fill=(240, 230, 200, 160))
    save(img, "card_ac_recipe_item.png", s)

    # Stamp Slot Card (Nook Miles Stamp Slot)
    img, draw, s = canvas(260, 72)
    w, h = 260 * s, 72 * s
    draw.rounded_rectangle([2 * s, 2 * s, w - 2 * s, h - 2 * s], radius=8 * s, fill=(238, 248, 252, 255), outline=(180, 220, 235, 255), width=2 * s)
    # 3 circular stamp spots
    for idx, sx in enumerate([35 * s, 105 * s, 175 * s]):
        draw.ellipse([sx, 12 * s, sx + 48 * s, 60 * s], fill=(255, 255, 255, 240), outline=(200, 230, 240, 255), width=2 * s)
        # Subtle stamp punch marks
        draw.ellipse([sx + 6 * s, 18 * s, sx + 42 * s, 54 * s], outline=(220, 240, 248, 180), width=1 * s)
    save(img, "card_ac_stamp_slot.png", s)

# ---------------------------------------------------------------------------
# 7. MOONLIGHTER CUTE REACTION EMOJI STAMPS
# ---------------------------------------------------------------------------
def generate_moonlighter_reactions():
    print("Generating Moonlighter Reaction Emoji Stamps...")
    reactions = [
        ("emoji_reaction_happy.png", (42, 157, 143), (255, 255, 255), "^ _ ^", "50G"),
        ("emoji_reaction_cheer.png", (233, 196, 106), (42, 22, 14), "* w *", "PERFECT!"),
        ("emoji_reaction_neutral.png", (244, 162, 97), (42, 22, 14), "- _ -", "GOOD"),
        ("emoji_reaction_sweat.png", (231, 111, 81), (255, 255, 255), "O _ O;", "MISS")
    ]
    for filename, bg, face_col, face, tag in reactions:
        img, draw, s = canvas(64, 64)
        w, h = 64 * s, 64 * s
        cx, cy = w // 2, h // 2
        # Circle stamp
        draw.ellipse([3 * s, 3 * s, w - 3 * s, h - 3 * s], fill=bg, outline=(255, 255, 255, 240), width=3 * s)
        # Inner dashed ring
        draw.ellipse([8 * s, 8 * s, w - 8 * s, h - 8 * s], outline=(255, 255, 255, 140), width=1 * s)
        # Eyes
        if "^" in face:
            draw.arc([16 * s, 20 * s, 26 * s, 30 * s], start=180, end=360, fill=face_col, width=3 * s)
            draw.arc([38 * s, 20 * s, 48 * s, 30 * s], start=180, end=360, fill=face_col, width=3 * s)
            draw.arc([24 * s, 32 * s, 40 * s, 46 * s], start=0, end=180, fill=face_col, width=3 * s)
        elif "*" in face:
            draw.polygon([(21 * s, 20 * s), (23 * s, 26 * s), (17 * s, 24 * s)], fill=face_col)
            draw.polygon([(43 * s, 20 * s), (41 * s, 26 * s), (47 * s, 24 * s)], fill=face_col)
            draw.arc([22 * s, 30 * s, 42 * s, 48 * s], start=0, end=180, fill=face_col, width=4 * s)
        else:
            draw.ellipse([18 * s, 22 * s, 26 * s, 30 * s], fill=face_col)
            draw.ellipse([38 * s, 22 * s, 46 * s, 30 * s], fill=face_col)
            draw.line([(24 * s, 40 * s), (40 * s, 40 * s)], fill=face_col, width=3 * s)
        save(img, filename, s)

# ---------------------------------------------------------------------------
# 8. GOLD COIN & STAR FAVORITE BADGES
# ---------------------------------------------------------------------------
def generate_coin_and_star():
    print("Generating Gold Coins & Star Badges...")
    # Shiny Gold Coin
    img, draw, s = canvas(54, 54)
    w, h = 54 * s, 54 * s
    draw.ellipse([2 * s, 2 * s, w - 2 * s, h - 2 * s], fill=(233, 196, 106, 255), outline=(180, 130, 40, 255), width=3 * s)
    draw.ellipse([8 * s, 8 * s, w - 8 * s, h - 8 * s], fill=(250, 220, 130, 255), outline=(210, 160, 60, 255), width=2 * s)
    # Embossed 'G'
    draw.arc([16 * s, 16 * s, 38 * s, 38 * s], start=45, end=315, fill=(150, 100, 20, 255), width=4 * s)
    draw.line([(26 * s, 27 * s), (36 * s, 27 * s)], fill=(150, 100, 20, 255), width=4 * s)
    save(img, "badge_gold_coin_rpg.png", s)

    # Gold Favorite Star Pin
    img, draw, s = canvas(48, 48)
    w, h = 48 * s, 48 * s
    cx, cy = w // 2, h // 2
    pts = []
    for i in range(10):
        angle = i * (math.pi / 5) - math.pi / 2
        r = (21 * s) if (i % 2 == 0) else (10 * s)
        pts.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    draw.polygon(pts, fill=(255, 215, 0, 255), outline=(190, 140, 20, 255))
    save(img, "badge_star_pin.png", s)

# ---------------------------------------------------------------------------
# 9. RPG STAT GAUGE & HP/MANA ORNATE FRAMES
# ---------------------------------------------------------------------------
def generate_rpg_gauges():
    print("Generating RPG Health/Mana & Dynamic Strike Gauges...")
    img, draw, s = canvas(380, 48)
    w, h = 380 * s, 48 * s
    # Ornate bronze / golden bezel
    draw.rounded_rectangle([2 * s, 2 * s, w - 2 * s, h - 2 * s], radius=10 * s, fill=(35, 22, 15, 255), outline=(180, 135, 70, 255), width=3 * s)
    # Gauge channel
    draw.rounded_rectangle([8 * s, 8 * s, w - 8 * s, h - 8 * s], radius=6 * s, fill=(18, 12, 8, 255))
    # Mana / Energy Gradient Fill (75% full)
    draw.rounded_rectangle([10 * s, 10 * s, int((w - 20 * s) * 0.78), h - 10 * s], radius=4 * s, fill=(42, 157, 143, 255))
    # Glass gloss shine on top half
    draw.line([(12 * s, 12 * s), (int((w - 20 * s) * 0.78), 12 * s)], fill=(255, 255, 255, 140), width=2 * s)
    # Gemstone bracket at left
    draw.ellipse([4 * s, 14 * s, 20 * s, 34 * s], fill=(231, 111, 81, 255), outline=(255, 220, 140, 255), width=2 * s)
    save(img, "gauge_rpg_mana_bar.png", s)

# ---------------------------------------------------------------------------
# 10. AC MIRAGE & MOONLIGHTER TACTICAL CODEX PARCHMENT SCROLL
# ---------------------------------------------------------------------------
def generate_codex_parchment():
    print("Generating AC Mirage Codex Parchment Scroll...")
    img, draw, s = canvas(340, 140)
    w, h = 340 * s, 140 * s
    # Rolled parchment scroll
    draw.rounded_rectangle([6 * s, 6 * s, w - 6 * s, h - 6 * s], radius=8 * s, fill=(253, 248, 235, 255), outline=(195, 175, 140, 255), width=2 * s)
    # Scroll curled edges
    draw.line([(10 * s, 10 * s), (w - 10 * s, 10 * s)], fill=(225, 205, 170, 200), width=2 * s)
    draw.line([(10 * s, h - 10 * s), (w - 10 * s, h - 10 * s)], fill=(225, 205, 170, 200), width=2 * s)
    # Subtle ornate corner filigree
    draw.arc([8 * s, 8 * s, 28 * s, 28 * s], start=180, end=270, fill=(160, 130, 80, 255), width=2 * s)
    draw.arc([w - 28 * s, 8 * s, w - 8 * s, 28 * s], start=270, end=360, fill=(160, 130, 80, 255), width=2 * s)
    save(img, "parchment_codex_scroll.png", s)

# ---------------------------------------------------------------------------
# 11. GOTHIC RPG RUNIC SPELL CARD (Active Harmony Box)
# ---------------------------------------------------------------------------
def generate_spell_card():
    print("Generating Gothic RPG Runic Spell Card...")
    img, draw, s = canvas(340, 136)
    w, h = 340 * s, 136 * s
    # Dark arcane stone card with glowing gold trim
    draw.rounded_rectangle([4 * s, 4 * s, w - 4 * s, h - 4 * s], radius=12 * s, fill=(28, 22, 32, 255), outline=(212, 175, 55, 255), width=3 * s)
    # Inner runic border
    draw.rounded_rectangle([10 * s, 10 * s, w - 10 * s, h - 10 * s], radius=8 * s, outline=(107, 70, 193, 200), width=2 * s)
    # Four corner runes
    for rx, ry in [(16 * s, 16 * s), (w - 24 * s, 16 * s), (16 * s, h - 24 * s), (w - 24 * s, h - 24 * s)]:
        draw.polygon([(rx + 4 * s, ry), (rx + 8 * s, ry + 4 * s), (rx + 4 * s, ry + 8 * s), (rx, ry + 4 * s)], fill=(0, 245, 212, 255))
    save(img, "card_rpg_spell_harmony.png", s)

# ---------------------------------------------------------------------------
# MAIN EXECUTION
# ---------------------------------------------------------------------------
def main():
    print("=" * 70)
    print("ANANTANADA: GENERATING SWEET RPG UI/UX ASSET SUITE")
    print("=" * 70)
    generate_desk_wood()
    generate_notebook_page()
    generate_spiral_binding()
    generate_ac_scalloped_header()
    generate_bookmark_tabs()
    generate_ac_cards()
    generate_moonlighter_reactions()
    generate_coin_and_star()
    generate_rpg_gauges()
    generate_codex_parchment()
    generate_spell_card()
    print("=" * 70)
    print("ALL SWEET RPG ASSETS GENERATED SUCCESSFULLY!")
    print(f"Path: {SPRITES_DIR}")
    print("=" * 70)

if __name__ == "__main__":
    main()
