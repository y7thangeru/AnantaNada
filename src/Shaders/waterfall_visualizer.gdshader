shader_type canvas_item;
render_mode blend_mix;

// --- ANANTANADA CYBER-CASCADE: WATERFALL VISUALIZER SHADER (V2 POLISHED) ---
// 3D Trapezoidal Rhythm Game Runway (Guitar Hero / SDVX / Muse Dash)
// Deep obsidian background, 61 crisp lane tracks, vibrant hit beam bloom.

uniform vec4 background_color : source_color = vec4(0.043, 0.051, 0.067, 0.98); // Deep Obsidian #0b0d11
uniform vec4 track_lane_even : source_color = vec4(0.055, 0.067, 0.086, 0.98);  // Alternate lane #0e1116
uniform vec4 grid_line_color : source_color = vec4(0.14, 0.18, 0.25, 0.55);     // Subtle lane separator
uniform vec4 hit_beam_color : source_color = vec4(0.0, 0.96, 0.83, 1.0);        // Cyber Cyan strike line
uniform vec4 beat_marker_color : source_color = vec4(1.0, 0.72, 0.01, 0.40);    // Electric Amber beat bar
uniform vec4 spark_flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

uniform float scroll_speed : hint_range(0.2, 5.0) = 1.3;
uniform float beat_frequency : hint_range(0.5, 10.0) = 2.0;
uniform float strike_line_y : hint_range(0.85, 0.98) = 0.94;
uniform float bloom_spread : hint_range(0.01, 0.20) = 0.06;
uniform float beat_pulse : hint_range(0.0, 1.0) = 0.0;
uniform float hit_beam_flash : hint_range(0.0, 2.0) = 0.0;
uniform float trapezoid_perspective : hint_range(0.5, 1.0) = 0.76; // 18° inward tilt (top narrower)

// Note stream buffer: R=velocity (0..1), G=hand/channel (0=Left Amber, 1=Right Cyan)
// hint_default_black ensures unassigned texture defaults to 0.0 (pitch black)
uniform sampler2D note_stream_texture : hint_default_black, filter_linear;
// Live key state buffer: 64x1 texture driven directly by physical Roland keybed
uniform sampler2D key_states : hint_default_black, filter_nearest;

int get_white_key_midi_wf(int w_idx) {
    int oct = w_idx / 7;
    int rem = w_idx % 7;
    const int offsets[7] = int[](0, 2, 4, 5, 7, 9, 11);
    return 36 + (oct * 12) + offsets[rem];
}

float get_black_key_center_wf(int index) {
    int oct = index / 5;
    int idx_in_oct = index % 5;
    float base_w = float(oct * 7);
    if (idx_in_oct == 0) return base_w + 1.0;
    if (idx_in_oct == 1) return base_w + 2.0;
    if (idx_in_oct == 2) return base_w + 4.0;
    if (idx_in_oct == 3) return base_w + 5.0;
    return base_w + 6.0;
}

int get_black_key_midi_wf(int index) {
    int oct = index / 5;
    int idx_in_oct = index % 5;
    const int offsets[5] = int[](1, 3, 6, 8, 10);
    return 36 + (oct * 12) + offsets[idx_in_oct];
}

void fragment() {
    vec2 uv = UV;

    // 1. Trapezoidal 3D Perspective Projection (Runway tilt)
    float runway_width = mix(trapezoid_perspective, 1.0, uv.y);
    float centered_x = (uv.x - 0.5) / runway_width + 0.5;

    // Render gutter border outside the trapezoidal runway boundary
    if (centered_x < 0.0 || centered_x > 1.0) {
        COLOR = vec4(0.02, 0.025, 0.035, 1.0);
    } else {
        // 2. 36-White-Key Lane Geometry (Aligned 1:1 with Roland Keybed below)
        float lane_pos = centered_x * 36.0;
        int lane_idx = clamp(int(floor(lane_pos)), 0, 35);
        float lane_frac = fract(lane_pos);

        // Alternate lane shading for depth
        vec4 col = (lane_idx % 2 == 0) ? background_color : track_lane_even;

        // Subtle lane divider lines aligned exactly with key slits
        float lane_edge = min(lane_frac, 1.0 - lane_frac);
        float divider = smoothstep(0.0, 0.06, lane_edge);
        col = mix(grid_line_color, col, divider);

        // 3. Beat Marker Horizontal Bands
        float beat_line = fract((1.0 - uv.y) * beat_frequency + TIME * (scroll_speed * 0.4));
        float beat_mask = smoothstep(0.0, 0.012, beat_line) * smoothstep(0.025, 0.012, beat_line);
        col = mix(col, beat_marker_color * (1.0 + beat_pulse * 1.6), beat_mask * 0.65);

        // 4. Live Key Reaction Laser Column (Rising from strike line when physical key is pressed)
        int white_midi = get_white_key_midi_wf(lane_idx);
        float white_u = (float(white_midi - 36) + 0.5) / 64.0;
        vec4 live_white_data = texture(key_states, vec2(white_u, 0.5));

        float max_live_vel = live_white_data.r;
        float max_live_split = live_white_data.g;
        float max_live_impact = live_white_data.b;
        float pill_dist = abs(lane_frac - 0.5);

        // Also check adjacent black keys
        for (int k = 0; k < 25; k++) {
            float bk_center = get_black_key_center_wf(k);
            if (abs(lane_pos - bk_center) < 0.55) {
                int bk_midi = get_black_key_midi_wf(k);
                float bk_u = (float(bk_midi - 36) + 0.5) / 64.0;
                vec4 bk_data = texture(key_states, vec2(bk_u, 0.5));
                if (bk_data.r > max_live_vel) {
                    max_live_vel = bk_data.r;
                    max_live_split = bk_data.g;
                    max_live_impact = bk_data.b;
                    pill_dist = abs(lane_pos - bk_center);
                }
            }
        }

        if (max_live_vel > 0.01) {
            vec4 live_color = mix(vec4(1.0, 0.72, 0.01, 1.0), vec4(0.0, 0.96, 0.83, 1.0), max_live_split);
            float lane_mask = smoothstep(0.48, 0.15, pill_dist);
            // Laser beam shoots upward from the strike line (uv.y <= strike_line_y)
            float beam_reach = smoothstep(0.0, strike_line_y, uv.y);
            col += live_color * (max_live_vel * 2.2 + max_live_impact * 2.0) * beam_reach * lane_mask;
            col += spark_flash_color * max_live_impact * smoothstep(0.06, 0.0, abs(uv.y - strike_line_y));
        }

        // 5. HIT BEAM Strike Zone (Target collision bar at y = 0.94)
        float strike_dist = abs(uv.y - strike_line_y);
        float strike_beam = smoothstep(bloom_spread, 0.0, strike_dist);
        vec4 beam_col = mix(hit_beam_color, spark_flash_color, hit_beam_flash * 0.5);
        col += beam_col * strike_beam * (1.2 + hit_beam_flash * 1.8);

        // Strike line comic sparks along the hit line
        if (strike_dist < 0.008) {
            col += spark_flash_color * (0.8 + hit_beam_flash * 1.0);
        }

        // 6. Horizon atmospheric haze (darker at top horizon)
        col.rgb *= smoothstep(0.0, 0.30, uv.y) * 0.4 + 0.6;

        // 7. Vignette along runway edges
        float edge_fade = min(centered_x, 1.0 - centered_x);
        col.rgb *= smoothstep(0.0, 0.08, edge_fade) * 0.6 + 0.4;

        COLOR = col;
    }
}
