using System.Text.Json;
using AnantaNada.Core.Data;
using AnantaNada.Core.Models;

namespace AnantaNada.Core.Services;

/// <summary>
/// Service managing the Roland Go:Keys 3 Tone Library Catalog and Stage Repertoire.
/// Provides O(1) ID lookups, category grouping, and multi-tag search filters.
/// </summary>
public sealed class ToneService
{
    private readonly Dictionary<string, ToneItem> _tonesById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<ToneItem>> _tonesByCategory = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ToneItem> _allTones = new();

    private readonly Dictionary<string, SongMission> _songsById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<SongMission>> _songsByCategory = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<SongMission> _allSongs = new();

    public IReadOnlyList<ToneItem> AllTones => _allTones;
    public IEnumerable<string> Categories => _tonesByCategory.Keys;

    public IReadOnlyList<SongMission> AllSongs => _allSongs;
    public IEnumerable<string> SongCategories => _songsByCategory.Keys;

    public ToneService(string? jsonFilePath = null, string? repertoireFilePath = null)
    {
        string path = ResolveCatalogFilePath(jsonFilePath);
        if (File.Exists(path))
        {
            LoadFromJsonFile(path);
        }
        else
        {
            // Fallback: Populate baseline Roland Go:Keys 3 presets in-memory if file not found
            PopulateBaselineFallback();
        }

        // Initialize 100-song repertoire database
        LoadRepertoireFromJson(repertoireFilePath);
    }

