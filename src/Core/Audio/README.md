# ANANTANADA Parametric 3D Head Model Spatializer

Inspired by **Ohpe's `Space3D.cpp`**, this audio engine delivers binaural 3D spatial separation for live musician In-Ear Monitors (IEMs) without requiring heavy HRTF databases.

## Acoustic Foundations

1. **Woodworth Interaural Time Difference (ITD)**:
   $$\Delta t = \frac{r}{c}(\theta + \sin\theta)$$
   where $r = 0.0875\text{ m}$ (head radius) and $c = 343\text{ m/s}$ (speed of sound).
2. **Head Shadow Attenuation**: Attenuates high frequencies on the contralateral ear via low-pass shelving filters.
3. **Pinna Elevation Notch**: Shifts spectral notches between $6.5\text{ kHz}$ and $10\text{ kHz}$ based on elevation angle.

## 4-Bus In-Ear Monitor (IEM) Layout

- **Bus 1: Click / Metronome**: Azimuth $-45^\circ$, Elevation $+35^\circ$ (High Left)
- **Bus 2: Audio Cue Prompts**: Azimuth $+45^\circ$, Elevation $+35^\circ$ (High Right)
- **Bus 3: Keyboard / Synth**: Azimuth $0^\circ$, Elevation $0^\circ$ (Center Front)
- **Bus 4: Backing Track**: Azimuth $-75^\circ$, Elevation $5^\circ$ (Wide Front)

This spatial separation prevents ear fatigue and keeps clicks crystal-clear during high-decibel live stage concerts.
