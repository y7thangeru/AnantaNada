# ANANTANADA Motion CC Recorder & Polymetric Looper

Inspired by **Ohpe's `MotionRecorder.cpp`**, the Motion CC Recorder captures hardware gestures (Modulation Wheel, Expression Pedal, Pitch Bend, and Filter Cutoff) and loops them as polymetric automation lanes.

## Core Capabilities

1. **Free Hands for Live Performance**: A stage keyboardist can sweep a filter or bend pitch for 2-4 bars once; ANANTANADA captures the curve and continues looping it smoothly while both hands play acoustic chords and melody.
2. **Smooth Linear Interpolation**: Sub-millisecond continuous interpolation ensures zero zipper noise or stepped jumps.
3. **Independent Polymetric Loop Durations**: Lanes can run at different lengths (e.g. 2.0s vs 3.5s), producing evolving, non-static timbres.

## Usage

```csharp
var motion = new MotionRecorder();
motion.OnControlChange += (ch, cc, val) => midiEngine.SendControlChange(ch, cc, val);

// Arm and record for 2 bars
motion.StartRecording(targetDuration: 2.0);
motion.RecordInput(controllerId: 74, value: 110); // CC 74 Filter Cutoff

// Loop playback continuously
motion.Tick(deltaSeconds);
```
