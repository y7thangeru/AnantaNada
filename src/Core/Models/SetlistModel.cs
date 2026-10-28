using System.Text.Json;
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

/// <summary>
/// Reference to a Roland Go:Keys 3 Tone preset with MSB/LSB/PC wire dispatch data.
/// Supports deserialization from either a string ID ("PR001") or a full object.
/// </summary>
[JsonConverter(typeof(TonePresetRefConverter))]
public sealed class TonePresetRef
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("msb")]
    public byte Msb { get; set; } = 71;

    [JsonPropertyName("lsb")]
    public byte Lsb { get; set; } = 96;

    /// <summary>
    /// 1-indexed Roland Program Change (1–128).
    /// </summary>
    [JsonPropertyName("pc")]
    public byte Pc { get; set; } = 1;

    [JsonIgnore]
    public byte RawProgramChange => (byte)Math.Max(0, Pc - 1);

    public TonePresetRef() { }

    public TonePresetRef(string name, string category, byte msb, byte lsb, byte pc, string id = "")
    {
        Name = name;
        Category = category;
        Msb = msb;
        Lsb = lsb;
        Pc = pc;
        Id = id;
    }

    public static implicit operator TonePresetRef(string idOrName) => new() { Id = idOrName, Name = idOrName };
    public static implicit operator string(TonePresetRef? toneRef) => toneRef?.Name ?? string.Empty;

    public override string ToString() => $"{Name} ({Category}) [MSB:{Msb} LSB:{Lsb} PC:{Pc}]";
}

/// <summary>
/// Polymorphic converter supporting both string tone IDs and nested tone preset objects.
/// </summary>
public sealed class TonePresetRefConverter : JsonConverter<TonePresetRef>
{
    public override TonePresetRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? idOrName = reader.GetString();
            if (string.IsNullOrEmpty(idOrName)) return null;
            return new TonePresetRef { Id = idOrName, Name = idOrName };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var result = new TonePresetRef();
            if (root.TryGetProperty("id", out var idProp)) result.Id = idProp.GetString() ?? "";
            if (root.TryGetProperty("name", out var nameProp)) result.Name = nameProp.GetString() ?? "";
            if (root.TryGetProperty("category", out var catProp)) result.Category = catProp.GetString() ?? "";
            if (root.TryGetProperty("msb", out var msbProp)) result.Msb = msbProp.GetByte();
            if (root.TryGetProperty("lsb", out var lsbProp)) result.Lsb = lsbProp.GetByte();
            if (root.TryGetProperty("pc", out var pcProp)) result.Pc = pcProp.GetByte();

            return result;
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing TonePresetRef");
    }

    public override void Write(Utf8JsonWriter writer, TonePresetRef value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (!string.IsNullOrEmpty(value.Id)) writer.WriteString("id", value.Id);
        writer.WriteString("name", value.Name);
        writer.WriteString("category", value.Category);
        writer.WriteNumber("msb", value.Msb);
        writer.WriteNumber("lsb", value.Lsb);
        writer.WriteNumber("pc", value.Pc);
        writer.WriteEndObject();
    }
}

public sealed class ChordProgressionInfo
{
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;

    [JsonPropertyName("verse")]
    public string Verse { get; set; } = string.Empty;

    [JsonPropertyName("chorus")]
    public string Chorus { get; set; } = string.Empty;
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
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("songId")]
    public string SongId
    {
        get => string.IsNullOrEmpty(_songId) ? Id : _songId;
        set { _songId = value; if (string.IsNullOrEmpty(Id)) Id = value; }
    }
    private string _songId = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = "Indo_Band";

    [JsonPropertyName("bpm")]
    public int Bpm { get; set; } = 120;

    [JsonPropertyName("keySignature")]
    public string KeySignature { get; set; } = "C Major";

    [JsonPropertyName("transpose")]
    public int Transpose { get; set; } = 0; // Semitone offset (-24 to +24)

    [JsonPropertyName("timeSignature")]
    public string TimeSignature { get; set; } = "4/4";

    /// <summary>
    /// Tone Preset for Right Hand / Zone B
    /// </summary>
    [JsonPropertyName("primaryTone")]
    public TonePresetRef PrimaryTone { get; set; } = new();

    /// <summary>
    /// Tone Preset for Left Hand / Zone A (nullable if full keyboard)
    /// </summary>
    [JsonPropertyName("splitTone")]
    public TonePresetRef? SplitTone { get; set; }

    [JsonPropertyName("splitPoint")]
    public int SplitPoint { get; set; } = 60; // C4

    [JsonPropertyName("chordProgression")]
    public ChordProgressionInfo? ChordProgression { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("chordsTimeline")]
    public List<ChordEvent> ChordsTimeline { get; set; } = new();

    [JsonPropertyName("lyricsTimeline")]
    public List<LyricEvent> LyricsTimeline { get; set; } = new();

    /// <summary>
    /// Generates playback chord timeline from ChordProgression if ChordsTimeline is empty.
    /// </summary>
    public void EnsureTimelineGenerated()
    {
        if (ChordsTimeline.Count > 0 || ChordProgression == null) return;

        var chords = new List<string>();
        if (!string.IsNullOrWhiteSpace(ChordProgression.Intro))
            chords.AddRange(ChordProgression.Intro.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries));
        if (!string.IsNullOrWhiteSpace(ChordProgression.Verse))
            chords.AddRange(ChordProgression.Verse.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries));
        if (!string.IsNullOrWhiteSpace(ChordProgression.Chorus))
            chords.AddRange(ChordProgression.Chorus.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries));

        if (chords.Count == 0) chords.Add("C");

        for (int i = 0; i < chords.Count; i++)
        {
            int bar = (i * 2) + 1;
            string cur = chords[i];
            string nxt = chords[(i + 1) % chords.Count];
            ChordsTimeline.Add(new ChordEvent { Bar = bar, Beat = 1, CurrentChord = cur, NextChord = nxt });
        }

        if (LyricsTimeline.Count == 0)
        {
            LyricsTimeline.Add(new LyricEvent { Bar = 1, Text = $"{Title} - {Artist}" });
            LyricsTimeline.Add(new LyricEvent { Bar = 3, Text = $"Intro: {ChordProgression.Intro}" });
            LyricsTimeline.Add(new LyricEvent { Bar = 5, Text = $"Verse: {ChordProgression.Verse}" });
            LyricsTimeline.Add(new LyricEvent { Bar = 9, Text = $"Chorus: {ChordProgression.Chorus}" });
        }
    }
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
