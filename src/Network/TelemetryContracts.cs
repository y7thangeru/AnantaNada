using System.Text.Json.Serialization;

namespace AnantaNada.Network;

/// <summary>
/// Outbound telemetry broadcast from Godot Host to connected Mobile Teleprompters.
/// </summary>
public sealed class PlaybackTickMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "PLAYBACK_TICK";

    [JsonPropertyName("data")]
    public PlaybackTickData Data { get; set; } = new();
}

public sealed class PlaybackTickData
{
    [JsonPropertyName("songId")]
    public string SongId { get; set; } = string.Empty;

    [JsonPropertyName("bar")]
    public int Bar { get; set; }

    [JsonPropertyName("beat")]
    public int Beat { get; set; }

    [JsonPropertyName("bpm")]
    public int Bpm { get; set; } = 120;

    [JsonPropertyName("currentChord")]
    public string CurrentChord { get; set; } = string.Empty;

    [JsonPropertyName("nextChord")]
    public string NextChord { get; set; } = string.Empty;

    [JsonPropertyName("activePatch")]
    public string ActivePatch { get; set; } = string.Empty;
}

/// <summary>
/// Inbound command from Mobile Remote requesting a song/tone change.
/// </summary>
public sealed class TriggerSetlistItemCommand
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = "TRIGGER_SETLIST_ITEM";

    [JsonPropertyName("payload")]
    public TriggerSetlistItemPayload? Payload { get; set; }
}

public sealed class TriggerSetlistItemPayload
{
    [JsonPropertyName("songIndex")]
    public int SongIndex { get; set; }

    [JsonPropertyName("programChange")]
    public byte ProgramChange { get; set; }

    [JsonPropertyName("bankMsb")]
    public byte BankMsb { get; set; }

    [JsonPropertyName("bankLsb")]
    public byte BankLsb { get; set; }

    [JsonPropertyName("transpose")]
    public int Transpose { get; set; }
}

/// <summary>
/// Generic envelope for inbound WebSocket commands.
/// </summary>
public sealed class InboundCommandEnvelope
{
    [JsonPropertyName("command")]
    public string? Command { get; set; }
}

/// <summary>
/// Inbound command from Mobile Remote requesting a Performance Pad action (Ohpe-inspired).
/// </summary>
public sealed class TriggerPerformancePadCommand
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = "TRIGGER_PERFORMANCE_PAD";

    [JsonPropertyName("padType")]
    public string PadType { get; set; } = "Stutter"; // Stutter, TapeStop, PitchRiser, FilterSweep, ReverbFreeze

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("channel")]
    public byte Channel { get; set; } = 0;
}

/// <summary>
/// Inbound command to configure Euclidean & Orbit Polymetric Sequencer.
/// </summary>
public sealed class SetEuclideanConfigCommand
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = "SET_EUCLIDEAN_CONFIG";

    [JsonPropertyName("ringIndex")]
    public int RingIndex { get; set; }

    [JsonPropertyName("steps")]
    public int Steps { get; set; } = 16;

    [JsonPropertyName("pulses")]
    public int Pulses { get; set; } = 4;

    [JsonPropertyName("rotation")]
    public int Rotation { get; set; } = 0;
}

/// <summary>
/// Inbound command to configure Harmony Brain Key Gravity.
/// </summary>
public sealed class SetKeyGravityCommand
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = "SET_KEY_GRAVITY";

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "SnapNearest"; // Off, SnapNearest, HighlightOnly

    [JsonPropertyName("rootNote")]
    public byte RootNote { get; set; } = 0;

    [JsonPropertyName("scale")]
    public string Scale { get; set; } = "Major";
}

/// <summary>
/// Outbound telemetry broadcast when Harmony Brain recognizes chords or active scales.
/// </summary>
public sealed class HarmonyUpdateMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "HARMONY_UPDATE";

    [JsonPropertyName("chordName")]
    public string ChordName { get; set; } = "None";

    [JsonPropertyName("root")]
    public string Root { get; set; } = "";

    [JsonPropertyName("quality")]
    public string Quality { get; set; } = "";

    [JsonPropertyName("bass")]
    public string Bass { get; set; } = "";

    [JsonPropertyName("gravityMode")]
    public string GravityMode { get; set; } = "SnapNearest";
}

