[Windows.Devices.Enumeration.DeviceInformation, Windows.Foundation, ContentType = WindowsRuntime] | Out-Null
[Windows.Devices.Midi.MidiInPort, Windows.Devices.Midi, ContentType = WindowsRuntime] | Out-Null
[Windows.Devices.Midi.MidiOutPort, Windows.Devices.Midi, ContentType = WindowsRuntime] | Out-Null

$selectorIn = [Windows.Devices.Midi.MidiInPort]::GetDeviceSelector()
$opIn = [Windows.Devices.Enumeration.DeviceInformation]::FindAllAsync($selectorIn)
$taskIn = [System.WindowsRuntimeSystemExtensions]::AsTask($opIn)
$taskIn.Wait()
$devicesIn = $taskIn.Result

Write-Host "=== WINDOWS BLE / UWP MIDI IN DEVICES ==="
foreach ($d in $devicesIn) {
    Write-Host ("Name: " + $d.Name + " | ID: " + $d.Id + " | Enabled: " + $d.IsEnabled)
}

$selectorOut = [Windows.Devices.Midi.MidiOutPort]::GetDeviceSelector()
$opOut = [Windows.Devices.Enumeration.DeviceInformation]::FindAllAsync($selectorOut)
$taskOut = [System.WindowsRuntimeSystemExtensions]::AsTask($opOut)
$taskOut.Wait()
$devicesOut = $taskOut.Result

Write-Host "`n=== WINDOWS BLE / UWP MIDI OUT DEVICES ==="
foreach ($d in $devicesOut) {
    Write-Host ("Name: " + $d.Name + " | ID: " + $d.Id + " | Enabled: " + $d.IsEnabled)
}
