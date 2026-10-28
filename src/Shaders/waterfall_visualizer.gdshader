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

void fragment() {
    vec2 uv = UV;

    // 1. Trapezoidal 3D Perspective Projection (Runway tilt)
    float runway_width = mix(trapezoid_perspective, 1.0, uv.y);
    float centered_x = (uv.x - 0.5) / runway_width + 0.5;

    // Render gutter border outside the trapezoidal runway boundary
    if (centered_x < 0.0 || centered_x > 1.0) {
        COLOR = vec4(0.02, 0.025, 0.035, 1.0);
    } else {
        // 2. 61-Key Lane Geometry
        float lane_pos = centered_x * 61.0;
        int lane_idx = int(floor(lane_pos));
        float lane_frac = fract(lane_pos);

        // Alternate lane shading for depth
        vec4 col = (lane_idx % 2 == 0) ? background_color : track_lane_even;

        // Subtle lane divider lines
        float lane_edge = min(lane_frac, 1.0 - lane_frac);
        float divider = smoothstep(0.0, 0.06, lane_edge);
        col = mix(grid_line_color, col, divider);

        // 3. Beat Marker Horizontal Bands
        float beat_line = fract((1.0 - uv.y) * beat_frequency + TIME * (scroll_speed * 0.4));
        float beat_mask = smoothstep(0.0, 0.012, beat_line) * smoothstep(0.025, 0.012, beat_line);
        col = mix(col, beat_marker_color * (1.0 + beat_pulse * 1.6), beat_mask * 0.65);

        // 4. Note Stream Sampling (Guaranteed black when empty)
        vec2 stream_uv = vec2(clamp(centered_x, 0.0, 1.0), uv.y);
        vec4 note_data = texture(note_stream_texture, stream_uv);
        float note_velocity = note_data.r;
        float note_channel  = note_data.g;

        // Active falling note rendering
        if (note_velocity > 0.05) {
            vec4 note_color = mix(vec4(1.0, 0.72, 0.01, 1.0), vec4(0.0, 0.96, 0.83, 1.0), note_channel);
            
            // Rounded note pill within lane
            float pill_dist = abs(lane_frac - 0.5);
            float pill_mask = smoothstep(0.46, 0.35, pill_dist);

            // Core intensity + soft bloom halo
            float intensity = note_velocity * 1.8;
            col = mix(col, note_color * intensity, pill_mask * 0.95);
            col += note_color * (1.0 - smoothstep(0.0, 0.45, pill_dist)) * 0.4;
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
