using AnantaNada.Tests;

namespace AnantaNada;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine("[ANANTANADA HYBRID WORKSTATION] Host Process Initializing...");
        return await VerificationHarness.RunAll();
    }
}
