shader_type canvas_item;
render_mode blend_mix;

// --- TACTICAL TECHWEAR & HAZARD PANEL SHADER ---
// Arknights / Zenless Zone Zero aesthetic: Chamfered polygon corners,
// perimeter hazard stripes, CRT scanlines, and emergency glitch flash.

uniform vec4 panel_bg_color : source_color = vec4(0.07, 0.08, 0.11, 0.95);
uniform vec4 border_color : source_color = vec4(0.18, 0.22, 0.30, 1.0);
uniform vec4 accent_color : source_color = vec4(0.0, 0.96, 0.83, 1.0);        // Cyber Cyan
uniform vec4 hazard_yellow : source_color = vec4(1.0, 0.82, 0.05, 1.0);
uniform vec4 hazard_black : source_color = vec4(0.06, 0.06, 0.08, 1.0);

uniform bool enable_hazard_stripes = false;
uniform bool is_panic_panel = false;
uniform float border_width : hint_range(1.0, 12.0) = 3.0;
uniform float corner_cut_size : hint_range(0.0, 32.0) = 14.0;
uniform float scanline_strength : hint_range(0.0, 0.1) = 0.025;
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float pulse_intensity : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec2 size = 1.0 / SCREEN_PIXEL_SIZE;
    vec2 pos = UV * size;
    vec2 uv = UV;

    // Corner chamfers (Tactical 45° cuts on top-right and bottom-left)
    float cut_tr = (size.x - pos.x) + pos.y;
    float cut_bl = pos.x + (size.y - pos.y);

    if (cut_tr < corner_cut_size || cut_bl < corner_cut_size) {
        discard;
    }

    // Distance to outer boundary including chamfers
    float dist_edge = min(min(pos.x, size.x - pos.x), min(pos.y, size.y - pos.y));
    float dist_tr = abs(cut_tr - corner_cut_size);
    float dist_bl = abs(cut_bl - corner_cut_size);
    float perimeter_dist = min(dist_edge, min(dist_tr, dist_bl));

    vec4 col = panel_bg_color;

    // Outer hazard frame or border
    if (enable_hazard_stripes) {
        // If hazard is enabled, render diagonal yellow/black hazard stripes around the border ring
        if (perimeter_dist < border_width * 2.5) {
            float stripe = fract((pos.x + pos.y) / 20.0);
            col = (stripe > 0.5) ? hazard_yellow : hazard_black;
        } else {
            // Inner dark cockpit core so text remains 100% legible
            col = is_panic_panel ? vec4(0.18, 0.04, 0.06, 0.98) : panel_bg_color;
        }
    } else {
        // Standard tactical border line
        if (perimeter_dist < border_width) {
            col = border_color;
        }
    }

    // Breathing pulse effect (for incoming chords or active triggers)
    if (pulse_intensity > 0.01) {
        col.rgb += accent_color.rgb * pulse_intensity * 0.25;
        if (perimeter_dist < border_width * 1.5) {
            col = mix(col, accent_color, pulse_intensity * 0.8);
        }
    }

    // CRT scanline pass
    float scanline = sin(uv.y * 480.0) * scanline_strength;
    col.rgb -= vec3(scanline);

    // Emergency Panic glitch flash
    if (glitch_intensity > 0.01) {
        col.rgb = mix(col.rgb, vec4(1.0, 0.1, 0.15, 1.0).rgb, glitch_intensity);
        float noise = fract(sin(dot(uv + vec2(TIME, 0.0), vec2(12.9898, 78.233))) * 43758.5453);
        col.rgb += vec3(noise * glitch_intensity * 0.35);
    }

    COLOR = col;
}
