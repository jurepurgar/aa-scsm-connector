$ErrorActionPreference = "Stop"
$MPName = "PurgarNET.AAConnector.Library"

$SMPath = (Get-ItemProperty 'hklm:/software/microsoft/System Center/2010/Service Manager/Setup').InstallDirectory
if (-not (Test-Path $(Join-Path $SMPath "Microsoft.Mom.ConfigServiceHost.exe"))) {
    throw "Service Manager server is not installed on this computer!";
}

Import-Module "$SMPath\Powershell\System.Center.Service.Manager.psm1" -Force

$newMPPath = Join-Path $PSScriptRoot "$MPName.mpb"
$existingMP = Get-SCManagementPack -Name $MPName
$newMP = Get-SCManagementPack -BundleFile $newMPPath

$importMP = $existingMP.Version -lt $newMP.Version

$toCopy = @()
$restartHealthService = $false

Get-ChildItem (Join-Path $PSScriptRoot "*.dll") | foreach {

    $existingDllPath = Join-Path $SMPath $_.Name
    $copy = $true

    if (Test-Path $existingDllPath) {
        $existingDll = Get-item $existingDllPath
        $copy = ($_.VersionInfo.ProductVersion -gt $existingDll.VersionInfo.ProductVersion)
        if ($copy) {
            $restartHealthService = $true
        }
    }

    if ($copy) {
        $toCopy += $_
    }
}

$go = $true
if ($toCopy.Length -gt 0 -and $restartHealthService) {
    Write-Warning "HealthService will have to be restarted!" 
    $answer = Read-Host @"
Do you want to continue?
[Y] Yes [N] No
"@
    $go = ($answer -like "Y")
}


if ($go) {
    if ($toCopy.Length -gt 0) {

        Write-Progress "Copying files..."
        if ($restartHealthService) {
            Restart-Service HealthService -force
        }

        $toCopy | foreach {
            Copy-Item $_ -Destination $SMPath
        }
        Write-Host "Files copied successfully."
    } 
    else {
        Write-Host "All files are up to date."
    }


    if ($importMP) {
        Import-SCManagementPack -Fullname $newMPPath
        Write-Host "Management pack imported successfully."
    } 
    else {
        Write-Host "Management pack is up to date."
    }

}

Write-Host "Press enter to continue..."
Read-Host