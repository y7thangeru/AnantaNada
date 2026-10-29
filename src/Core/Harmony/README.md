# ANANTANADA Harmony Brain & Key Gravity

Inspired by **Ohpe's `HarmonyBrain.cpp` and `KeyGravity.cpp`**, this engine brings real-time music theory intelligence to live stage performance on the Roland Go:Keys 3:

1. **Real-time Polyphonic Chord Recognition**: Analyzes held notes on the keybed and identifies chord root, quality (Major, Minor, Diminished, Augmented, Dominant 7th, maj7, m7, sus2, sus4, add9), and slash bass inversions.
2. **Key Gravity (Stage-Safe Magnetic Pitch Clamping)**: When enabled, pulls off-scale semitone slips to the nearest valid scale degree in real time, preventing cringe-worthy live performance mistakes while playing fast runs.
3. **Telemetry & Visual HUD Feedback**: Emits chord updates via WebSocket to connected stage teleprompters and the Godot HUD.

## API Usage

```csharp
var brain = new HarmonyBrain();
brain.SetScale(rootNote: 0, MusicalScale.Major); // C Major
brain.GravityMode = KeyGravityMode.SnapNearest;

brain.OnChordDetected += chord => {
    Console.WriteLine($"Current Chord: {chord.Name} (Root: {chord.Root}, Bass: {chord.Bass})");
};

// Process Note On
byte effectiveNote = brain.ProcessNoteOn(note: 61, velocity: 100); // C# slips snap to C (60) or D (62)
```