    private static string ResolveCatalogFilePath(string? preferredPath)
    {
        if (!string.IsNullOrEmpty(preferredPath) && File.Exists(preferredPath))
        {
            return preferredPath;
        }

        // Search common project locations
        string[] candidates =
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Core", "Data", "gokeys3_tones.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Core", "Data", "gokeys3_tones.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "gokeys3_tones.json")
        };

        foreach (var c in candidates)
        {
            if (File.Exists(c)) return c;
        }

        return candidates[0];
    }

    public void LoadFromJsonFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<ToneItem>>(json);
            if (items != null)
            {
                _tonesById.Clear();
                _tonesByCategory.Clear();
                _allTones.Clear();

                foreach (var tone in items)
                {
                    _tonesById[tone.Id] = tone;
                    _allTones.Add(tone);

                    if (!_tonesByCategory.TryGetValue(tone.Category, out var list))
                    {
                        list = new List<ToneItem>();
                        _tonesByCategory[tone.Category] = list;
                    }
                    list.Add(tone);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ToneService] Warning: Failed to parse {filePath}: {ex.Message}. Falling back to baseline presets.");
            PopulateBaselineFallback();
        }
    }

    public ToneItem? GetToneById(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        _tonesById.TryGetValue(id, out var tone);
        return tone;
    }

    public ToneItem? ResolveTone(TonePresetRef? toneRef)
    {
        if (toneRef == null) return null;
        if (!string.IsNullOrEmpty(toneRef.Id) && _tonesById.TryGetValue(toneRef.Id, out var existing))
        {
            return existing;
        }

        return new ToneItem
        {
            Id = toneRef.Id,
            Name = toneRef.Name,
            Category = toneRef.Category,
            Msb = toneRef.Msb,
            Lsb = toneRef.Lsb,
            Pc = toneRef.Pc,
            DefaultSplitKey = 60
        };
    }

    public IReadOnlyList<ToneItem> GetTonesByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category)) return _allTones;
        if (_tonesByCategory.TryGetValue(category, out var list))
        {
            return list;
        }
        return Array.Empty<ToneItem>();
    }

    public IReadOnlyList<ToneItem> SearchTones(string query, string? category = null)
    {
        var source = string.IsNullOrWhiteSpace(category) ? _allTones : GetTonesByCategory(category);
        if (string.IsNullOrWhiteSpace(query))
        {
            return source;
        }

        string q = query.Trim();
        var results = new List<ToneItem>();

        foreach (var t in source)
        {
            bool matchName = t.Name.Contains(q, StringComparison.OrdinalIgnoreCase);
            bool matchId = t.Id.Contains(q, StringComparison.OrdinalIgnoreCase);
            bool matchTags = t.Tags.Any(tag => tag.Contains(q, StringComparison.OrdinalIgnoreCase));

            if (matchName || matchId || matchTags)
            {
                results.Add(t);
            }
        }

        return results;
    }

    private void PopulateBaselineFallback()
    {
        _tonesById.Clear();
        _tonesByCategory.Clear();
        _allTones.Clear();

        var baseline = new List<ToneItem>
        {
            new() { Id = "PR001", Name = "Concert Piano", Category = "Ac.Piano", Msb = 71, Lsb = 96, Pc = 1, Tags = new() { "Acoustic", "Solo" } },
            new() { Id = "PR022", Name = "70s Suitcase EP", Category = "E.Piano", Msb = 71, Lsb = 96, Pc = 22, Tags = new() { "Electric", "Rhodes", "Vintage" } },
            new() { Id = "CM085_01", Name = "B3 Jazz Organ", Category = "Organ", Msb = 87, Lsb = 85, Pc = 1, Tags = new() { "Organ", "Drawbar" } },
            new() { Id = "PR045", Name = "Concert Strings Sect", Category = "Strings", Msb = 71, Lsb = 96, Pc = 45, Tags = new() { "Strings", "Ensemble" } },
            new() { Id = "PR060", Name = "NSoul Synth Lead", Category = "Synth Lead", Msb = 71, Lsb = 96, Pc = 60, Tags = new() { "Synth", "Lead" } },
            new() { Id = "CM088_01", Name = "Analog Brass Sect 1", Category = "Synth Brass", Msb = 87, Lsb = 88, Pc = 1, Tags = new() { "Brass", "Synth" } },
            new() { Id = "CM089_22", Name = "Sub Heavy Synth Bass", Category = "Bass", Msb = 87, Lsb = 89, Pc = 22, Tags = new() { "Bass", "Sub" } }
        };

        foreach (var tone in baseline)
        {
            _tonesById[tone.Id] = tone;
            _allTones.Add(tone);

            if (!_tonesByCategory.TryGetValue(tone.Category, out var list))
            {
                list = new List<ToneItem>();
                _tonesByCategory[tone.Category] = list;
            }
            list.Add(tone);
        }
    }

    public void LoadRepertoireFromJson(string? preferredPath = null)
    {
        string path = ResolveRepertoireFilePath(preferredPath);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                List<SongMission>? songs = null;
                if (json.TrimStart().StartsWith("["))
                {
                    songs = JsonSerializer.Deserialize<List<SongMission>>(json);
                }
                else
                {
                    var doc = JsonSerializer.Deserialize<SetlistDocument>(json);
                    songs = doc?.Songs;
                }

                if (songs != null && songs.Count > 0)
                {
                    PopulateRepertoire(songs);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ToneService] Warning: Failed to parse repertoire from {path}: {ex.Message}. Using in-memory builder.");
            }
        }

        // If file not found or failed to parse, build 100 repertoire in-memory and write to disk
        var fallbackSongs = RepertoireDatabaseBuilder.Build100Repertoire();
        PopulateRepertoire(fallbackSongs);
        try
        {
            RepertoireDatabaseBuilder.ExportMasterRepertoireJson(path);
        }
        catch
        {
            // Ignore write errors in restricted environments
        }
    }

    private void PopulateRepertoire(IEnumerable<SongMission> songs)
    {
        _songsById.Clear();
        _songsByCategory.Clear();
        _allSongs.Clear();

        foreach (var song in songs)
        {
            song.EnsureTimelineGenerated();
            _songsById[song.Id] = song;
            if (!string.IsNullOrEmpty(song.SongId) && !song.SongId.Equals(song.Id, StringComparison.OrdinalIgnoreCase))
            {
                _songsById[song.SongId] = song;
            }
            _allSongs.Add(song);

            if (!_songsByCategory.TryGetValue(song.Category, out var list))
            {
                list = new List<SongMission>();
                _songsByCategory[song.Category] = list;
            }
            list.Add(song);
        }
    }

    private static string ResolveRepertoireFilePath(string? preferredPath)
    {
        if (!string.IsNullOrEmpty(preferredPath) && File.Exists(preferredPath))
        {
            return preferredPath;
        }

        string[] candidates =
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setlists", "master_100_repertoire.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "setlists", "master_100_repertoire.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "master_100_repertoire.json")
        };

        foreach (var c in candidates)
        {
            if (File.Exists(c)) return c;
        }

        return candidates[1];
    }

    public SongMission? GetSongById(string songId)
    {
        if (string.IsNullOrWhiteSpace(songId)) return null;
        _songsById.TryGetValue(songId, out var song);
        return song;
    }

    public IReadOnlyList<SongMission> GetSongsByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category) || category.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            return _allSongs;
        }

        if (_songsByCategory.TryGetValue(category, out var list))
        {
            return list;
        }

        return _allSongs.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public IReadOnlyList<SongMission> SearchSongs(string keyword, string? category = null)
    {
        var source = string.IsNullOrWhiteSpace(category) || category.Equals("ALL", StringComparison.OrdinalIgnoreCase)
            ? _allSongs
            : GetSongsByCategory(category);

        if (string.IsNullOrWhiteSpace(keyword))
        {
            return source;
        }

        string q = keyword.Trim();
        var results = new List<SongMission>();

        foreach (var s in source)
        {
            bool matchTitle = s.Title.Contains(q, StringComparison.OrdinalIgnoreCase);
            bool matchArtist = s.Artist.Contains(q, StringComparison.OrdinalIgnoreCase);
            bool matchId = s.Id.Contains(q, StringComparison.OrdinalIgnoreCase) || s.SongId.Contains(q, StringComparison.OrdinalIgnoreCase);
            bool matchTags = s.Tags.Any(tag => tag.Contains(q, StringComparison.OrdinalIgnoreCase));

            if (matchTitle || matchArtist || matchId || matchTags)
            {
                results.Add(s);
            }
        }

        return results;
    }
}
