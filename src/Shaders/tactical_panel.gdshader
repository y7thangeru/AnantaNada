shader_type canvas_item;
render_mode blend_mix;

// --- TACTICAL TECHWEAR & HAZARD PANEL SHADER ---
// Arknights / Zenless Zone Zero aesthetic: Caution hazard stripes, hex grid & scanlines.

uniform vec4 panel_bg_color : source_color = vec4(0.066, 0.078, 0.098, 0.95); // #111419
uniform vec4 border_color : source_color = vec4(0.14, 0.17, 0.22, 1.0);
uniform vec4 accent_color : source_color = vec4(0.0, 0.96, 0.83, 1.0);        // Cyber Cyan
uniform vec4 hazard_yellow : source_color = vec4(1.0, 0.82, 0.05, 1.0);
uniform vec4 hazard_black : source_color = vec4(0.08, 0.08, 0.08, 1.0);

uniform bool enable_hazard_stripes = false;
uniform bool is_panic_panel = false;
uniform float border_width : hint_range(1.0, 10.0) = 2.0;
uniform float corner_cut_size : hint_range(0.0, 40.0) = 14.0;
uniform float scanline_strength : hint_range(0.0, 0.1) = 0.03;
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec2 size = 1.0 / SCREEN_PIXEL_SIZE;
    vec2 pos = UV * size;
    vec2 uv = UV;

    // Corner chamfer / cut-off corners (Tactical Polygon aesthetic)
    float cut_tl = pos.x + pos.y;
    float cut_tr = (size.x - pos.x) + pos.y;
    float cut_br = (size.x - pos.x) + (size.y - pos.y);
    float cut_bl = pos.x + (size.y - pos.y);

    if (cut_tr < corner_cut_size || cut_bl < corner_cut_size) {
        discard;
    }

    vec4 col = panel_bg_color;

    // Hazard Caution Stripes (Yellow / Black 45-degree diagonal pattern)
    if (enable_hazard_stripes) {
        float stripe = fract((pos.x + pos.y) / 24.0);
        col = (stripe > 0.5) ? hazard_yellow : hazard_black;
    }

    // Border Frame calculation
    bool is_border = (pos.x < border_width || pos.x > size.x - border_width ||
                      pos.y < border_width || pos.y > size.y - border_width ||
                      abs(cut_tr - corner_cut_size) < border_width * 1.4 ||
                      abs(cut_bl - corner_cut_size) < border_width * 1.4);

    if (is_border) {
        col = is_panic_panel ? vec4(0.93, 0.14, 0.23, 1.0) : border_color;
    }

    // Scanlines
    float scanline = sin(uv.y * 360.0) * scanline_strength;
    col.rgb -= vec3(scanline);

    // Glitch flash (triggered during Emergency Purge / Panic)
    if (glitch_intensity > 0.01) {
        col.rgb = mix(col.rgb, vec3(1.0, 0.1, 0.15), glitch_intensity);
        col.rgb += vec3(fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453) * glitch_intensity * 0.4);
    }

    COLOR = col;
}
