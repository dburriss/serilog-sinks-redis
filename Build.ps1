$nugetProjects = @(".\src\Serilog.Sinks.Redis.Core", ".\src\Serilog.Sinks.Redis.List")
$testProjects = @(".\test\Serilog.Sinks.Redis.Tests", ".\test\Serilog.Sinks.Redis.IntegrationTests")

function EnsurePsbuildInstalled{  
    [cmdletbinding()]
    param(
        [string]$psbuildInstallUri = 'https://raw.githubusercontent.com/ligershark/psbuild/master/src/GetPSBuild.ps1'
    )
    process{
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            'Installing psbuild from [{0}]' -f $psbuildInstallUri | Write-Verbose
            (new-object Net.WebClient).DownloadString($psbuildInstallUri) | iex
        }
        else{
            'psbuild already loaded, skipping download' | Write-Verbose
        }

        # make sure it's loaded and throw if not
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            throw ('Unable to install/load psbuild from [{0}]' -f $psbuildInstallUri)
        }
    }
}

function Exec  
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

EnsurePsbuildInstalled
Write-Host "=================================================" -ForegroundColor Green
Write-Host "  RESTORE" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Exec { & dotnet restore }

Write-Host "=================================================" -ForegroundColor Green
Write-Host " BUILDING" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Invoke-MSBuild

$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$revision = "{0:D4}" -f [convert]::ToInt32($revision, 10)

foreach ($testProjectPath in $testProjects) {
	Write-Host "=================================================" -ForegroundColor Green
    Write-Host " TESTING " $testProjectPath -ForegroundColor Green
	Write-Host "=================================================" -ForegroundColor Green
    Exec { & dotnet test $testProjectPath -c Release -notrait "Category=Integration" }#can add dotnet test -notrait "Category=Integration" to skip tests marked [Trait("Category", "Integration")]
}

foreach ($projectPath in $nugetProjects) {
	Write-Host "=================================================" -ForegroundColor Green
    Write-Host " PACKAGING " $projectPath -ForegroundColor Green
	Write-Host "=================================================" -ForegroundColor Green
    Exec { & dotnet pack $projectPath -c Release -o .\artifacts --version-suffix=$revision } 
}

Write-Host "=================================================" -ForegroundColor Green
Write-Host "====================== DONE =====================" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green