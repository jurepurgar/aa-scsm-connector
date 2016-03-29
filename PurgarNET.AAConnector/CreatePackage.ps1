$ErrorActionPreference = "Stop"
$Config = "Release"
$PackagesFolderName = "ReleasePackages"

function Set-AssemblyVersion
{
	param($Solution, $ProjectName, [Version]$Version)
	$proj = $Solution.projects | where Name -Like $ProjectName
	$proj.Properties.Item("AssemblyVersion").Value = $Version.ToString()
	$proj.Properties.Item("AssemblyFileVersion").Value = $Version.ToString()
}

Write-Output "Enter version:"
$version = [Version](Read-Host)
Write-Output "Creating package for version: $version"

$project = Get-Project
$solution = $dte.DTE.Solution

Write-Output "Setting versions..."
$mpProject = $solution.projects | where Name -Like "PurgarNET.AAConnector.Library"
$wfProject = $solution.projects | where Name -Like "PurgarNET.AAConnector.Workflows"

$currentVer = $mpProject.Object.GetProjectProperty("Version")
$mpProject.Object.SetProjectProperty("Version", $version.ToString())
$mpProject.Object.SetProjectProperty("DeploymentAutoIncrementVersion", "False")

Set-AssemblyVersion -Solution $solution -ProjectName "PurgarNET.AAConnector.Workflows" -Version $version
Set-AssemblyVersion -Solution $solution -ProjectName "PurgarNET.AAConnector.Shared" -Version $version
Set-AssemblyVersion -Solution $solution -ProjectName "PurgarNET.AAConnector.Console" -Version $version

Write-Output "Building..."
$sb = $solution.SolutionBuild
$currentConfig = $sb.ActiveConfiguration.Name
$sb.SolutionConfigurations.Item($Config).Activate()
$sb.Build($true)
$sb.SolutionConfigurations.Item($currentConfig).Activate()

Write-Output "Creating package..."

$mpProject.Object.SetProjectProperty("Version", $currentVer)
$mpProject.Object.SetProjectProperty("DeploymentAutoIncrementVersion", "True")

$mpBinPath = Join-Path $mpProject.Object.ProjectFolder "bin\$Config"
$mpPath = Join-Path $mpBinPath  $mpProject.Object.NodeProperties.OutputFileName

$packagesFolder = New-Item -ItemType Directory -Name $PackagesFolderName -Force

$packagePath = Join-Path $packagesFolder $("AAConnector-$version.zip")

Compress-Archive -Path $mpPath -DestinationPath $packagePath -Force 
Compress-Archive -Path "install.ps1" -DestinationPath $packagePath -Update

$fwBinFolder = Join-Path $(Split-Path $wfProject.FullName) "bin\$Config"
Join-Path $fwBinFolder "*.dll" | Get-ChildItem  | foreach {
	Compress-Archive -Path $_ -DestinationPath $packagePath -Update
}

Write-Output "done."

