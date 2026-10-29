# ANANTANADA Euclidean & Orbit Polymetric Sequencer

Inspired by **Ohpe's `EuclidSequencer.cpp` and `OrbitSequencer.cpp`**, this engine generates Euclidean rhythms and polymetric patterns to drive Roland Go:Keys 3 accompaniment, drum grooves, and melodic arpeggios.

## Key Features

1. **Bjorklund Euclidean Algorithm**: Evenly spaces $K$ pulses over $N$ steps (e.g. Tresillo $E(3,8)$, Cinquillo $E(5,8)$, Bossa $E(7,16)$).
2. **Orbit Polymetric Sequencing**: Allows independent loop lengths per concentric ring (e.g. Ring 1 has 16 steps, Ring 2 has 5 steps, Ring 3 has 7 steps). The patterns drift organically against each other without repetitive fatigue.
3. **Scale Quantization**: Maps arpeggiator notes to musical scales (Major, Minor, Pentatonic, Dorian, Mixolydian, Blues, Hirajoshi).
4. **Hardware Routing**: Emits note hits directly to Roland Go:Keys 3 Channel 10 (Drum Kit) or Channel 1/4 (Melodic Synth).

## Quickstart

```csharp
var seq = new SequencerEngine();
seq.OnNoteTriggered += (ch, note, vel) => midiEngine.SendNoteOn(ch, note, vel);

// Advance 16th-note clock
seq.Start();
seq.StepTick();
```
