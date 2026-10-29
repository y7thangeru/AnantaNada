# ANANTANADA Performance Pads Engine

Inspired by **Ohpe's `PerformancePads.cpp`**, the Performance Pads Engine brings game-grade tactile live performance gestures and DJ throw effects to the Roland Go:Keys 3 stage workstation:

1. **Quantized Stutter**: Automatically pulses and retriggers held keys at exact beat fractions (1/4, 1/8, 1/16, 1/32).
2. **Tape Stop (Vinyl Brake)**: Generates a smooth, exponential pitch bend drop down to `-8192` with mute cutoff at duration completion.
3. **Pitch Riser / Transition**: Ascending pitch bend sweep coupled with MIDI CC 74 (Filter Cutoff / Brightness) crescendo.
4. **Filter Sweep Riser**: Dynamic sweeps of CC 74 (Cutoff) and CC 71 (Resonance) over selectable bar increments.
5. **Reverb Freeze Throw**: Captures active chords into an ambient sustained soundscape via CC 91 (Reverb Send = 127) and CC 64 (Sustain = 127) before clean release.

## API Usage

```csharp
var pads = new PerformancePadsEngine();
pads.OnPitchBend += (ch, bend) => midiEngine.SendPitchBend(ch, bend);
pads.OnControlChange += (ch, cc, val) => midiEngine.SendControlChange(ch, cc, val);

// Engage Stutter
pads.SetStutterDivision(bpm: 128, subdivision: 16);
pads.TriggerPad(PerformancePadType.Stutter, active: true, channel: 0);

// Advance continuous tick
pads.Tick(deltaSeconds);
```
