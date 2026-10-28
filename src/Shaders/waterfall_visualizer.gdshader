shader_type canvas_item;
render_mode blend_mix;

// --- ANANTANADA CYBER-CASCADE WATERFALL NOTE VISUALIZER ---
// Hi-Fi Rush beat rhythm pulse + Hit Beam collision neon burst.

uniform vec4 background_color : source_color = vec4(0.043, 0.051, 0.063, 0.98); // Deep Obsidian
uniform vec4 grid_line_color : source_color = vec4(0.12, 0.16, 0.22, 0.5);
uniform vec4 hit_beam_color : source_color = vec4(0.0, 0.96, 0.83, 1.0);        // Cyber Cyan
uniform vec4 beat_marker_color : source_color = vec4(1.0, 0.72, 0.01, 0.45);    // Electric Amber
uniform vec4 spark_flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

uniform float scroll_speed : hint_range(0.2, 5.0) = 1.3;
uniform float beat_frequency : hint_range(0.5, 10.0) = 2.0;
uniform float strike_line_y : hint_range(0.8, 1.0) = 0.95;
uniform float bloom_spread : hint_range(0.01, 0.25) = 0.10;
uniform float beat_pulse : hint_range(0.0, 1.0) = 0.0;
uniform float hit_beam_flash : hint_range(0.0, 2.0) = 0.0;

// Note stream buffer
uniform sampler2D note_stream_texture : filter_linear;

void fragment() {
    vec2 uv = UV;

    // Perspective slant
    float perspective = mix(0.90, 1.0, uv.y);
    float centered_x = (uv.x - 0.5) / perspective + 0.5;

    vec4 col = background_color;

    // 61-Key Column Grid
    float key_grid = fract(centered_x * 61.0);
    float key_line = smoothstep(0.0, 0.05, key_grid) * smoothstep(1.0, 0.95, key_grid);
    col = mix(grid_line_color, col, key_line);

    // BPM Beat Bar Horizontal Markers
    float beat_line = fract((1.0 - uv.y) * beat_frequency + TIME * (scroll_speed * 0.5));
    float beat_mask = smoothstep(0.0, 0.02, beat_line) * smoothstep(0.04, 0.02, beat_line);
    col = mix(col, beat_marker_color * (1.0 + beat_pulse * 1.5), beat_mask * 0.75);

    // Note stream
    vec2 stream_uv = vec2(clamp(centered_x, 0.0, 1.0), uv.y);
    vec4 note_data = texture(note_stream_texture, stream_uv);

    float note_velocity = note_data.r;
    float note_channel  = note_data.g;

    if (note_velocity > 0.02) {
        vec4 note_color = mix(vec4(1.0, 0.72, 0.01, 1.0), vec4(0.0, 0.96, 0.83, 1.0), note_channel);
        float trail_intensity = note_velocity * 2.0;
        col = mix(col, note_color, clamp(trail_intensity, 0.0, 1.0));
    }

    // HIT BEAM (Target Collision Strike Line)
    float strike_dist = abs(uv.y - strike_line_y);
    float strike_beam = smoothstep(bloom_spread, 0.0, strike_dist);
    vec4 beam_col = mix(hit_beam_color, spark_flash_color, hit_beam_flash * 0.5);
    col += beam_col * strike_beam * (1.0 + hit_beam_flash * 1.8);

    // Vignette
    float vignette = distance(uv, vec2(0.5, 0.5));
    col.rgb *= smoothstep(0.95, 0.30, vignette);

    COLOR = col;
}
