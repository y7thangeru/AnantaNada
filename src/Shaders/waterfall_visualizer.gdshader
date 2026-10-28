shader_type canvas_item;
render_mode blend_mix;

// --- ANANTANADA WATERFALL NOTE VISUALIZER SHADER ---
// High-energy kinetic falling notes stream flowing into Roland Go:Keys 3 keybed.

uniform vec4 background_color : source_color = vec4(0.043, 0.051, 0.063, 0.95); // Deep Obsidian (#0b0d10)
uniform vec4 grid_line_color : source_color = vec4(0.12, 0.15, 0.20, 0.4);
uniform vec4 strike_line_color : source_color = vec4(0.0, 0.96, 0.83, 1.0);     // Cyber Cyan
uniform vec4 beat_marker_color : source_color = vec4(1.0, 0.72, 0.01, 0.35);    // Electric Amber

uniform float scroll_speed : hint_range(0.2, 5.0) = 1.2;
uniform float beat_frequency : hint_range(0.5, 10.0) = 2.0;
uniform float strike_line_y : hint_range(0.8, 1.0) = 0.96;
uniform float bloom_spread : hint_range(0.01, 0.2) = 0.08;

// Note stream buffer: Rendered note trail texture (X = 61 keys, Y = time history)
uniform sampler2D note_stream_texture : filter_linear;

void fragment() {
    vec2 uv = UV;

    // Slanted diegetic perspective: slight wedge expansion towards the bottom
    float perspective = mix(0.92, 1.0, uv.y);
    float centered_x = (uv.x - 0.5) / perspective + 0.5;
    
    // Background base
    vec4 col = background_color;

    // 61-Key Grid Line dividing columns
    float key_grid = fract(centered_x * 61.0);
    float key_line = smoothstep(0.0, 0.04, key_grid) * smoothstep(1.0, 0.96, key_grid);
    col = mix(grid_line_color, col, key_line);

    // BPM Beat Bar Horizontal Markers
    float beat_line = fract((1.0 - uv.y) * beat_frequency + TIME * (scroll_speed * 0.5));
    float beat_mask = smoothstep(0.0, 0.015, beat_line) * smoothstep(0.03, 0.015, beat_line);
    col = mix(col, beat_marker_color, beat_mask * 0.7);

    // Sample note stream texture
    vec2 stream_uv = vec2(clamp(centered_x, 0.0, 1.0), uv.y);
    vec4 note_data = texture(note_stream_texture, stream_uv);

    float note_velocity = note_data.r;
    float note_channel  = note_data.g; // 0 = Left Hand (Amber), 1 = Right Hand (Cyan)

    // Compute neon glow
    if (note_velocity > 0.02) {
        vec4 note_color = mix(vec4(1.0, 0.72, 0.01, 1.0), vec4(0.0, 0.96, 0.83, 1.0), note_channel);
        
        // Intensity ramp toward leading edge (bottom of note)
        float trail_intensity = note_velocity * 1.8;
        col = mix(col, note_color, clamp(trail_intensity, 0.0, 1.0));
    }

    // Strike Line Flash (Target Bar)
    float strike_dist = abs(uv.y - strike_line_y);
    float strike_beam = smoothstep(bloom_spread, 0.0, strike_dist);
    col += strike_line_color * strike_beam * 0.85;

    // Peripheral vignette
    float vignette = distance(uv, vec2(0.5, 0.5));
    col.rgb *= smoothstep(0.95, 0.35, vignette);

    COLOR = col;
}
