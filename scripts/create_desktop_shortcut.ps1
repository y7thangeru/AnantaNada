$WshShell = New-Object -ComObject WScript.Shell
$DesktopPath = [System.Environment]::GetFolderPath('Desktop')
$ShortcutPath = Join-Path $DesktopPath "AnantaNada - Roland GoKeys 3.lnk"

$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = "d:\AntiGravity_Project\AnantaNada_RolandGoKeys3\launch_anantanada.bat"
$Shortcut.WorkingDirectory = "d:\AntiGravity_Project\AnantaNada_RolandGoKeys3"
$Shortcut.Description = "AnantaNada Hybrid Workstation for Roland Go:Keys 3"
$Shortcut.IconLocation = "C:\Godot\Godot_v4.5-stable_win64.exe,0"
$Shortcut.Save()

Write-Host "Desktop shortcut created at: $ShortcutPath"
