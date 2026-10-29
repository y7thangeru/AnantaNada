namespace AnantaNada.Core.Audio;

public sealed class Spatial3DChannelConfig
{
    public string Name { get; set; } = "Bus";
    public double AzimuthDeg { get; set; }    // -180 to +180 deg (0 = Front, -90 = Left, +90 = Right, 180 = Behind)
    public double ElevationDeg { get; set; }  // -90 to +90 deg (0 = Level, +90 = Directly overhead)
    public double DistanceMeters { get; set; } = 1.0;
}

public sealed class BinauralCoefficients
{
    public double LeftGain { get; set; }
    public double RightGain { get; set; }
    public double LeftDelayMs { get; set; }
    public double RightDelayMs { get; set; }
    public double LeftCutoffHz { get; set; }
    public double RightCutoffHz { get; set; }
    public double PinnaNotchHz { get; set; }
}

/// <summary>
/// Parametric 3D Head Model Spatializer inspired by Ohpe's Space3D.cpp.
/// Uses mathematical acoustic equations (Woodworth ITD, spherical head shadow, pinna elevation notch)
/// to create an expansive, low-latency 3D binaural soundfield for musician In-Ear Monitors (IEM).
/// </summary>
public sealed class SpatialAudioMatrix
{
    private const double HeadRadiusMeters = 0.0875; // Average adult head radius ~8.75 cm
    private const double SpeedOfSound = 343.0;      // m/s in air at 20°C

    private readonly Dictionary<string, Spatial3DChannelConfig> _buses = new();

    public IReadOnlyDictionary<string, Spatial3DChannelConfig> Buses => _buses;

    public SpatialAudioMatrix()
    {
        // 4 Standard Stage In-Ear Monitor (IEM) Spatial Buses:
        // Bus 1: Click / Metronome (High Left, elevated above ear canal)
        _buses["Click"] = new Spatial3DChannelConfig { Name = "Click", AzimuthDeg = -45.0, ElevationDeg = 35.0, DistanceMeters = 1.2 };
        // Bus 2: Vocal Cue Prompter (High Right, separated from click)
        _buses["Cue"] = new Spatial3DChannelConfig { Name = "Cue", AzimuthDeg = 45.0, ElevationDeg = 35.0, DistanceMeters = 1.2 };
        // Bus 3: Roland Go:Keys 3 Live Keyboard (Center Front)
        _buses["Keyboard"] = new Spatial3DChannelConfig { Name = "Keyboard", AzimuthDeg = 0.0, ElevationDeg = 0.0, DistanceMeters = 1.0 };
        // Bus 4: Backing Track (Wide Stage Front)
        _buses["BackingTrack"] = new Spatial3DChannelConfig { Name = "BackingTrack", AzimuthDeg = -75.0, ElevationDeg = 5.0, DistanceMeters = 2.0 };
    }

    public void SetBusPosition(string busName, double azimuthDeg, double elevationDeg, double distanceMeters)
    {
        if (_buses.TryGetValue(busName, out var cfg))
        {
            cfg.AzimuthDeg = Math.Clamp(azimuthDeg, -180.0, 180.0);
            cfg.ElevationDeg = Math.Clamp(elevationDeg, -90.0, 90.0);
            cfg.DistanceMeters = Math.Max(0.1, distanceMeters);
        }
        else
        {
            _buses[busName] = new Spatial3DChannelConfig
            {
                Name = busName,
                AzimuthDeg = Math.Clamp(azimuthDeg, -180.0, 180.0),
                ElevationDeg = Math.Clamp(elevationDeg, -90.0, 90.0),
                DistanceMeters = Math.Max(0.1, distanceMeters)
            };
        }
    }

    /// <summary>
    /// Computes binaural gains, interaural time delays (ITD), and shadow filtering for a given bus.
    /// Implements the classical Woodworth model used in Ohpe.
    /// </summary>
    public BinauralCoefficients ComputeBinauralParameters(string busName)
    {
        if (!_buses.TryGetValue(busName, out var cfg))
        {
            cfg = new Spatial3DChannelConfig { Name = busName, AzimuthDeg = 0, ElevationDeg = 0, DistanceMeters = 1.0 };
        }

        double azRad = cfg.AzimuthDeg * Math.PI / 180.0;
        double elRad = cfg.ElevationDeg * Math.PI / 180.0;
        double dist = cfg.DistanceMeters;

        // Woodworth formula for Interaural Time Difference (ITD)
        double absAz = Math.Abs(azRad);
        double itdSeconds = (HeadRadiusMeters / SpeedOfSound) * (absAz + Math.Sin(absAz));
        double itdMs = itdSeconds * 1000.0;

        double leftDelayMs = 0;
        double rightDelayMs = 0;

        if (azRad > 0)
        {
            // Sound is on the right -> Left ear is shadowed/delayed
            leftDelayMs = itdMs;
            rightDelayMs = 0;
        }
        else
        {
            // Sound is on the left -> Right ear is shadowed/delayed
            leftDelayMs = 0;
            rightDelayMs = itdMs;
        }

        // Distance attenuation: 1 / distance (clamped to prevent infinite gain)
        double distanceGain = 1.0 / Math.Max(0.5, dist);

        // Interaural Level Difference (ILD) panning
        double pan = Math.Sin(azRad); // -1.0 (Left) to +1.0 (Right)
        double leftPannedGain = Math.Cos((pan + 1.0) * Math.PI / 4.0);
        double rightPannedGain = Math.Sin((pan + 1.0) * Math.PI / 4.0);

        // Head shadow filtering: Contralateral ear gets low-passed (2 kHz to 8 kHz)
        double leftCutoff = 20000.0;
        double rightCutoff = 20000.0;

        if (azRad > 0.1)
        {
            leftCutoff = Math.Max(2500.0, 20000.0 * (1.0 - (azRad / Math.PI) * 0.75));
        }
        else if (azRad < -0.1)
        {
            rightCutoff = Math.Max(2500.0, 20000.0 * (1.0 - (Math.Abs(azRad) / Math.PI) * 0.75));
        }

        // Pinna elevation notch (elevation shifts notch from 6 kHz up to 10 kHz)
        double pinnaNotchHz = 6500.0 + (elRad / (Math.PI / 2.0)) * 3500.0;

        return new BinauralCoefficients
        {
            LeftGain = leftPannedGain * distanceGain,
            RightGain = rightPannedGain * distanceGain,
            LeftDelayMs = leftDelayMs,
            RightDelayMs = rightDelayMs,
            LeftCutoffHz = leftCutoff,
            RightCutoffHz = rightCutoff,
            PinnaNotchHz = pinnaNotchHz
        };
    }
}
