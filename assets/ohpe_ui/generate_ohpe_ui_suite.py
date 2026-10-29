#!/usr/bin/env python3
"""
OHPE UI/UX DESIGN SUITE GENERATOR
Generates modern, minimalist, Dieter Rams / Teenage Engineering OP-1 / OP-XY
inspired hardware UI sprites for AnantaNada (Roland Go:Keys 3 Workstation).
"""

import os
import math
from PIL import Image, ImageDraw, ImageFont, ImageFilter

OUTPUT_DIR = os.path.abspath("assets/ohpe_ui/sprites")
TARGET_GODOT = os.path.abspath("assets/ui_sprites/png")
TARGET_WEB = os.path.abspath("web/assets/ui_sprites/png")

os.makedirs(OUTPUT_DIR, exist_ok=True)
os.makedirs(TARGET_GODOT, exist_ok=True)
os.makedirs(TARGET_WEB, exist_ok=True)

S = 4  # 4x Supersampling for razor-sharp vector-like rendering

def save_sprite(im, filename):
    downsampled = im.resize((im.width // S, im.height // S), Image.Resampling.LANCZOS)
    path_out = os.path.join(OUTPUT_DIR, filename)
    downsampled.save(path_out, "PNG")
    downsampled.save(os.path.join(TARGET_GODOT, filename), "PNG")
    downsampled.save(os.path.join(TARGET_WEB, filename), "PNG")
    print(f"Generated: {filename} ({downsampled.width}x{downsampled.height})")

def make_chassis():
    w, h = 1920 * S, 1080 * S
    im = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(im)

    # Outer chassis - warm matte cream / light stone
    # #ece9e2 = (236, 233, 226)
    chassis_col = (236, 233, 226, 255)
    border_col = (212, 208, 198, 255)
    shadow_col = (180, 175, 165, 255)

    pad = 12 * S
    corner_r = 28 * S

    # Drop shadow below chassis
    draw.rounded_rectangle([pad + 4*S, pad + 8*S, w - pad + 4*S, h - pad + 8*S], radius=corner_r, fill=(0, 0, 0, 40))
    # Main unibody plate
    draw.rounded_rectangle([pad, pad, w - pad, h - pad], radius=corner_r, fill=chassis_col, outline=border_col, width=3*S)

    # Subtle inner bevel line
    draw.rounded_rectangle([pad + 4*S, pad + 4*S, w - pad - 4*S, h - pad - 4*S], radius=corner_r - 2*S, outline=(255, 255, 255, 180), width=2*S)

    # Micro screws in 4 corners
    screw_r = 8 * S
    screw_pos = [
        (pad + 36 * S, pad + 36 * S),
        (w - pad - 36 * S, pad + 36 * S),
        (pad + 36 * S, h - pad - 36 * S),
        (w - pad - 36 * S, h - pad - 36 * S)
    ]
    for sx, sy in screw_pos:
        draw.ellipse([sx - screw_r, sy - screw_r, sx + screw_r, sy + screw_r], fill=(215, 210, 200, 255), outline=(170, 165, 155, 255), width=2*S)
        # Screw cross groove
        draw.line([sx - screw_r*0.6, sy, sx + screw_r*0.6, sy], fill=(150, 145, 135, 255), width=2*S)
        draw.line([sx, sy - screw_r*0.6, sx, sy + screw_r*0.6], fill=(150, 145, 135, 255), width=2*S)

    save_sprite(im, "ohpe_chassis_bg.png")

def make_screen_bezel():
    w, h = 960 * S, 430 * S
    im = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(im)

    r = 16 * S
    # Recessed shadow frame
    draw.rounded_rectangle([2*S, 4*S, w - 2*S, h], radius=r, fill=(0, 0, 0, 60))
    # Outer dark bezel
    draw.rounded_rectangle([0, 0, w - 4*S, h - 4*S], radius=r, fill=(24, 23, 28, 255), outline=(42, 40, 48, 255), width=2*S)
    # Inner OLED screen
    screen_pad = 8 * S
    draw.rounded_rectangle([screen_pad, screen_pad, w - 4*S - screen_pad, h - 4*S - screen_pad], radius=r - 4*S, fill=(12, 11, 15, 255))

    # Subtle inner top shadow
    draw.line([screen_pad + 8*S, screen_pad + 2*S, w - 4*S - screen_pad - 8*S, screen_pad + 2*S], fill=(0, 0, 0, 120), width=3*S)

    save_sprite(im, "ohpe_screen_bezel.png")

def make_tile_btn(name, active=False, accent_col=(245, 158, 11)):
    w, h = 96 * S, 64 * S
    im = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(im)

    r = 10 * S
    # Drop shadow
    draw.rounded_rectangle([2*S, 4*S, w - 2*S, h - 2*S], radius=r, fill=(0, 0, 0, 30))

    if active:
        # Pressed / Active state
        bg_col = (255, 253, 248, 255)
        border_col = (200, 195, 182, 255)
        draw.rounded_rectangle([0, 2*S, w - 4*S, h - 4*S], radius=r, fill=bg_col, outline=border_col, width=2*S)
        # Accent pill indicator at the bottom
        pill_w = 40 * S
        pill_h = 4 * S
        px = (w - 4*S - pill_w) // 2
        py = h - 12 * S
        draw.rounded_rectangle([px, py, px + pill_w, py + pill_h], radius=2*S, fill=accent_col + (255,))
    else:
        # Default state - tactile matte off-white
        bg_col = (248, 246, 240, 255)
        border_col = (216, 212, 202, 255)
        draw.rounded_rectangle([0, 0, w - 4*S, h - 6*S], radius=r, fill=bg_col, outline=border_col, width=2*S)
        # Highlight top edge
        draw.line([r, 2*S, w - 4*S - r, 2*S], fill=(255, 255, 255, 200), width=2*S)

    save_sprite(im, name)

def make_knob(name, arc_color, val_pct=0.35):
    size = 110 * S
    im = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(im)

    cx, cy = size // 2, size // 2
    r_outer = 48 * S
    r_knob = 36 * S

    # Drop shadow of the knob
    draw.ellipse([cx - r_outer, cy - r_outer + 3*S, cx + r_outer, cy + r_outer + 3*S], fill=(0, 0, 0, 25))

    # Outer arc track (subtle light gray)
    track_col = (220, 216, 206, 255)
    start_angle = 135
    total_sweep = 270
    draw.arc([cx - r_outer + 4*S, cy - r_outer + 4*S, cx + r_outer - 4*S, cy + r_outer - 4*S],
             start=start_angle, end=start_angle + total_sweep, fill=track_col, width=6*S)

    # Active colored arc
    active_sweep = int(total_sweep * val_pct)
    draw.arc([cx - r_outer + 4*S, cy - r_outer + 4*S, cx + r_outer - 4*S, cy + r_outer - 4*S],
             start=start_angle, end=start_angle + active_sweep, fill=arc_color, width=6*S)

    # Knob Base / Body (concave light gray cylinder)
    draw.ellipse([cx - r_knob, cy - r_knob, cx + r_knob, cy + r_knob], fill=(244, 241, 235, 255), outline=(210, 205, 195, 255), width=2*S)
    # Inner depression
    r_inner = 28 * S
    draw.ellipse([cx - r_inner, cy - r_inner, cx + r_inner, cy + r_inner], fill=(236, 233, 226, 255), outline=(225, 220, 210, 255), width=1*S)

    # Indicator needle pointer
    pointer_angle = math.radians(start_angle + active_sweep)
    px = cx + (r_inner - 2*S) * math.cos(pointer_angle)
    py = cy + (r_inner - 2*S) * math.sin(pointer_angle)
    draw.line([cx, cy, px, py], fill=(40, 38, 44, 255), width=4*S)
    draw.ellipse([cx - 3*S, cy - 3*S, cx + 3*S, cy + 3*S], fill=(40, 38, 44, 255))

    save_sprite(im, name)

def make_wheels():
    w, h = 48 * S, 120 * S

    # Pitch Wheel
    im_pitch = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw_p = ImageDraw.Draw(im_pitch)
    r = 8 * S
    # Recessed slot
    draw_p.rounded_rectangle([0, 0, w, h], radius=r, fill=(28, 27, 32, 255), outline=(18, 17, 20, 255), width=2*S)
    # Ribbed thumbwheel
    ww = w - 8 * S
    wh = h - 16 * S
    wx = 4 * S
    wy = 8 * S
    draw_p.rounded_rectangle([wx, wy, wx + ww, wy + wh], radius=6*S, fill=(48, 46, 52, 255))

    # Horizontal grip ribs
    for i in range(12):
        ry = wy + (wh // 13) * (i + 1)
        draw_p.line([wx + 4*S, ry, wx + ww - 4*S, ry], fill=(24, 23, 26, 255), width=2*S)
        draw_p.line([wx + 4*S, ry - 1*S, wx + ww - 4*S, ry - 1*S], fill=(70, 68, 76, 255), width=1*S)

    # Center detent / marker line (Amber)
    mid_y = wy + wh // 2
    draw_p.rounded_rectangle([wx + 6*S, mid_y - 2*S, wx + ww - 6*S, mid_y + 2*S], radius=2*S, fill=(245, 158, 11, 255))
    save_sprite(im_pitch, "ohpe_wheel_pitch.png")

    # Mod Wheel
    im_mod = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw_m = ImageDraw.Draw(im_mod)
    draw_m.rounded_rectangle([0, 0, w, h], radius=r, fill=(28, 27, 32, 255), outline=(18, 17, 20, 255), width=2*S)
    draw_m.rounded_rectangle([wx, wy, wx + ww, wy + wh], radius=6*S, fill=(48, 46, 52, 255))
    for i in range(12):
        ry = wy + (wh // 13) * (i + 1)
        draw_m.line([wx + 4*S, ry, wx + ww - 4*S, ry], fill=(24, 23, 26, 255), width=2*S)
        draw_m.line([wx + 4*S, ry - 1*S, wx + ww - 4*S, ry - 1*S], fill=(70, 68, 76, 255), width=1*S)
    # Bottom zero position marker (Amber)
    bot_y = wy + wh - 8*S
    draw_m.rounded_rectangle([wx + 6*S, bot_y - 2*S, wx + ww - 6*S, bot_y + 2*S], radius=2*S, fill=(245, 158, 11, 255))
    save_sprite(im_mod, "ohpe_wheel_mod.png")

def make_transport_btns():
    w, h = 54 * S, 44 * S
    types = [
        ("ohpe_btn_to_start.png", "to_start"),
        ("ohpe_btn_stop.png", "stop"),
        ("ohpe_btn_play.png", "play"),
        ("ohpe_btn_rec.png", "rec"),
        ("ohpe_btn_loop.png", "loop"),
    ]
    r = 8 * S
    for fname, kind in types:
        im = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        draw = ImageDraw.Draw(im)
        draw.rounded_rectangle([2*S, 4*S, w - 2*S, h - 2*S], radius=r, fill=(0, 0, 0, 30))
        draw.rounded_rectangle([0, 0, w - 4*S, h - 4*S], radius=r, fill=(248, 246, 240, 255), outline=(214, 210, 200, 255), width=2*S)
        draw.line([r, 2*S, w - 4*S - r, 2*S], fill=(255, 255, 255, 220), width=1*S)

        cx, cy = (w - 4*S) // 2, (h - 4*S) // 2
        col = (45, 42, 48, 255)
        if kind == "to_start":
            # |◀
            draw.line([cx - 8*S, cy - 8*S, cx - 8*S, cy + 8*S], fill=col, width=3*S)
            draw.polygon([(cx + 8*S, cy - 8*S), (cx - 6*S, cy), (cx + 8*S, cy + 8*S)], fill=col)
        elif kind == "stop":
            # ■
            draw.rectangle([cx - 7*S, cy - 7*S, cx + 7*S, cy + 7*S], fill=col)
        elif kind == "play":
            # ▶
            draw.polygon([(cx - 7*S, cy - 9*S), (cx + 9*S, cy), (cx - 7*S, cy + 9*S)], fill=col)
        elif kind == "rec":
            # ● Red dot
            draw.ellipse([cx - 8*S, cy - 8*S, cx + 8*S, cy + 8*S], fill=(239, 68, 68, 255))
        elif kind == "loop":
            # ⟲ Loop arrow
            draw.arc([cx - 8*S, cy - 8*S, cx + 8*S, cy + 8*S], start=45, end=330, fill=col, width=3*S)
            draw.polygon([(cx + 6*S, cy - 10*S), (cx + 12*S, cy - 5*S), (cx + 6*S, cy)], fill=col)

        save_sprite(im, fname)

def make_circle_dials():
    size = 46 * S
    dials = [
        ("ohpe_dial_random.png", (239, 68, 68), "dice"),
        ("ohpe_dial_save.png", (245, 158, 11), "save"),
        ("ohpe_dial_settings.png", (139, 92, 246), "gear")
    ]
    for fname, ring_col, kind in dials:
        im = Image.new("RGBA", (size, size), (0, 0, 0, 0))
        draw = ImageDraw.Draw(im)
        cx, cy = size // 2, size // 2
        r = 20 * S

        # Drop shadow
        draw.ellipse([cx - r, cy - r + 3*S, cx + r, cy + r + 3*S], fill=(0, 0, 0, 25))
        # Outer color arc ring
        draw.arc([cx - r, cy - r, cx + r, cy + r], start=0, end=360, fill=ring_col + (200,), width=3*S)
        # Inner button
        r_in = 16 * S
        draw.ellipse([cx - r_in, cy - r_in, cx + r_in, cy + r_in], fill=(248, 246, 240, 255), outline=(215, 210, 200, 255), width=2*S)

        # Icon inside
        col = (45, 42, 48, 255)
        if kind == "dice":
            draw.rounded_rectangle([cx - 6*S, cy - 6*S, cx + 6*S, cy + 6*S], radius=2*S, outline=col, width=2*S)
            draw.ellipse([cx - 1*S, cy - 1*S, cx + 1*S, cy + 1*S], fill=col)
        elif kind == "save":
            draw.rectangle([cx - 5*S, cy - 7*S, cx + 5*S, cy + 7*S], outline=col, width=2*S)
            draw.line([cx, cy - 3*S, cx, cy + 3*S], fill=col, width=2*S)
            draw.line([cx - 3*S, cy + 1*S, cx, cy + 4*S], fill=col, width=2*S)
            draw.line([cx + 3*S, cy + 1*S, cx, cy + 4*S], fill=col, width=2*S)
        elif kind == "gear":
            draw.ellipse([cx - 4*S, cy - 4*S, cx + 4*S, cy + 4*S], outline=col, width=2*S)
            for a in range(0, 360, 45):
                rad = math.radians(a)
                gx1 = cx + 5 * S * math.cos(rad)
                gy1 = cy + 5 * S * math.sin(rad)
                gx2 = cx + 8 * S * math.cos(rad)
                gy2 = cy + 8 * S * math.sin(rad)
                draw.line([gx1, gy1, gx2, gy2], fill=col, width=2*S)

        save_sprite(im, fname)

def make_engine_icons():
    # 16 line art icons for the left and right rails
    icons = [
        ("ohpe_ic_piano.png", "piano"),
        ("ohpe_ic_epiano.png", "epiano"),
        ("ohpe_ic_organ.png", "organ"),
        ("ohpe_ic_strings.png", "strings"),
        ("ohpe_ic_brass.png", "brass"),
        ("ohpe_ic_synth.png", "synth"),
        ("ohpe_ic_pad.png", "pad"),
        ("ohpe_ic_sampler.png", "sampler"),
        ("ohpe_ic_stage.png", "stage"),
        ("ohpe_ic_waterfall.png", "waterfall"),
        ("ohpe_ic_chords.png", "chords"),
        ("ohpe_ic_lyrics.png", "lyrics"),
        ("ohpe_ic_arranger.png", "arranger"),
        ("ohpe_ic_sequencer.png", "sequencer"),
        ("ohpe_ic_tape.png", "tape"),
        ("ohpe_ic_drum.png", "drum")
    ]
    size = 48 * S
    col = (45, 42, 48, 255)

    for fname, kind in icons:
        im = Image.new("RGBA", (size, size), (0, 0, 0, 0))
        draw = ImageDraw.Draw(im)
        cx, cy = size // 2, size // 2

        if kind == "piano":
            # 3 piano keys
            draw.rectangle([cx - 10*S, cy - 10*S, cx - 3*S, cy + 10*S], outline=col, width=2*S)
            draw.rectangle([cx - 3*S, cy - 10*S, cx + 4*S, cy + 10*S], outline=col, width=2*S)
            draw.rectangle([cx + 4*S, cy - 10*S, cx + 11*S, cy + 10*S], outline=col, width=2*S)
            draw.rectangle([cx - 5*S, cy - 10*S, cx - 1*S, cy + 2*S], fill=col)
            draw.rectangle([cx + 2*S, cy - 10*S, cx + 6*S, cy + 2*S], fill=col)
        elif kind == "epiano":
            # Sine waveform with dot
            draw.arc([cx - 12*S, cy - 8*S, cx, cy + 8*S], start=180, end=360, fill=col, width=2*S)
            draw.arc([cx, cy - 8*S, cx + 12*S, cy + 8*S], start=0, end=180, fill=col, width=2*S)
        elif kind == "organ":
            # 3 vertical organ pipe lines
            draw.line([cx - 8*S, cy - 10*S, cx - 8*S, cy + 10*S], fill=col, width=3*S)
            draw.line([cx, cy - 14*S, cx, cy + 10*S], fill=col, width=3*S)
            draw.line([cx + 8*S, cy - 6*S, cx + 8*S, cy + 10*S], fill=col, width=3*S)
        elif kind == "strings":
            # Violin f-hole stylized
            draw.arc([cx - 8*S, cy - 10*S, cx + 4*S, cy], start=180, end=360, fill=col, width=2*S)
            draw.arc([cx - 4*S, cy, cx + 8*S, cy + 10*S], start=0, end=180, fill=col, width=2*S)
        elif kind == "brass":
            # Trumpet bell stylized
            draw.polygon([(cx - 10*S, cy - 4*S), (cx + 8*S, cy - 10*S), (cx + 8*S, cy + 10*S), (cx - 10*S, cy + 4*S)], outline=col)
        elif kind == "synth":
            # Analog pulse wave
            draw.line([cx - 12*S, cy + 6*S, cx - 4*S, cy + 6*S], fill=col, width=2*S)
            draw.line([cx - 4*S, cy + 6*S, cx - 4*S, cy - 6*S], fill=col, width=2*S)
            draw.line([cx - 4*S, cy - 6*S, cx + 4*S, cy - 6*S], fill=col, width=2*S)
            draw.line([cx + 4*S, cy - 6*S, cx + 4*S, cy + 6*S], fill=col, width=2*S)
            draw.line([cx + 4*S, cy + 6*S, cx + 12*S, cy + 6*S], fill=col, width=2*S)
        elif kind == "pad":
            # Floating clouds / soft concentric circles
            draw.ellipse([cx - 10*S, cy - 6*S, cx + 2*S, cy + 6*S], outline=col, width=2*S)
            draw.ellipse([cx - 2*S, cy - 8*S, cx + 10*S, cy + 4*S], outline=col, width=2*S)
        elif kind == "sampler":
            # Soundwave burst
            for dx in [-10, -6, -2, 2, 6, 10]:
                h_line = (14 - abs(dx)) * S
                draw.line([cx + dx*S, cy - h_line, cx + dx*S, cy + h_line], fill=col, width=2*S)
        elif kind == "stage":
            # Cockpit / console monitor
            draw.rounded_rectangle([cx - 11*S, cy - 8*S, cx + 11*S, cy + 8*S], radius=2*S, outline=col, width=2*S)
            draw.line([cx - 11*S, cy + 4*S, cx + 11*S, cy + 4*S], fill=col, width=1*S)
        elif kind == "waterfall":
            # Falling bars
            draw.rectangle([cx - 8*S, cy - 10*S, cx - 4*S, cy + 4*S], fill=col)
            draw.rectangle([cx - 2*S, cy - 6*S, cx + 2*S, cy + 10*S], fill=col)
            draw.rectangle([cx + 4*S, cy - 12*S, cx + 8*S, cy - 2*S], fill=col)
        elif kind == "chords":
            # Chord triad dots on stave
            draw.line([cx - 10*S, cy - 6*S, cx + 10*S, cy - 6*S], fill=col, width=1*S)
            draw.line([cx - 10*S, cy, cx + 10*S, cy], fill=col, width=1*S)
            draw.line([cx - 10*S, cy + 6*S, cx + 10*S, cy + 6*S], fill=col, width=1*S)
            draw.ellipse([cx - 6*S, cy - 8*S, cx - 2*S, cy - 4*S], fill=col)
            draw.ellipse([cx - 2*S, cy - 2*S, cx + 2*S, cy + 2*S], fill=col)
            draw.ellipse([cx + 2*S, cy + 4*S, cx + 6*S, cy + 8*S], fill=col)
        elif kind == "lyrics":
            # Document lines
            draw.rounded_rectangle([cx - 8*S, cy - 10*S, cx + 8*S, cy + 10*S], radius=2*S, outline=col, width=2*S)
            draw.line([cx - 5*S, cy - 5*S, cx + 5*S, cy - 5*S], fill=col, width=2*S)
            draw.line([cx - 5*S, cy, cx + 5*S, cy], fill=col, width=2*S)
            draw.line([cx - 5*S, cy + 5*S, cx + 1*S, cy + 5*S], fill=col, width=2*S)
        elif kind == "arranger":
            # Play arrangement blocks
            draw.rectangle([cx - 9*S, cy - 7*S, cx - 1*S, cy - 1*S], fill=col)
            draw.rectangle([cx + 1*S, cy - 7*S, cx + 9*S, cy - 1*S], outline=col, width=2*S)
            draw.rectangle([cx - 9*S, cy + 1*S, cx + 3*S, cy + 7*S], outline=col, width=2*S)
        elif kind == "sequencer":
            # 2x3 step sequencer grid dots
            for r_idx in [-4, 4]:
                for c_idx in [-8, 0, 8]:
                    draw.ellipse([cx + c_idx*S - 2*S, cy + r_idx*S - 2*S, cx + c_idx*S + 2*S, cy + r_idx*S + 2*S], fill=col)
        elif kind == "tape":
            # 2 tape reels connected
            draw.ellipse([cx - 10*S, cy - 5*S, cx - 2*S, cy + 5*S], outline=col, width=2*S)
            draw.ellipse([cx + 2*S, cy - 5*S, cx + 10*S, cy + 5*S], outline=col, width=2*S)
            draw.line([cx - 6*S, cy + 5*S, cx + 6*S, cy + 5*S], fill=col, width=2*S)
        elif kind == "drum":
            # Snare drum & sticks
            draw.ellipse([cx - 8*S, cy - 2*S, cx + 8*S, cy + 6*S], outline=col, width=2*S)
            draw.line([cx - 8*S, cy + 2*S, cx - 8*S, cy + 8*S], fill=col, width=2*S)
            draw.line([cx + 8*S, cy + 2*S, cx + 8*S, cy + 8*S], fill=col, width=2*S)
            draw.line([cx - 8*S, cy - 9*S, cx - 1*S, cy], fill=col, width=2*S)
            draw.line([cx + 8*S, cy - 9*S, cx + 1*S, cy], fill=col, width=2*S)

        save_sprite(im, fname)

def main():
    print("Generating OHPE Design Suite...")
    make_chassis()
    make_screen_bezel()
    make_tile_btn("ohpe_tile_normal.png", active=False)
    make_tile_btn("ohpe_tile_active.png", active=True, accent_col=(245, 158, 11))

    # 4 Signature macro encoders matching Ohpe colors
    make_knob("ohpe_knob_red.png", (239, 68, 68), val_pct=0.35)      # 🔴 Shape / Split Bal
    make_knob("ohpe_knob_yellow.png", (245, 158, 11), val_pct=0.50)  # 🟡 Detune / Transpose
    make_knob("ohpe_knob_purple.png", (139, 92, 246), val_pct=0.70)  # 🟣 Cutoff
    make_knob("ohpe_knob_pink.png", (236, 72, 153), val_pct=0.45)    # 🌸 Resonance / Reverb

    make_wheels()
    make_transport_btns()
    make_circle_dials()
    make_engine_icons()
    print("OHPE Design Suite successfully generated and synchronized!")

if __name__ == "__main__":
    main()
