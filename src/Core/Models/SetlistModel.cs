using System.Text.Json.Serialization;

namespace AnantaNada.Core.Models;

/// <summary>
/// Roland Go:Keys 3 Tone Catalog Entry.
/// </summary>
public sealed class ToneItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("msb")]
    public byte Msb { get; set; }

    [JsonPropertyName("lsb")]
    public byte Lsb { get; set; }

    /// <summary>
    /// 1-indexed Roland Tone PC (1–128). Normalizes to (pc - 1) on MIDI wire.
    /// </summary>
    [JsonPropertyName("pc")]
    public byte Pc { get; set; }

    [JsonPropertyName("defaultSplitKey")]
    public int DefaultSplitKey { get; set; } = 60; // C4

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Returns 0-indexed Program Change byte for raw MIDI wire transmission.
    /// </summary>
    [JsonIgnore]
    public byte RawProgramChange => (byte)Math.Max(0, Pc - 1);

    public override string ToString() =>
        $"[{Id}] {Name} ({Category}) MSB:{Msb} LSB:{Lsb} PC:{Pc}";
}

public sealed class ChordEvent
{
    [JsonPropertyName("bar")]
    public int Bar { get; set; }

    [JsonPropertyName("beat")]
    public int Beat { get; set; } = 1;

    [JsonPropertyName("currentChord")]
    public string CurrentChord { get; set; } = string.Empty;

    [JsonPropertyName("nextChord")]
    public string NextChord { get; set; } = string.Empty;
}

public sealed class LyricEvent
{
    [JsonPropertyName("bar")]
    public int Bar { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Single Song Mission in the Live Setlist Roster.
/// </summary>
public sealed class SongMission
{
    [JsonPropertyName("songId")]
    public string SongId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("bpm")]
    public int Bpm { get; set; } = 120;

    [JsonPropertyName("keySignature")]
    public string KeySignature { get; set; } = "C";

    [JsonPropertyName("transpose")]
    public int Transpose { get; set; } = 0; // Semitone offset (-24 to +24)

    /// <summary>
    /// Tone ID for Right Hand / Zone B (e.g. "PR022")
    /// </summary>
    [JsonPropertyName("primaryTone")]
    public string PrimaryTone { get; set; } = "PR001";

    /// <summary>
    /// Tone ID for Left Hand / Zone A (e.g. "CM089_22"), nullable if full keyboard
    /// </summary>
    [JsonPropertyName("splitTone")]
    public string? SplitTone { get; set; }

    [JsonPropertyName("splitPoint")]
    public int SplitPoint { get; set; } = 60; // C4

    [JsonPropertyName("chordsTimeline")]
    public List<ChordEvent> ChordsTimeline { get; set; } = new();

    [JsonPropertyName("lyricsTimeline")]
    public List<LyricEvent> LyricsTimeline { get; set; } = new();
}

public sealed class SetlistDocument
{
    [JsonPropertyName("setlistName")]
    public string SetlistName { get; set; } = "AnantaNada Live Setlist";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("songs")]
    public List<SongMission> Songs { get; set; } = new();
}
