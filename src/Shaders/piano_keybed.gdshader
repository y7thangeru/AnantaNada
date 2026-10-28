shader_type canvas_item;
render_mode blend_mix;

// --- ANANTANADA SYNTH-RUNNER 61-KEY DIEGETIC KEYBED SHADER ---
// Atlus Persona 5 kinetic slant + Arknights tactical borders + Hi-Fi Rush beat pulse.

uniform vec4 color_white_key : source_color = vec4(0.88, 0.90, 0.94, 1.0);
uniform vec4 color_black_key : source_color = vec4(0.06, 0.07, 0.09, 1.0);
uniform vec4 color_neon_cyan : source_color = vec4(0.0, 0.96, 0.83, 1.0);    // Cyber Cyan (#00f5d4) - Zone B Lead
uniform vec4 color_neon_amber : source_color = vec4(1.0, 0.72, 0.01, 1.0);   // Electric Amber (#ffb703) - Zone A Bass
uniform vec4 color_border : source_color = vec4(0.04, 0.05, 0.06, 1.0);
uniform vec4 color_split_divider : source_color = vec4(1.0, 0.2, 0.4, 0.8);  // Split Marker

// Key state texture: 64x1 8-bit texture where R=velocity (0..1), G=hand/channel (0=Left/Amber, 1=Right/Cyan), B=hit_impact
uniform sampler2D key_states : filter_nearest;

uniform float black_key_height_ratio : hint_range(0.5, 0.75) = 0.62;
uniform float black_key_width_ratio : hint_range(0.4, 0.7) = 0.58;
uniform float border_thickness : hint_range(0.001, 0.02) = 0.006;
uniform float glow_intensity : hint_range(0.5, 5.0) = 2.8;
uniform float chromatic_offset : hint_range(0.0, 0.02) = 0.005;
uniform float beat_pulse : hint_range(0.0, 1.0) = 0.0;
uniform float split_point_norm : hint_range(0.0, 1.0) = 0.393; // Note 60 (C4) split point across 61 keys

const float TOTAL_WHITE_KEYS = 36.0;

bool has_black_key_right(int white_in_octave) {
    return (white_in_octave == 0 || white_in_octave == 1 || white_in_octave == 3 || white_in_octave == 4 || white_in_octave == 5);
}

int get_white_key_midi(int white_index) {
    int octave = white_index / 7;
    int note_in_oct = white_index % 7;
    const int white_offsets[7] = int[](0, 2, 4, 5, 7, 9, 11);
    return 36 + (octave * 12) + white_offsets[note_in_oct];
}

int get_black_key_midi(int white_index) {
    return get_white_key_midi(white_index) + 1;
}

void fragment() {
    vec2 uv = UV;

    // Slanted kinetic stage tilt effect (Persona 5 8° tilt aesthetic)
    float slant = (uv.y - 0.5) * 0.035;
    float tilted_x = clamp(uv.x + slant, 0.0, 1.0);

    float white_pos = tilted_x * TOTAL_WHITE_KEYS;
    int white_idx = int(floor(white_pos));
    float white_frac = fract(white_pos);

    bool is_black_key = false;
    int current_midi_note = get_white_key_midi(white_idx);
    float key_center_dist = abs(white_frac - 0.5);

    // Check if UV falls inside a Black Key overlay
    if (uv.y < black_key_height_ratio) {
        if (white_idx > 0 && white_frac < (black_key_width_ratio * 0.5)) {
            int prev_white_in_oct = (white_idx - 1) % 7;
            if (has_black_key_right(prev_white_in_oct)) {
                is_black_key = true;
                current_midi_note = get_black_key_midi(white_idx - 1);
                key_center_dist = abs(white_frac + 0.5 * black_key_width_ratio);
            }
        }
        else if (white_frac > (1.0 - black_key_width_ratio * 0.5) && white_idx < int(TOTAL_WHITE_KEYS) - 1) {
            int white_in_oct = white_idx % 7;
            if (has_black_key_right(white_in_oct)) {
                is_black_key = true;
                current_midi_note = get_black_key_midi(white_idx);
                key_center_dist = abs(white_frac - 1.0);
            }
        }
    }

    // Sample dynamic key state
    float sample_u = clamp(float(current_midi_note - 36) / 60.0, 0.0, 1.0);
    vec4 state = texture(key_states, vec2(sample_u, 0.5));
    float velocity = state.r;      // 0.0 = released, >0.0 = pressed velocity
    float hand_split = state.g;    // 0.0 = Zone A Amber, 1.0 = Zone B Cyan
    float impact = state.b;        // Instant attack transient flash

    // Base key body color
    vec4 base_color = is_black_key ? color_black_key : color_white_key;

    // Border Frame
    float border_dist = is_black_key ? 
        min(white_frac, min(1.0 - white_frac, uv.y)) : 
        min(white_frac, 1.0 - white_frac);
    float border_factor = smoothstep(0.0, border_thickness * TOTAL_WHITE_KEYS, border_dist);

    // Active Neon Color based on Split Zone
    vec4 active_neon = mix(color_neon_amber, color_neon_cyan, hand_split);

    // Reactive glow & Hi-Fi Rush comic strike flash
    if (velocity > 0.01) {
        // High velocity (>= 0.78 = MIDI ~100) triggers overdrive comic burst
        bool is_overdrive = velocity >= 0.78;
        float burst_mult = is_overdrive ? 1.6 : 1.0;

        vec4 glow = active_neon * (velocity * glow_intensity * burst_mult + impact * 2.2);

        // Chromatic fringe
        float fringe = smoothstep(0.35, 0.5, key_center_dist) * chromatic_offset * 120.0;
        base_color = mix(base_color, glow, (is_black_key ? 0.90 : 0.72) * velocity + fringe);

        // Strike impact flash at head of the key
        float strike_flash = (1.0 - smoothstep(0.0, 0.35, uv.y)) * impact * 1.4;
        base_color += (is_overdrive ? vec4(1.0, 1.0, 1.0, 1.0) : active_neon) * strike_flash;
    } else {
        // Idle bevel with subtle BPM metronome pulse
        float bevel = is_black_key ? 
            smoothstep(0.0, black_key_height_ratio, uv.y) * 0.15 : 
            smoothstep(0.0, 1.0, uv.y) * 0.12;
        base_color.rgb -= vec3(bevel);
        base_color.rgb += (active_neon.rgb * beat_pulse * 0.04);
    }

    // Split point divider line between Zone A (Bass) and Zone B (Lead)
    float split_dist = abs(tilted_x - split_point_norm);
    if (split_dist < 0.003) {
        base_color = mix(base_color, color_split_divider, 0.9);
    }

    vec4 final_color = mix(color_border, base_color, border_factor);

    // Persona 5 scanline texture
    float scanline = sin(uv.y * 420.0) * 0.025;
    final_color.rgb += vec3(scanline);

    COLOR = final_color;
}
