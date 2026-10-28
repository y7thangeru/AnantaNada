@echo off
title AnantaNada Hybrid Workstation Launcher
cd /d "d:\AntiGravity_Project\AnantaNada_RolandGoKeys3"

echo =========================================================================
echo   ANANTANADA HYBRID WORKSTATION (ROLAND GO:KEYS 3)
echo   Starting C# MIDI Engine + Godot Stage Console GUI...
echo =========================================================================

:: 1. Ensure latest C# Engine binary exists
if not exist "bin\Debug\net8.0\AnantaNada.exe" (
    echo Compiling C# Host Engine...
    dotnet build -c Debug
)

:: 2. Start C# MIDI Host Engine (handles Bluetooth & USB MIDI + WebSocket Server)
echo [1/2] Starting C# Bluetooth/USB MIDI Host Engine...
start "AnantaNada Engine Host" cmd /k "bin\Debug\net8.0\AnantaNada.exe"

:: Brief 2-second delay to ensure WebSocket listener is armed
timeout /t 2 /nobreak > nul

:: 3. Launch Godot Forward+ 1080p Stage Console GUI
echo [2/2] Launching Godot Stage Console GUI...
start "" "C:\Godot\Godot_v4.5-stable_win64.exe" --path "d:\AntiGravity_Project\AnantaNada_RolandGoKeys3"

echo.
echo >> Workstation active. Enjoy your gig/rehearsal!
