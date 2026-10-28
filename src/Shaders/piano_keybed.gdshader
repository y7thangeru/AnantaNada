shader_type canvas_item;
render_mode blend_mix;

// --- ANANTANADA SYNTH-RUNNER 61-KEY DIEGETIC KEYBED SHADER (V2 POLISHED) ---
// Exact Roland Go:Keys 3 Geometry: 36 White Keys (Full Length), 25 Black Keys (60% Length)
// Elevated 3D relief, drop shadows, Split Point C4 marker, velocity-mapped neon glow.

uniform vec4 color_white_key : source_color = vec4(0.92, 0.94, 0.97, 1.0);      // Crisp Ivory
uniform vec4 color_white_shadow : source_color = vec4(0.78, 0.82, 0.87, 1.0);   // White key shadow
uniform vec4 color_black_key : source_color = vec4(0.08, 0.09, 0.12, 1.0);      // Deep Ebony Obsidian
uniform vec4 color_black_bevel : source_color = vec4(0.22, 0.26, 0.34, 1.0);    // Specular ridge
uniform vec4 color_neon_cyan : source_color = vec4(0.0, 0.96, 0.83, 1.0);       // Zone B Lead (#00f5d4)
uniform vec4 color_neon_amber : source_color = vec4(1.0, 0.72, 0.01, 1.0);      // Zone A Bass (#ffb703)
uniform vec4 color_slit_dark : source_color = vec4(0.03, 0.04, 0.05, 1.0);      // Slit gap
uniform vec4 color_split_marker : source_color = vec4(1.0, 0.20, 0.35, 1.0);    // C4 Split Marker

uniform float black_key_length : hint_range(0.5, 0.75) = 0.60;
uniform float black_key_width : hint_range(0.4, 0.7) = 0.58;
uniform float key_slit_width : hint_range(0.01, 0.08) = 0.035;
uniform float glow_intensity : hint_range(1.0, 5.0) = 3.0;
uniform float beat_pulse : hint_range(0.0, 1.0) = 0.0;
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;

// Key state texture: 64x1 8-bit texture where R=velocity (0..1), G=hand/channel (0=Left Amber, 1=Right Cyan), B=hit_impact
uniform sampler2D key_states : hint_default_black, filter_nearest;

const float TOTAL_WHITE_KEYS = 36.0;

// Returns black key center position in white-key units [0..35]
// Octave offsets: C#(1.0), D#(2.0), F#(4.0), G#(5.0), A#(6.0)
float get_black_key_center(int index) {
    int oct = index / 5;
    int idx_in_oct = index % 5;
    float base_w = float(oct * 7);
    if (idx_in_oct == 0) return base_w + 1.0; // C#
    if (idx_in_oct == 1) return base_w + 2.0; // D#
    if (idx_in_oct == 2) return base_w + 4.0; // F#
    if (idx_in_oct == 3) return base_w + 5.0; // G#
    return base_w + 6.0;                      // A#
}

// Maps black key index (0..24) to MIDI note number (37 to 94)
int get_black_key_midi(int index) {
    int oct = index / 5;
    int idx_in_oct = index % 5;
    const int offsets[5] = int[](1, 3, 6, 8, 10);
    return 36 + (oct * 12) + offsets[idx_in_oct];
}

// Maps white key index (0..35) to MIDI note number (36 to 96)
int get_white_key_midi(int w_idx) {
    int oct = w_idx / 7;
    int rem = w_idx % 7;
    const int offsets[7] = int[](0, 2, 4, 5, 7, 9, 11);
    return 36 + (oct * 12) + offsets[rem];
}

