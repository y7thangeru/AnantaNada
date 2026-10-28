using System.Text.Json;
using AnantaNada.Core.Models;

namespace AnantaNada.Core.Data;

/// <summary>
/// Static builder generating the comprehensive 100-Song Tactical Stage Repertoire
/// for high school and university band live performances on Roland Go:Keys 3.
/// </summary>
public static class RepertoireDatabaseBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static List<SongMission> Build100Repertoire()
    {
        var list = new List<SongMission>(100);

        // =========================================================================
        // SECTION A: 50 INDONESIAN POP / ROCK SCHOOL BAND ANTHEMS (1-50)
        // =========================================================================

        // 1. Dan - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_001",
            Title = "Dan",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 100,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - Em - C - D", Verse = "G - Em - C - D", Chorus = "C - D - G - Em - C - D - G" },
            Tags = new List<string> { "SMP/SMA Favorite", "Classic 90s", "Sing-Along" }
        });

        // 2. Kisah Klasik Untuk Masa Depan - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_002",
            Title = "Kisah Klasik Untuk Masa Depan",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 114,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - F#m - D - E", Verse = "A - C#m - D - E", Chorus = "D - E - C#m - F#m - Bm - E - A" },
            Tags = new List<string> { "SMP/SMA Favorite", "Graduation Anthem", "Nostalgic" }
        });

        // 3. Melompat Lebih Tinggi - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_003",
            Title = "Melompat Lebih Tinggi",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 136,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Windy Synth Lead", "Synth Lead", 71, 96, 61),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - G - Bm - A", Verse = "D - G - Bm - A", Chorus = "G - A - F#m - Bm - Em - A - D" },
            Tags = new List<string> { "Energetic", "Festival Standard", "High Energy" }
        });

        // 4. Sahabat Sejati - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_004",
            Title = "Sahabat Sejati",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 120,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pop Piano", "Ac.Piano", 71, 96, 4),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - G - Am - F", Verse = "C - G - Am - F", Chorus = "C - G - Am - Em - F - C - Dm - G" },
            Tags = new List<string> { "SMP/SMA Favorite", "Friendship", "Pop Rock" }
        });

        // 5. Sephia - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_005",
            Title = "Sephia",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 88,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Mellow Grand", "Ac.Piano", 71, 96, 2),
            SplitTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - Bm - C - G", Verse = "Em - Bm - C - G", Chorus = "C - D - G - Em - C - D - Em" },
            Tags = new List<string> { "Ballad", "Emotional", "Classic" }
        });

        // 6. Berharap Tak Berpisah - Reza Artamevia
        list.Add(new SongMission
        {
            Id = "SONG_006",
            Title = "Berharap Tak Berpisah",
            Artist = "Reza Artamevia",
            Category = "Indo_Band",
            Bpm = 116,
            KeySignature = "G Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Disco Clav", "E.Piano", 71, 96, 26),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Gm - Cm7 - F - Bbmaj7", Verse = "Gm7 - Cm7 - F - Bbmaj7", Chorus = "Cm7 - F - Bbmaj7 - Ebmaj7 - Am7b5 - D7 - Gm" },
            Tags = new List<string> { "Disco Funk", "Festival Favorite", "Dance" }
        });

        // 7. Kangen - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_007",
            Title = "Kangen",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 74,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - Bm - G - A", Verse = "D - Bm - G - A", Chorus = "G - F#m - Em - A - D" },
            Tags = new List<string> { "Classic Rock", "Legendary", "Slow Rock" }
        });

        // 8. Pupus - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_008",
            Title = "Pupus",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 78,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - D/F# - Em - C", Verse = "G - D/F# - Em - C", Chorus = "G - D - Em - C - Cm" },
            Tags = new List<string> { "Anthem", "Sing-Along", "Epic Ballad" }
        });

        // 9. Separuh Nafas - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_009",
            Title = "Separuh Nafas",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 122,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Square Lead Synth", "Synth Lead", 71, 96, 63),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm - Bb - F - C", Verse = "Dm - Bb - F - C", Chorus = "Dm - Bb - F - C" },
            Tags = new List<string> { "Festival Standard", "Pop Rock", "Crowd Pleaser" }
        });

        // 10. Kamulah Satu-Satunya - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_010",
            Title = "Kamulah Satu-Satunya",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 126,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitTone = new TonePresetRef("Rock Drawbar Organ", "Organ", 71, 96, 31),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - C - G - C", Verse = "G - Bm - C - D", Chorus = "G - Em - C - D" },
            Tags = new List<string> { "Energetic", "Brass Stabs", "Classic 90s" }
        });

        // 11. Sempurna - Andra and the Backbone
        list.Add(new SongMission
        {
            Id = "SONG_011",
            Title = "Sempurna",
            Artist = "Andra and the Backbone",
            Category = "Indo_Band",
            Bpm = 88,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - C#m - A - B", Verse = "E - C#m - A - B", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "Acoustic Pop", "Sweet Ballad", "High School Standard" }
        });

        // 12. Main Hati - Andra and the Backbone
        list.Add(new SongMission
        {
            Id = "SONG_012",
            Title = "Main Hati",
            Artist = "Andra and the Backbone",
            Category = "Indo_Band",
            Bpm = 120,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - C#m - D - E", Verse = "A - C#m - D - E", Chorus = "D - E - C#m - F#m - Bm - E - A" },
            Tags = new List<string> { "Rock Organ", "Upbeat", "Band Festival" }
        });

        // 13. Laskar Pelangi - Nidji
        list.Add(new SongMission
        {
            Id = "SONG_013",
            Title = "Laskar Pelangi",
            Artist = "Nidji",
            Category = "Indo_Band",
            Bpm = 92,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Shimmer Pad", "Pad", 87, 85, 38),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - D - A - D", Verse = "A - C#m - D - E", Chorus = "A - C#m - D - E - F#m - E - D" },
            Tags = new List<string> { "Inspirational", "Soundtrack", "School Anthem" }
        });

        // 14. Hapus Aku - Nidji
        list.Add(new SongMission
        {
            Id = "SONG_014",
            Title = "Hapus Aku",
            Artist = "Nidji",
            Category = "Indo_Band",
            Bpm = 138,
            KeySignature = "B Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bm - G - D - A", Verse = "Bm - G - D - A", Chorus = "G - A - Bm - F#m - G - A - D" },
            Tags = new List<string> { "Modern Rock", "Synth Hook", "Energetic" }
        });

        // 15. Disco Darurat - The Adams
        list.Add(new SongMission
        {
            Id = "SONG_015",
            Title = "Disco Darurat",
            Artist = "The Adams",
            Category = "Indo_Band",
            Bpm = 134,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s PolySynth", "Synth Lead", 71, 96, 65),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - Bm - C - D", Verse = "G - Bm - C - D", Chorus = "C - D - Bm - Em - Am - D - G" },
            Tags = new List<string> { "Indie Rock", "Power Pop", "Dance Rock" }
        });

        // 16. Konservatif - The Adams
        list.Add(new SongMission
        {
            Id = "SONG_016",
            Title = "Konservatif",
            Artist = "The Adams",
            Category = "Indo_Band",
            Bpm = 118,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Wurly 200A", "E.Piano", 71, 96, 24),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - F#m - G - A", Verse = "D - F#m - G - A", Chorus = "G - A - F#m - Bm - Em - A - D" },
            Tags = new List<string> { "Indie Darling", "Harmonies", "Youth Anthem" }
        });

        // 17. C.H.R.I.S.Y.E. - Diskoria, Laleilmanino, Eva Celia
        list.Add(new SongMission
        {
            Id = "SONG_017",
            Title = "C.H.R.I.S.Y.E.",
            Artist = "Diskoria, Laleilmanino, Eva Celia",
            Category = "Indo_Band",
            Bpm = 118,
            KeySignature = "C# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Juno Synth Brass", "Synth Brass", 87, 88, 14),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - B - G#m - C#m", Verse = "A - B - G#m - C#m", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "City Pop", "Disco Groove", "Modern Classic" }
        });

        // 18. Serenata Jiwa Disko - Diskoria feat. Dian Sastrowardoyo
        list.Add(new SongMission
        {
            Id = "SONG_018",
            Title = "Serenata Jiwa Disko",
            Artist = "Diskoria feat. Dian Sastrowardoyo",
            Category = "Indo_Band",
            Bpm = 120,
            KeySignature = "F# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Disco Funk Clav", "E.Piano", 71, 96, 26),
            SplitTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F#m7 - Bm7 - E - Amaj7", Verse = "F#m7 - Bm7 - E - Amaj7", Chorus = "Dmaj7 - C#m7 - Bm7 - C#7" },
            Tags = new List<string> { "Retro Disco", "City Pop", "Dance Floor" }
        });

        // 19. Runtuh - Feby Putri feat. Fiersa Besari
        list.Add(new SongMission
        {
            Id = "SONG_019",
            Title = "Runtuh",
            Artist = "Feby Putri feat. Fiersa Besari",
            Category = "Indo_Band",
            Bpm = 74,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Felt Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - F - C - F", Verse = "C - Em - F - G", Chorus = "F - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Acoustic Ballad", "Intimate", "Viral Hit" }
        });

        // 20. Komang - Raim Laode
        list.Add(new SongMission
        {
            Id = "SONG_020",
            Title = "Komang",
            Artist = "Raim Laode",
            Category = "Indo_Band",
            Bpm = 80,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - D - C - D", Verse = "G - D - C - D", Chorus = "C - D - Bm - Em - Am - D - G" },
            Tags = new List<string> { "Romantic", "Acoustic", "Sing-Along" }
        });

        // 21. Akad - Payung Teduh
        list.Add(new SongMission
        {
            Id = "SONG_021",
            Title = "Akad",
            Artist = "Payung Teduh",
            Category = "Indo_Band",
            Bpm = 102,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Acoustic Upright Bass", "Bass", 71, 96, 32),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - G#m - A - B", Verse = "E - G#m - A - B", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "Jazz Pop", "Horn Stabs", "Festival Standard" }
        });

        // 22. Menghujam Jantungku - Tompi
        list.Add(new SongMission
        {
            Id = "SONG_022",
            Title = "Menghujam Jantungku",
            Artist = "Tompi",
            Category = "Indo_Band",
            Bpm = 100,
            KeySignature = "Bb Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Jazz B3 Organ", "Organ", 71, 96, 30),
            SplitTone = new TonePresetRef("Finger Electric Bass", "Bass", 71, 96, 33),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bb - Dm - Eb - F", Verse = "Bb - Dm - Eb - F", Chorus = "Eb - F - Dm - Gm - Cm - F - Bb" },
            Tags = new List<string> { "Jazz Funk", "Organ Solo", "Groovy" }
        });

        // 23. Dekat di Hati - RAN
        list.Add(new SongMission
        {
            Id = "SONG_023",
            Title = "Dekat di Hati",
            Artist = "RAN",
            Category = "Indo_Band",
            Bpm = 95,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Phase Rhodes", "E.Piano", 71, 96, 23),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Cmaj7 - D - Bm7 - Em7", Verse = "Cmaj7 - D - Bm7 - Em7", Chorus = "Cmaj7 - D - Bm7 - Em7 - Am7 - D - G" },
            Tags = new List<string> { "Urban Pop", "Sweet Harmonies", "LDR Anthem" }
        });

        // 24. Pandangan Pertama - RAN
        list.Add(new SongMission
        {
            Id = "SONG_024",
            Title = "Pandangan Pertama",
            Artist = "RAN",
            Category = "Indo_Band",
            Bpm = 105,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Clavinet Funk", "E.Piano", 71, 96, 26),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E7 - A7", Verse = "E7 - A7 - E7 - B7", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "Funk", "Clavinet Grooves", "Upbeat" }
        });

        // 25. Selamat Pagi - RAN
        list.Add(new SongMission
        {
            Id = "SONG_025",
            Title = "Selamat Pagi",
            Artist = "RAN",
            Category = "Indo_Band",
            Bpm = 110,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - F#m - G - A", Verse = "D - F#m - G - A", Chorus = "G - A - F#m - Bm - Em - A - D" },
            Tags = new List<string> { "Morning Jam", "Bright Pop", "High School Vibe" }
        });

        // 26. Kenangan Manis - Pamungkas
        list.Add(new SongMission
        {
            Id = "SONG_026",
            Title = "Kenangan Manis",
            Artist = "Pamungkas",
            Category = "Indo_Band",
            Bpm = 80,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Mellow Grand", "Ac.Piano", 71, 96, 2),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - C - G - C", Verse = "G - Bm - C - D", Chorus = "C - D - Bm - Em - Am - D - G" },
            Tags = new List<string> { "Indie Pop", "Sing-Along", "Nostalgic" }
        });

        // 27. To the Bone - Pamungkas
        list.Add(new SongMission
        {
            Id = "SONG_027",
            Title = "To the Bone",
            Artist = "Pamungkas",
            Category = "Indo_Band",
            Bpm = 75,
            KeySignature = "F# Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Felt Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F# - B - F# - B", Verse = "F# - B - F# - B", Chorus = "B - C# - A#m - D#m - G#m - C# - F#" },
            Tags = new List<string> { "Viral Global", "Modern Ballad", "Crowd Favorite" }
        });

        // 28. Evaluasi - Hindia
        list.Add(new SongMission
        {
            Id = "SONG_028",
            Title = "Evaluasi",
            Artist = "Hindia",
            Category = "Indo_Band",
            Bpm = 115,
            KeySignature = "G Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Dark Lo-Fi Synth", "Synth Lead", 71, 96, 66),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Gm - Eb - Bb - F", Verse = "Gm - Eb - Bb - F", Chorus = "Eb - F - Gm - Bb" },
            Tags = new List<string> { "Indie Wave", "Gen-Z Anthem", "Moody Pop" }
        });

        // 29. Rumah ke Rumah - Hindia
        list.Add(new SongMission
        {
            Id = "SONG_029",
            Title = "Rumah ke Rumah",
            Artist = "Hindia",
            Category = "Indo_Band",
            Bpm = 118,
            KeySignature = "F# Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s PolySynth", "Synth Lead", 71, 96, 65),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F# - B - D#m - C#", Verse = "F# - B - D#m - C#", Chorus = "B - C# - A#m - D#m - G#m - C# - F#" },
            Tags = new List<string> { "Indie Pop", "Emotional", "Festival Hit" }
        });

        // 30. Secukupnya - Hindia
        list.Add(new SongMission
        {
            Id = "SONG_030",
            Title = "Secukupnya",
            Artist = "Hindia",
            Category = "Indo_Band",
            Bpm = 130,
            KeySignature = "C# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C#m - A - E - B", Verse = "C#m - A - E - B", Chorus = "A - B - C#m - E" },
            Tags = new List<string> { "Cyber Synth", "Ost NKCTHI", "Fast Paced" }
        });

        // 31. Berita Kehilangan - .Feast
        list.Add(new SongMission
        {
            Id = "SONG_031",
            Title = "Berita Kehilangan",
            Artist = ".Feast",
            Category = "Indo_Band",
            Bpm = 140,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Aggressive Dist Synth", "Synth Lead", 71, 96, 67),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm - C - Bb - A", Verse = "Dm - C - Bb - A", Chorus = "Bb - C - Dm - F" },
            Tags = new List<string> { "Alternative Rock", "Aggressive", "Heavy Stabs" }
        });

        // 32. Peradaban - .Feast
        list.Add(new SongMission
        {
            Id = "SONG_032",
            Title = "Peradaban",
            Artist = ".Feast",
            Category = "Indo_Band",
            Bpm = 145,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - G - A - C", Verse = "Em - G - A - C", Chorus = "C - D - Em - G" },
            Tags = new List<string> { "Industrial Rock", "Riot Vibe", "Festival Headliner" }
        });

        // 33. Pelangi di Matamu - Jamrud
        list.Add(new SongMission
        {
            Id = "SONG_033",
            Title = "Pelangi di Matamu",
            Artist = "Jamrud",
            Category = "Indo_Band",
            Bpm = 72,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - Em - F - G", Verse = "C - Em - F - G", Chorus = "F - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Rock Ballad", "Classic 2000s", "All-Time Favorite" }
        });

        // 34. Terpesona - Glenn Fredly
        list.Add(new SongMission
        {
            Id = "SONG_034",
            Title = "Terpesona",
            Artist = "Glenn Fredly",
            Category = "Indo_Band",
            Bpm = 100,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Finger Electric Bass", "Bass", 71, 96, 33),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Emaj7 - C#m7 - F#m7 - B7", Verse = "Emaj7 - C#m7 - F#m7 - B7", Chorus = "Amaj7 - G#m7 - F#m7 - B7 - E" },
            Tags = new List<string> { "Soul", "R&B Pop", "Romantic" }
        });

        // 35. Kisah Romantis - Glenn Fredly
        list.Add(new SongMission
        {
            Id = "SONG_035",
            Title = "Kisah Romantis",
            Artist = "Glenn Fredly",
            Category = "Indo_Band",
            Bpm = 98,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Phase Rhodes", "E.Piano", 71, 96, 23),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - C#m - D - E", Verse = "A - C#m - D - E", Chorus = "D - E - C#m - F#m - Bm - E - A" },
            Tags = new List<string> { "Wedding Anthem", "Soulful", "Catchy" }
        });

        // 36. Cinta Luar Biasa - Andmesh
        list.Add(new SongMission
        {
            Id = "SONG_036",
            Title = "Cinta Luar Biasa",
            Artist = "Andmesh",
            Category = "Indo_Band",
            Bpm = 70,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - Em - C - D", Verse = "G - Em - C - D", Chorus = "C - D - Bm - Em - Am - D - G" },
            Tags = new List<string> { "Pop Ballad", "Viral Solo", "Heartfelt" }
        });

        // 37. Hampa - Ari Lasso
        list.Add(new SongMission
        {
            Id = "SONG_037",
            Title = "Hampa",
            Artist = "Ari Lasso",
            Category = "Indo_Band",
            Bpm = 72,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - G#m - A - B", Verse = "E - G#m - A - B", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "Classic Ballad", "Vocal Showcase", "Nostalgic" }
        });

        // 38. Mengejar Matahari - Ari Lasso
        list.Add(new SongMission
        {
            Id = "SONG_038",
            Title = "Mengejar Matahari",
            Artist = "Ari Lasso",
            Category = "Indo_Band",
            Bpm = 82,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - A - Bm - G", Verse = "D - A - Bm - G", Chorus = "D - A - Bm - G - Em - A - D" },
            Tags = new List<string> { "Anthem", "Movie Soundtrack", "Uplifting" }
        });

        // 39. Zona Nyaman - Fourtwnty
        list.Add(new SongMission
        {
            Id = "SONG_039",
            Title = "Zona Nyaman",
            Artist = "Fourtwnty",
            Category = "Indo_Band",
            Bpm = 114,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Acoustic Upright Bass", "Bass", 71, 96, 32),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - A - E - A", Verse = "E - A - E - A", Chorus = "A - B - G#m - C#m - F#m - B - E" },
            Tags = new List<string> { "Indie Folk", "Chill", "Filosofi Kopi" }
        });

        // 40. Fana Merah Jambu - Fourtwnty
        list.Add(new SongMission
        {
            Id = "SONG_040",
            Title = "Fana Merah Jambu",
            Artist = "Fourtwnty",
            Category = "Indo_Band",
            Bpm = 110,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Wurly 200A", "E.Piano", 71, 96, 24),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - F - C - F", Verse = "C - F - C - F", Chorus = "Dm - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Acoustic Pop", "Breezy", "Campfire" }
        });

        // 41. Cemburu - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_041",
            Title = "Cemburu",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 130,
            KeySignature = "G Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Gm - F - Eb - D", Verse = "Gm - F - Eb - D", Chorus = "Cm - F - Bb - Eb - D7 - Gm" },
            Tags = new List<string> { "Hard Pop", "Rock Organ", "Crowd Chant" }
        });

        // 42. Pangeran Cinta - Dewa 19
        list.Add(new SongMission
        {
            Id = "SONG_042",
            Title = "Pangeran Cinta",
            Artist = "Dewa 19",
            Category = "Indo_Band",
            Bpm = 142,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Analog Synth Brass", "Synth Brass", 87, 88, 12),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - C - G - D", Verse = "Em - C - G - D", Chorus = "C - D - Em - G - Am - B7 - Em" },
            Tags = new List<string> { "Epic Rock", "Heavy Brass", "Fast Rock" }
        });

        // 43. J.A.P (Jadikan Aku Pacarmu) - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_043",
            Title = "J.A.P (Jadikan Aku Pacarmu)",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 128,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pop Piano", "Ac.Piano", 71, 96, 4),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - E - F#m - D", Verse = "A - E - F#m - D", Chorus = "A - E - F#m - D - Bm - E - A" },
            Tags = new List<string> { "Pop Punk", "High Energy", "School Festival" }
        });

        // 44. Kita - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_044",
            Title = "Kita",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 105,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - Em - F - G", Verse = "C - Em - F - G", Chorus = "F - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Debut Classic", "Sing-Along", "Nostalgic" }
        });

        // 45. Radio - Sheila on 7
        list.Add(new SongMission
        {
            Id = "SONG_045",
            Title = "Radio",
            Artist = "Sheila on 7",
            Category = "Indo_Band",
            Bpm = 120,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Wurly 200A", "E.Piano", 71, 96, 24),
            SplitTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - Bm - C - D", Verse = "G - Bm - C - D", Chorus = "C - D - Bm - Em - Am - D - G" },
            Tags = new List<string> { "Pop Rock", "Catchy Melody", "Fun" }
        });

        // 46. Bento - Iwan Fals
        list.Add(new SongMission
        {
            Id = "SONG_046",
            Title = "Bento",
            Artist = "Iwan Fals",
            Category = "Indo_Band",
            Bpm = 125,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Rock Drawbar Organ", "Organ", 71, 96, 31),
            SplitTone = new TonePresetRef("Finger Electric Bass", "Bass", 71, 96, 33),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - A - B - E", Verse = "E - A - B - E", Chorus = "A - B - E - C#m - A - B - E" },
            Tags = new List<string> { "Folk Rock", "Legendary Chant", "Stage Explosion" }
        });

        // 47. Bongkar - Swami / Iwan Fals
        list.Add(new SongMission
        {
            Id = "SONG_047",
            Title = "Bongkar",
            Artist = "Swami / Iwan Fals",
            Category = "Indo_Band",
            Bpm = 110,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Percussive Organ", "Organ", 71, 96, 32),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - C - D - Em", Verse = "Em - C - D - Em", Chorus = "C - D - Em - C - D - Em" },
            Tags = new List<string> { "Anthem", "Crowd Roar", "Classic" }
        });

        // 48. Kemesraan - Iwan Fals
        list.Add(new SongMission
        {
            Id = "SONG_048",
            Title = "Kemesraan",
            Artist = "Iwan Fals",
            Category = "Indo_Band",
            Bpm = 74,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - F - G - C", Verse = "C - F - G - C", Chorus = "F - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Closing Anthem", "Sing-Along", "Timeless" }
        });

        // 49. Rumit - Langit Sore
        list.Add(new SongMission
        {
            Id = "SONG_049",
            Title = "Rumit",
            Artist = "Langit Sore",
            Category = "Indo_Band",
            Bpm = 78,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Felt Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - G - D - G", Verse = "D - G - D - G", Chorus = "G - A - F#m - Bm - Em - A - D" },
            Tags = new List<string> { "Acoustic Pop", "Viral Ballad", "Sweet" }
        });

        // 50. Si Lemah - RAN feat. Hindia
        list.Add(new SongMission
        {
            Id = "SONG_050",
            Title = "Si Lemah",
            Artist = "RAN feat. Hindia",
            Category = "Indo_Band",
            Bpm = 90,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dmaj7 - C#m7 - Bm7 - A", Verse = "Dmaj7 - C#m7 - Bm7 - A", Chorus = "Dmaj7 - C#m7 - Bm7 - E - A" },
            Tags = new List<string> { "Modern Pop", "Self Love", "Chill" }
        });

        // =========================================================================
        // SECTION B: 50 WESTERN POP / ROCK SCHOOL BAND ANTHEMS (51-100)
        // =========================================================================

        // 51. Don't Look Back in Anger - Oasis
        list.Add(new SongMission
        {
            Id = "SONG_051",
            Title = "Don't Look Back in Anger",
            Artist = "Oasis",
            Category = "Western_Band",
            Bpm = 82,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - F - C - F", Verse = "C - G - Am - E - F - G - C", Chorus = "C - G - Am - E - F - G - C - Am - G" },
            Tags = new List<string> { "Britpop Legend", "Piano Intro", "World Anthem" }
        });

        // 52. Wonderwall - Oasis
        list.Add(new SongMission
        {
            Id = "SONG_052",
            Title = "Wonderwall",
            Artist = "Oasis",
            Category = "Western_Band",
            Bpm = 88,
            KeySignature = "F# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em7 - G - Dsus4 - A7sus4", Verse = "Em7 - G - Dsus4 - A7sus4", Chorus = "C - D - Em" },
            Tags = new List<string> { "Acoustic Anthem", "Crowd Favorite", "Britpop" }
        });

        // 53. Yellow - Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_053",
            Title = "Yellow",
            Artist = "Coldplay",
            Category = "Western_Band",
            Bpm = 88,
            KeySignature = "B Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Mellow Grand", "Ac.Piano", 71, 96, 2),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "B - F# - E - B", Verse = "B - F# - E - B", Chorus = "E - G#m - F# - E" },
            Tags = new List<string> { "Post-Britpop", "Romantic Rock", "Starlight" }
        });

        // 54. The Scientist - Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_054",
            Title = "The Scientist",
            Artist = "Coldplay",
            Category = "Western_Band",
            Bpm = 73,
            KeySignature = "F Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm7 - Bb - F - Fsus2", Verse = "Dm7 - Bb - F - Fsus2", Chorus = "Bb - F - C - F" },
            Tags = new List<string> { "Iconic Piano Riff", "Ballad", "Emotional" }
        });

        // 55. Fix You - Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_055",
            Title = "Fix You",
            Artist = "Coldplay",
            Category = "Western_Band",
            Bpm = 69,
            KeySignature = "Eb Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pipe Organ", "Organ", 71, 96, 33),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Eb - Gm - Cm7 - Bb", Verse = "Eb - Gm - Cm7 - Bb", Chorus = "Ab - Eb - Bb - Ab - Eb - Bb" },
            Tags = new List<string> { "Church Organ", "Arena Climax", "Tearjerker" }
        });

        // 56. Viva La Vida - Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_056",
            Title = "Viva La Vida",
            Artist = "Coldplay",
            Category = "Western_Band",
            Bpm = 138,
            KeySignature = "Ab Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Staccato String Ensemble", "Strings", 71, 96, 41),
            SplitTone = new TonePresetRef("Percussive Organ", "Organ", 71, 96, 32),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Db - Eb - Ab - Fm", Verse = "Db - Eb - Ab - Fm", Chorus = "Db - Eb - Ab - Fm" },
            Tags = new List<string> { "Staccato Strings", "Festival Staple", "Worldwide Hit" }
        });

        // 57. Adventure of a Lifetime - Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_057",
            Title = "Adventure of a Lifetime",
            Artist = "Coldplay",
            Category = "Western_Band",
            Bpm = 112,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pluck Synth", "Synth Lead", 71, 96, 68),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm7 - G - Am7", Verse = "Dm7 - G - Am7", Chorus = "Dm7 - G - Am7 - C" },
            Tags = new List<string> { "Disco Funk", "Kalimba Pluck", "Upbeat Dance" }
        });

        // 58. Sunday Morning - Maroon 5
        list.Add(new SongMission
        {
            Id = "SONG_058",
            Title = "Sunday Morning",
            Artist = "Maroon 5",
            Category = "Western_Band",
            Bpm = 88,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Finger Electric Bass", "Bass", 71, 96, 33),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm9 - G13 - Cmaj9", Verse = "Dm9 - G13 - Cmaj9", Chorus = "Dm9 - G13 - Cmaj9" },
            Tags = new List<string> { "Neo-Soul", "Jazz Chords", "School Band Standard" }
        });

        // 59. This Love - Maroon 5
        list.Add(new SongMission
        {
            Id = "SONG_059",
            Title = "This Love",
            Artist = "Maroon 5",
            Category = "Western_Band",
            Bpm = 95,
            KeySignature = "C Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Cm - Fm - Bb - Eb - G", Verse = "Cm - Fm - Bb - Eb - G", Chorus = "Cm - Fm - Bb - Eb - G" },
            Tags = new List<string> { "Staccato Piano", "Funk Pop", "High Energy" }
        });

        // 60. She Will Be Loved - Maroon 5
        list.Add(new SongMission
        {
            Id = "SONG_060",
            Title = "She Will Be Loved",
            Artist = "Maroon 5",
            Category = "Western_Band",
            Bpm = 102,
            KeySignature = "C Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Phase Rhodes", "E.Piano", 71, 96, 23),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Cm - Bb - Ab - Bb", Verse = "Cm - Bb - Ab - Bb", Chorus = "Eb - Bb - Cm - Ab" },
            Tags = new List<string> { "Pop Ballad", "Rhodes MK1", "Crowd Favorite" }
        });

        // 61. Payphone - Maroon 5
        list.Add(new SongMission
        {
            Id = "SONG_061",
            Title = "Payphone",
            Artist = "Maroon 5",
            Category = "Western_Band",
            Bpm = 110,
            KeySignature = "B Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pop Piano", "Ac.Piano", 71, 96, 4),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "B - F# - G#m - E", Verse = "B - F# - G#m - E", Chorus = "B - F# - G#m - E" },
            Tags = new List<string> { "Radio Pop", "Piano Riff", "Catchy" }
        });

        // 62. Moves Like Jagger - Maroon 5
        list.Add(new SongMission
        {
            Id = "SONG_062",
            Title = "Moves Like Jagger",
            Artist = "Maroon 5",
            Category = "Western_Band",
            Bpm = 128,
            KeySignature = "B Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Clavinet Funk", "E.Piano", 71, 96, 26),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bm - Em", Verse = "Bm - Em", Chorus = "Bm - Em" },
            Tags = new List<string> { "Disco Dance", "Funk Clav", "Party Vibe" }
        });

        // 63. It's My Life - Bon Jovi
        list.Add(new SongMission
        {
            Id = "SONG_063",
            Title = "It's My Life",
            Artist = "Bon Jovi",
            Category = "Western_Band",
            Bpm = 120,
            KeySignature = "C Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Cm - Ab - Bb", Verse = "Cm - Ab - Bb", Chorus = "Cm - Ab - Eb - Bb" },
            Tags = new List<string> { "Arena Rock", "Talkbox Stabs", "High Octane" }
        });

        // 64. Livin' on a Prayer - Bon Jovi
        list.Add(new SongMission
        {
            Id = "SONG_064",
            Title = "Livin' on a Prayer",
            Artist = "Bon Jovi",
            Category = "Western_Band",
            Bpm = 123,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s PolySynth", "Synth Lead", 71, 96, 65),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - C - D", Verse = "Em - C - D", Chorus = "Em - C - D - G - C - D" },
            Tags = new List<string> { "80s Rock", "Key Modulation", "Global Stadium Hit" }
        });

        // 65. Sweet Child O' Mine - Guns N' Roses
        list.Add(new SongMission
        {
            Id = "SONG_065",
            Title = "Sweet Child O' Mine",
            Artist = "Guns N' Roses",
            Category = "Western_Band",
            Bpm = 125,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - C - G - D", Verse = "D - C - G - D", Chorus = "A - C - D" },
            Tags = new List<string> { "Classic Rock", "Guitar Riff Accompaniment", "Legendary" }
        });

        // 66. Creep - Radiohead
        list.Add(new SongMission
        {
            Id = "SONG_066",
            Title = "Creep",
            Artist = "Radiohead",
            Category = "Western_Band",
            Bpm = 92,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Mellow Grand", "Ac.Piano", 71, 96, 2),
            SplitTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - B - C - Cm", Verse = "G - B - C - Cm", Chorus = "G - B - C - Cm" },
            Tags = new List<string> { "90s Grunge", "Alt Rock", "Iconic Progression" }
        });

        // 67. Karma Police - Radiohead
        list.Add(new SongMission
        {
            Id = "SONG_067",
            Title = "Karma Police",
            Artist = "Radiohead",
            Category = "Western_Band",
            Bpm = 75,
            KeySignature = "A Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Classic Upright Piano", "Ac.Piano", 71, 96, 5),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Am - D/F# - Em - G", Verse = "Am - D/F# - Em - G", Chorus = "C - D - G - F# - Em" },
            Tags = new List<string> { "Acoustic Piano", "Alt Masterpiece", "Sing-Along" }
        });

        // 68. High and Dry - Radiohead
        list.Add(new SongMission
        {
            Id = "SONG_068",
            Title = "High and Dry",
            Artist = "Radiohead",
            Category = "Western_Band",
            Bpm = 88,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F#m11 - Aadd9 - E", Verse = "F#m11 - Aadd9 - E", Chorus = "F#m11 - Aadd9 - E" },
            Tags = new List<string> { "Indie Rock", "Sweet Falsetto", "Campfire" }
        });

        // 69. No Surprises - Radiohead
        list.Add(new SongMission
        {
            Id = "SONG_069",
            Title = "No Surprises",
            Artist = "Radiohead",
            Category = "Western_Band",
            Bpm = 76,
            KeySignature = "F Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pluck Synth", "Synth Lead", 71, 96, 68),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F - Bbm", Verse = "F - Dm - Am - C", Chorus = "Bbm - C - F" },
            Tags = new List<string> { "Glockenspiel Riff", "Lullaby Rock", "Melancholy" }
        });

        // 70. Zombie - The Cranberries
        list.Add(new SongMission
        {
            Id = "SONG_070",
            Title = "Zombie",
            Artist = "The Cranberries",
            Category = "Western_Band",
            Bpm = 84,
            KeySignature = "E Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Em - C - G - D/F#", Verse = "Em - C - G - D/F#", Chorus = "Em - C - G - D/F#" },
            Tags = new List<string> { "Post-Punk Grunge", "Crowd Roar", "90s Heavy" }
        });

        // 71. Smells Like Teen Spirit - Nirvana
        list.Add(new SongMission
        {
            Id = "SONG_071",
            Title = "Smells Like Teen Spirit",
            Artist = "Nirvana",
            Category = "Western_Band",
            Bpm = 116,
            KeySignature = "F Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Rock Organ", "Organ", 71, 96, 31),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F - Bb - Ab - Db", Verse = "F - Bb - Ab - Db", Chorus = "F - Bb - Ab - Db" },
            Tags = new List<string> { "Grunge Revolution", "Iconic Riff", "Energy Blast" }
        });

        // 72. Basket Case - Green Day
        list.Add(new SongMission
        {
            Id = "SONG_072",
            Title = "Basket Case",
            Artist = "Green Day",
            Category = "Western_Band",
            Bpm = 175,
            KeySignature = "Eb Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Rock Drawbar Organ", "Organ", 71, 96, 31),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Eb - Bb - Cm - Gm - Ab - Eb - Bb", Verse = "Eb - Bb - Cm - Gm - Ab - Eb - Bb", Chorus = "Ab - Bb - Eb - Cm - Ab - Bb - Eb" },
            Tags = new List<string> { "Punk Rock", "Speed", "90s Skate Punk" }
        });

        // 73. Wake Me Up When September Ends - Green Day
        list.Add(new SongMission
        {
            Id = "SONG_073",
            Title = "Wake Me Up When September Ends",
            Artist = "Green Day",
            Category = "Western_Band",
            Bpm = 105,
            KeySignature = "G Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - G/F# - Em - D - C - Cm - G", Verse = "G - G/F# - Em - D - C - Cm - G", Chorus = "Em - Bm - C - G - D" },
            Tags = new List<string> { "Acoustic Punk Ballad", "Nostalgic", "Sing-Along" }
        });

        // 74. 21 Guns - Green Day
        list.Add(new SongMission
        {
            Id = "SONG_074",
            Title = "21 Guns",
            Artist = "Green Day",
            Category = "Western_Band",
            Bpm = 80,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm - Bb - F - C", Verse = "Dm - Bb - F - C", Chorus = "F - C - Dm - C - Bb - F - C" },
            Tags = new List<string> { "Rock Ballad", "Peace Anthem", "Transformers Ost" }
        });

        // 75. American Idiot - Green Day
        list.Add(new SongMission
        {
            Id = "SONG_075",
            Title = "American Idiot",
            Artist = "Green Day",
            Category = "Western_Band",
            Bpm = 186,
            KeySignature = "Ab Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Ab - Db - Gb - Db - Ab", Verse = "Ab - Db - Gb - Db - Ab", Chorus = "Db - Ab - Eb - Ab" },
            Tags = new List<string> { "High Speed Punk", "Punchy Brass", "Moshpit" }
        });

        // 76. Boulevard of Broken Dreams - Green Day
        list.Add(new SongMission
        {
            Id = "SONG_076",
            Title = "Boulevard of Broken Dreams",
            Artist = "Green Day",
            Category = "Western_Band",
            Bpm = 84,
            KeySignature = "F Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Wurly 200A", "E.Piano", 71, 96, 24),
            SplitTone = new TonePresetRef("Dark Pad", "Pad", 87, 85, 36),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Fm - Ab - Eb - Bb", Verse = "Fm - Ab - Eb - Bb", Chorus = "Db - Ab - Eb - Fm" },
            Tags = new List<string> { "Tremolo Vibe", "Global Hit", "School Band Classic" }
        });

        // 77. All The Small Things - Blink-182
        list.Add(new SongMission
        {
            Id = "SONG_077",
            Title = "All The Small Things",
            Artist = "Blink-182",
            Category = "Western_Band",
            Bpm = 148,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - G - F - G", Verse = "C - G - F - G", Chorus = "C - G - F - G" },
            Tags = new List<string> { "Pop Punk Staple", "Catchy Hook", "High Energy" }
        });

        // 78. I Miss You - Blink-182
        list.Add(new SongMission
        {
            Id = "SONG_078",
            Title = "I Miss You",
            Artist = "Blink-182",
            Category = "Western_Band",
            Bpm = 110,
            KeySignature = "A Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Felt Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Acoustic Upright Bass", "Bass", 71, 96, 32),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "A - C#m - D - E", Verse = "A - C#m - D - E", Chorus = "D - E - A - F#m" },
            Tags = new List<string> { "Acoustic Pop Punk", "Emo Vibe", "Classic" }
        });

        // 79. What's My Age Again? - Blink-182
        list.Add(new SongMission
        {
            Id = "SONG_079",
            Title = "What's My Age Again?",
            Artist = "Blink-182",
            Category = "Western_Band",
            Bpm = 158,
            KeySignature = "F# Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F# - C# - D#m - B", Verse = "F# - C# - D#m - B", Chorus = "B - C# - F# - D#m" },
            Tags = new List<string> { "Pop Punk Riff", "Youth Vibe", "Fast" }
        });

        // 80. Count on Me - Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_080",
            Title = "Count on Me",
            Artist = "Bruno Mars",
            Category = "Western_Band",
            Bpm = 88,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Classic Upright Piano", "Ac.Piano", 71, 96, 5),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C - Em - Am - G - F", Verse = "C - Em - Am - G - F", Chorus = "F - G - Em - Am - Dm - G - C" },
            Tags = new List<string> { "Acoustic Pop", "Friendship", "School Camp" }
        });

        // 81. Just the Way You Are - Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_081",
            Title = "Just the Way You Are",
            Artist = "Bruno Mars",
            Category = "Western_Band",
            Bpm = 109,
            KeySignature = "F Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s DX Electric Piano", "E.Piano", 71, 96, 25),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "F - Dm - Bb - F", Verse = "F - Dm - Bb - F", Chorus = "F - Dm - Bb - F" },
            Tags = new List<string> { "Romantic Pop", "DX7 Keys", "Huge Sing-Along" }
        });

        // 82. Treasure - Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_082",
            Title = "Treasure",
            Artist = "Bruno Mars",
            Category = "Western_Band",
            Bpm = 116,
            KeySignature = "Eb Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Bright Brass Synth", "Synth Brass", 87, 88, 12),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Abmaj7 - Gm7 - Fm7 - Bbsus4", Verse = "Abmaj7 - Gm7 - Fm7 - Bbsus4", Chorus = "Abmaj7 - Gm7 - Fm7 - Bbsus4" },
            Tags = new List<string> { "Disco Funk", "Groovy Brass", "Dance" }
        });

        // 83. Uptown Funk - Mark Ronson ft. Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_083",
            Title = "Uptown Funk",
            Artist = "Mark Ronson ft. Bruno Mars",
            Category = "Western_Band",
            Bpm = 115,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Tower of Power Horns", "Synth Brass", 87, 88, 11),
            SplitTone = new TonePresetRef("Funk Slap Bass", "Bass", 71, 96, 35),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm7 - G7", Verse = "Dm7 - G7", Chorus = "Dm7 - G7" },
            Tags = new List<string> { "Funk Monster", "Brass Stabs", "Festival Banger" }
        });

        // 84. Locked Out of Heaven - Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_084",
            Title = "Locked Out of Heaven",
            Artist = "Bruno Mars",
            Category = "Western_Band",
            Bpm = 144,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s PolySynth", "Synth Lead", 71, 96, 65),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm - C - Bb - F - Gm", Verse = "Dm - C - Bb - F - Gm", Chorus = "Bb - C - Dm - F" },
            Tags = new List<string> { "New Wave Pop", "High Energy", "Police Vibe" }
        });

        // 85. When I Was Your Man - Bruno Mars
        list.Add(new SongMission
        {
            Id = "SONG_085",
            Title = "When I Was Your Man",
            Artist = "Bruno Mars",
            Category = "Western_Band",
            Bpm = 73,
            KeySignature = "C Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D7 - F - C", Verse = "Am - C - Dm - G - C", Chorus = "F - G - C - G/B - Am - D7 - F - Fm - C" },
            Tags = new List<string> { "Solo Piano Masterclass", "Soul Ballad", "Emotional" }
        });

        // 86. Believer - Imagine Dragons
        list.Add(new SongMission
        {
            Id = "SONG_086",
            Title = "Believer",
            Artist = "Imagine Dragons",
            Category = "Western_Band",
            Bpm = 125,
            KeySignature = "Bb Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Aggressive Dist Synth", "Synth Lead", 71, 96, 67),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bbm - Gb - F", Verse = "Bbm - Gb - F", Chorus = "Bbm - Gb - F" },
            Tags = new List<string> { "Industrial Pop", "Heavy Hits", "Stadium Anthem" }
        });

        // 87. Demons - Imagine Dragons
        list.Add(new SongMission
        {
            Id = "SONG_087",
            Title = "Demons",
            Artist = "Imagine Dragons",
            Category = "Western_Band",
            Bpm = 90,
            KeySignature = "Eb Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pop Piano", "Ac.Piano", 71, 96, 4),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Eb - Bb - Cm - Ab", Verse = "Eb - Bb - Cm - Ab", Chorus = "Eb - Bb - Cm - Ab" },
            Tags = new List<string> { "Power Pop", "Huge Chorus", "Band Standard" }
        });

        // 88. Radioactive - Imagine Dragons
        list.Add(new SongMission
        {
            Id = "SONG_088",
            Title = "Radioactive",
            Artist = "Imagine Dragons",
            Category = "Western_Band",
            Bpm = 136,
            KeySignature = "B Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bm - D - A - E", Verse = "Bm - D - A - E", Chorus = "Bm - D - A - E" },
            Tags = new List<string> { "Dubstep Rock", "Massive Bass", "Arena Anthem" }
        });

        // 89. Counting Stars - OneRepublic
        list.Add(new SongMission
        {
            Id = "SONG_089",
            Title = "Counting Stars",
            Artist = "OneRepublic",
            Category = "Western_Band",
            Bpm = 122,
            KeySignature = "C# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pop Piano", "Ac.Piano", 71, 96, 4),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C#m - E - B - A", Verse = "C#m - E - B - A", Chorus = "C#m - E - B - A" },
            Tags = new List<string> { "Folk Pop", "Dance Beats", "Worldwide Smash" }
        });

        // 90. Apologize - OneRepublic
        list.Add(new SongMission
        {
            Id = "SONG_090",
            Title = "Apologize",
            Artist = "OneRepublic",
            Category = "Western_Band",
            Bpm = 118,
            KeySignature = "C Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Cm - Ab - Eb - Bb", Verse = "Cm - Ab - Eb - Bb", Chorus = "Cm - Ab - Eb - Bb" },
            Tags = new List<string> { "Classic Piano Riff", "Pop Legend", "Timeless" }
        });

        // 91. Secrets - OneRepublic
        list.Add(new SongMission
        {
            Id = "SONG_091",
            Title = "Secrets",
            Artist = "OneRepublic",
            Category = "Western_Band",
            Bpm = 148,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pizzicato Strings", "Strings", 71, 96, 45),
            SplitTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - F#m - Bm - G", Verse = "D - F#m - Bm - G", Chorus = "D - F#m - Bm - G" },
            Tags = new List<string> { "Pizzicato Cello", "Orchestral Rock", "Anthem" }
        });

        // 92. Photograph - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_092",
            Title = "Photograph",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 108,
            KeySignature = "E Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Felt Piano", "Ac.Piano", 71, 96, 3),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "E - C#m - B - A", Verse = "E - C#m - B - A", Chorus = "E - B - C#m - A" },
            Tags = new List<string> { "Acoustic Pop", "Sweet Melody", "Graduation Vibe" }
        });

        // 93. Perfect - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_093",
            Title = "Perfect",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 63,
            KeySignature = "Ab Major",
            Transpose = 0,
            TimeSignature = "12/8",
            PrimaryTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitTone = new TonePresetRef("Warm Strings", "Strings", 71, 96, 42),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Ab - Fm - Db - Eb", Verse = "Ab - Fm - Db - Eb", Chorus = "Fm - Db - Ab - Eb" },
            Tags = new List<string> { "Slow Waltz 12/8", "Wedding Standard", "Mega Hit" }
        });

        // 94. Thinking Out Loud - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_094",
            Title = "Thinking Out Loud",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 79,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("70s Suitcase EP", "E.Piano", 71, 96, 22),
            SplitTone = new TonePresetRef("Finger Electric Bass", "Bass", 71, 96, 33),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - D/F# - G - A", Verse = "D - D/F# - G - A", Chorus = "Em - A - D" },
            Tags = new List<string> { "Blue-Eyed Soul", "Guitar Strum Accompaniment", "Classic" }
        });

        // 95. Shape of You - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_095",
            Title = "Shape of You",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 96,
            KeySignature = "C# Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Marimba Pluck", "Synth Lead", 71, 96, 68),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "C#m - F#m - A - B", Verse = "C#m - F#m - A - B", Chorus = "C#m - F#m - A - B" },
            Tags = new List<string> { "Marimba Hook", "Global Record", "Dance Pop" }
        });

        // 96. Castle on the Hill - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_096",
            Title = "Castle on the Hill",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 135,
            KeySignature = "D Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Shimmer Pad", "Pad", 87, 85, 38),
            SplitTone = new TonePresetRef("Concert Piano", "Ac.Piano", 71, 96, 1),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "D - D/F# - G", Verse = "D - D/F# - G", Chorus = "G - A - D - Bm" },
            Tags = new List<string> { "Stadium Rock", "Nostalgic Youth", "Epic Drive" }
        });

        // 97. Shivers - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_097",
            Title = "Shivers",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 141,
            KeySignature = "D Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pizzicato Strings", "Strings", 71, 96, 45),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Dm - Bb - C - Am", Verse = "Dm - Bb - C - Am", Chorus = "Dm - Bb - C - Am" },
            Tags = new List<string> { "Pizzicato Dance", "Upbeat", "Radio Standard" }
        });

        // 98. Bad Habits - Ed Sheeran
        list.Add(new SongMission
        {
            Id = "SONG_098",
            Title = "Bad Habits",
            Artist = "Ed Sheeran",
            Category = "Western_Band",
            Bpm = 126,
            KeySignature = "B Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Saw Lead Synth", "Synth Lead", 71, 96, 64),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Bm - D - G - Em", Verse = "Bm - D - G - Em", Chorus = "Bm - D - G - Em" },
            Tags = new List<string> { "Euro Dance", "Synth Bass Groove", "Catchy" }
        });

        // 99. Something Just Like This - The Chainsmokers & Coldplay
        list.Add(new SongMission
        {
            Id = "SONG_099",
            Title = "Something Just Like This",
            Artist = "The Chainsmokers & Coldplay",
            Category = "Western_Band",
            Bpm = 103,
            KeySignature = "B Minor",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("80s PolySynth", "Synth Lead", 71, 96, 65),
            SplitTone = new TonePresetRef("Sub Heavy Synth Bass", "Bass", 87, 89, 22),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "G - A - Bm - F#m", Verse = "G - A - Bm - F#m", Chorus = "G - A - Bm - F#m" },
            Tags = new List<string> { "EDM Superchords", "Festival Climax", "Huge Sing-Along" }
        });

        // 100. Closer - The Chainsmokers ft. Halsey
        list.Add(new SongMission
        {
            Id = "SONG_100",
            Title = "Closer",
            Artist = "The Chainsmokers ft. Halsey",
            Category = "Western_Band",
            Bpm = 95,
            KeySignature = "Ab Major",
            Transpose = 0,
            TimeSignature = "4/4",
            PrimaryTone = new TonePresetRef("Pluck Synth", "Synth Lead", 71, 96, 68),
            SplitTone = new TonePresetRef("Warm Pad", "Pad", 87, 85, 34),
            SplitPoint = 60,
            ChordProgression = new ChordProgressionInfo { Intro = "Db - Eb - Fm - Eb", Verse = "Db - Eb - Fm - Eb", Chorus = "Db - Eb - Fm - Eb" },
            Tags = new List<string> { "Electronic Pop", "Pluck Hook", "Generation Anthem" }
        });

        // Auto-generate chords and lyrics timeline for any song missions that need them
        foreach (var song in list)
        {
            song.EnsureTimelineGenerated();
        }

        return list;
    }

    /// <summary>
    /// Compiles all 100 canonical song missions into setlists/master_100_repertoire.json
    /// </summary>
    public static string ExportMasterRepertoireJson(string? targetFilePath = null)
    {
        string path = targetFilePath ?? Path.Combine(Directory.GetCurrentDirectory(), "setlists", "master_100_repertoire.json");
        string dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var songs = Build100Repertoire();
        string json = JsonSerializer.Serialize(songs, JsonOptions);
        File.WriteAllText(path, json);
        return path;
    }
}
