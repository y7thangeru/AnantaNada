using System.Text.Json.Serialization;
using AnantaNada.Core.Models;
using AnantaNada.Core.Services;

namespace AnantaNada.Core.Data;

public sealed class SoundGenome
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Hybrid Breed";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "SYNTH";

    [JsonPropertyName("bankMsb")]
    public byte BankMsb { get; set; }

    [JsonPropertyName("bankLsb")]
    public byte BankLsb { get; set; }

    [JsonPropertyName("programChange")]
    public byte ProgramChange { get; set; }

    [JsonPropertyName("cutoff")]
    public byte Cutoff { get; set; } = 64; // CC 74

    [JsonPropertyName("resonance")]
    public byte Resonance { get; set; } = 64; // CC 71

    [JsonPropertyName("attack")]
    public byte Attack { get; set; } = 64; // CC 73

    [JsonPropertyName("release")]
    public byte Release { get; set; } = 64; // CC 72

    [JsonPropertyName("reverbSend")]
    public byte ReverbSend { get; set; } = 40; // CC 91

    [JsonPropertyName("chorusSend")]
    public byte ChorusSend { get; set; } = 0; // CC 93

    [JsonPropertyName("generation")]
    public int Generation { get; set; } = 1;

    [JsonPropertyName("parents")]
    public string Parents { get; set; } = "Pure";
}

/// <summary>
/// Sound Genome & Preset Breeding Engine inspired by Ohpe's SoundGenome.cpp and RandomSound.cpp.
/// Performs genetic crossover and constrained musical mutation between two Roland Go:Keys 3 tones
/// to breed unique hybrid soundscapes without tedious menu diving.
/// </summary>
public sealed class SoundGenomeBreeder
{
    private readonly Random _rng = new();

    public SoundGenome CreateGenomeFromTone(ToneItem tone)
    {
        return new SoundGenome
        {
            Name = tone.Name,
            Category = tone.Category,
            BankMsb = tone.Msb,
            BankLsb = tone.Lsb,
            ProgramChange = tone.RawProgramChange,
            Cutoff = 64,
            Resonance = 64,
            Attack = 64,
            Release = 64,
            ReverbSend = 40,
            ChorusSend = 10,
            Generation = 0,
            Parents = tone.Name
        };
    }

    /// <summary>
    /// Breeds two parent genomes with genetic crossover and optional mutation.
    /// </summary>
    public SoundGenome Breed(SoundGenome parentA, SoundGenome parentB, double biasTowardsA = 0.5, double mutationRate = 0.15)
    {
        biasTowardsA = Math.Clamp(biasTowardsA, 0.0, 1.0);

        // Core tone selection: Choose base patch from parent A or B based on bias
        bool pickBaseA = _rng.NextDouble() < biasTowardsA;
        var baseParent = pickBaseA ? parentA : parentB;
        var altParent = pickBaseA ? parentB : parentA;

        var child = new SoundGenome
        {
            Name = $"{parentA.Name.Split(' ')[0]} x {parentB.Name.Split(' ')[0]}",
            Category = baseParent.Category,
            BankMsb = baseParent.BankMsb,
            BankLsb = baseParent.BankLsb,
            ProgramChange = baseParent.ProgramChange,
            Cutoff = BlendParam(parentA.Cutoff, parentB.Cutoff, biasTowardsA),
            Resonance = BlendParam(parentA.Resonance, parentB.Resonance, biasTowardsA),
            Attack = BlendParam(parentA.Attack, parentB.Attack, biasTowardsA),
            Release = BlendParam(parentA.Release, parentB.Release, biasTowardsA),
            ReverbSend = BlendParam(parentA.ReverbSend, parentB.ReverbSend, biasTowardsA),
            ChorusSend = BlendParam(parentA.ChorusSend, parentB.ChorusSend, biasTowardsA),
            Generation = Math.Max(parentA.Generation, parentB.Generation) + 1,
            Parents = $"{parentA.Name} + {parentB.Name}"
        };

        // Apply mutation if triggered
        if (_rng.NextDouble() < mutationRate)
        {
            // Mutate Cutoff (CC 74) by +/- 15
            int deltaCutoff = _rng.Next(-15, 16);
            child.Cutoff = (byte)Math.Clamp(child.Cutoff + deltaCutoff, 20, 127);

            // Mutate Reverb by +/- 20
            int deltaRev = _rng.Next(-20, 21);
            child.ReverbSend = (byte)Math.Clamp(child.ReverbSend + deltaRev, 0, 127);

            // Mutate Resonance
            int deltaRes = _rng.Next(-10, 15);
            child.Resonance = (byte)Math.Clamp(child.Resonance + deltaRes, 30, 110);
        }

        return child;
    }

    private static byte BlendParam(byte valA, byte valB, double bias)
    {
        double blended = (valA * bias) + (valB * (1.0 - bias));
        return (byte)Math.Clamp((int)Math.Round(blended), 0, 127);
    }
}