void fragment() {
    vec2 uv = UV;

    // 1. Kinetic Slanted Tilt (Persona 5 ~8° slant)
    float slant = (uv.y - 0.5) * 0.035;
    float tilted_x = clamp(uv.x + slant, 0.0, 1.0);

    // Coordinate in white-key units [0.0 to 36.0]
    float w_coord = tilted_x * TOTAL_WHITE_KEYS;
    int w_idx = int(floor(w_coord));
    float w_frac = fract(w_coord);

    bool is_black = false;
    int active_midi_note = get_white_key_midi(w_idx);
    float black_key_dist = 999.0;
    int active_black_idx = -1;

    // 2. Black Key Detection (25 black keys spanning from y=0.0 to y=black_key_length)
    if (uv.y < black_key_length) {
        float half_bk_w = black_key_width * 0.5;

        // Check closest black keys in current octave and boundaries
        int oct = int(floor(w_coord / 7.0));
        int bk_start = max(0, oct * 5 - 1);
        int bk_end = min(24, (oct + 1) * 5);

        for (int k = bk_start; k <= bk_end; k++) {
            float bk_center = get_black_key_center(k);
            float dist = abs(w_coord - bk_center);

            if (dist < half_bk_w) {
                is_black = true;
                black_key_dist = dist;
                active_black_idx = k;
                active_midi_note = get_black_key_midi(k);
                break;
            }
        }
    }

    // 3. Sample Key State Texture (Note 36 to 96 -> Exact texel center in 64x1 texture)
    float sample_u = (float(active_midi_note - 36) + 0.5) / 64.0;
    vec4 state = texture(key_states, vec2(sample_u, 0.5));
    float velocity = state.r;
    float hand_split = state.g; // 0.0 = Zone A Amber, 1.0 = Zone B Cyan
    float impact = state.b;

    vec4 active_neon = mix(color_neon_amber, color_neon_cyan, hand_split);
    vec4 col;

    // 4. Surface Rendering
    if (is_black) {
        // --- BLACK KEY RENDERING ---
        col = color_black_key;

        // 3D Bevel highlight on top edge and center ridge
        float half_w = black_key_width * 0.5;
        float norm_x = black_key_dist / half_w; // 0 at center, 1 at edge
        float ridge = smoothstep(0.0, 0.35, 1.0 - norm_x) * 0.25;
        col = mix(col, color_black_bevel, ridge);

        // Dark border slit
        float edge_slit = smoothstep(0.85, 1.0, norm_x);
        col = mix(col, color_slit_dark, edge_slit);

        // Bottom cap curvature (at y approaching black_key_length)
        float cap_dist = (black_key_length - uv.y) / black_key_length;
        if (cap_dist < 0.06) {
            col = mix(color_slit_dark, col, cap_dist / 0.06);
        }

        // Active velocity glow (neon edge rim + glowing face)
        if (velocity > 0.01) {
            float glow_mask = mix(0.7, 1.0, edge_slit);
            col = mix(col, active_neon, velocity * 0.85 * glow_mask);
            col += active_neon * impact * 1.5;
        }
    } else {
        // --- WHITE KEY RENDERING ---
        // Vertical gradient: subtle shading towards top under fallboard
        float vert_grad = smoothstep(0.0, 1.0, uv.y);
        col = mix(color_white_shadow, color_white_key, vert_grad * 0.4 + 0.6);

        // Inter-key slit gap between adjacent white keys
        float slit_dist = min(w_frac, 1.0 - w_frac);
        float slit_factor = smoothstep(0.0, key_slit_width, slit_dist);
        col = mix(color_slit_dark, col, slit_factor);

        // Soft drop shadow cast by nearby black keys on upper 60% of keybed
        if (uv.y < black_key_length + 0.04) {
            float shadow_intensity = 0.0;
            int oct = int(floor(w_coord / 7.0));
            int bk_start = max(0, oct * 5 - 1);
            int bk_end = min(24, (oct + 1) * 5);

            for (int k = bk_start; k <= bk_end; k++) {
                float bk_center = get_black_key_center(k);
                float dist_to_bk = abs(w_coord - bk_center);
                float shadow_reach = black_key_width * 0.5 + 0.16;
                if (dist_to_bk < shadow_reach && dist_to_bk > black_key_width * 0.5) {
                    float s = (shadow_reach - dist_to_bk) / 0.16;
                    shadow_intensity = max(shadow_intensity, s * 0.28);
                }
            }
            col.rgb -= vec3(shadow_intensity);
        }

        // Bottom lip chamfer (at y > 0.96)
        if (uv.y > 0.96) {
            float lip = (uv.y - 0.96) / 0.04;
            col.rgb -= vec3(lip * 0.12);
        }

        // Active velocity glow
        if (velocity > 0.01) {
            vec4 glow = active_neon * (velocity * glow_intensity * 0.75 + impact * 1.8);
            col = mix(col, glow, velocity * 0.75);
            // Attack strike flash at top
            float strike_flash = (1.0 - smoothstep(0.0, 0.4, uv.y)) * impact * 1.2;
            col += active_neon * strike_flash;
        } else {
            // Subtle beat pulse
            col.rgb += active_neon.rgb * beat_pulse * 0.03;
        }
    }

    // 5. SPLIT POINT C4 DIVIDER BAND (White Key #14 -> Note 60)
    // In white key units, boundary is at exactly 14.0
    float split_u = 14.0;
    float dist_split = abs(w_coord - split_u);
    if (dist_split < 0.08) {
        float split_glow = smoothstep(0.08, 0.0, dist_split);
        col = mix(col, color_split_marker, split_glow * 0.95);
        col += color_split_marker * 0.6;
    }

    // 6. Emergency Glitch Flash
    if (glitch_intensity > 0.01) {
        col.rgb = mix(col.rgb, vec3(1.0, 0.1, 0.15), glitch_intensity);
    }

    // Scanlines
    float scanline = sin(uv.y * 380.0) * 0.015;
    col.rgb -= vec3(scanline);

    COLOR = col;
}
