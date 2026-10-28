namespace AnantaNada.Core.Midi;

/// <summary>
/// MIDI 1.0 status bytes, Roland Go:Keys 3 controller numbers, and safety parameters.
/// </summary>
public static class MidiConstants
{
    // Channel Voice Messages (high nibble)
    public const byte NoteOff = 0x80;
    public const byte NoteOn = 0x90;
    public const byte PolyphonicKeyPressure = 0xA0;
    public const byte ControlChange = 0xB0;
    public const byte ProgramChange = 0xC0;
    public const byte ChannelPressure = 0xD0;
    public const byte PitchBend = 0xE0;

    // System Messages
    public const byte SysExStart = 0xF0;
    public const byte SysExEnd = 0xF7;
    public const byte TimingClock = 0xF8;
    public const byte ActiveSensing = 0xFE;
    public const byte SystemReset = 0xFF;

    // Standard Controllers for Tone Navigation & Safety (Roland Go:Keys 3 Compatible)
    public const byte BankSelectMsb = 0x00; // CC 0: Bank Select MSB (Volatile)
    public const byte BankSelectLsb = 0x20; // CC 32: Bank Select LSB (Volatile)
    public const byte ModulationWheel = 0x01; // CC 1
    public const byte ChannelVolume = 0x07; // CC 7
    public const byte ExpressionController = 0x0B; // CC 11
    public const byte SustainPedal = 0x40; // CC 64: Hold 1 / Damper Pedal

    // Panic / Channel Mode Messages
    public const byte AllSoundOff = 0x78; // CC 120: Immediately silences all sound generators
    public const byte ResetAllControllers = 0x79; // CC 121
    public const byte AllNotesOff = 0x7B; // CC 123: Turns off all sounding notes

    // Go:Keys 3 Hardware Dimensions
    public const int TotalKeys = 61;
    public const int LowestNote = 36; // C2
    public const int HighestNote = 96; // C7
    public const int TotalChannels = 16;

    // Hardware Safety Constraints
    public const int SysExDebounceMs = 20; // Strict 20ms debounce between SysEx dispatches to prevent buffer overflow
}
