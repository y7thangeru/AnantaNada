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
