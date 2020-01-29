$MSBuild = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
$NuGetPath = '.\.nuget\NuGet.exe'
$SolutionFileName = 'HFM.All.sln'
$SetupSolutionFileName = 'HFM.Setup.sln'

[string]$Global:ArtifactsPath = ''
[string]$Global:ArtifactsBin = ''
[string]$Global:ArtifactsPackages = ''
[string]$Global:Version = ''
[string]$Global:Platform = ''

# Taken from psake https://github.com/psake/psake
Function Exec
{
    [CmdletBinding()]
    param([Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
          [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ("Error executing command {0}." -f $cmd))
    & $cmd
    if ($LastExitCode -ne 0) 
    {
        throw ("Exec: " + $errorMessage)
    }
}

Function Update-AssemblyVersion
{
    [CmdletBinding()]
    param([string]$Path, 
          [string]$AssemblyVersion=$Global:Version, 
          [string]$AssemblyFileVersion=$Global:Version)

    Write-Host "---------------------------------------------------"
    Write-Host "Updating Assembly Version"
    Write-Host " Path: $Path"
    Write-Host " AssemblyVersion: $AssemblyVersion"
    Write-Host " AssemblyFileVersion: $AssemblyFileVersion"
    Write-Host "---------------------------------------------------"

    $content = Get-Content -Path $Path -ErrorAction Stop |
     %{ $_ -Replace 'AssemblyVersion(Attribute)?\(.*\)', "AssemblyVersion(`"$AssemblyVersion`")" } |
     %{ $_ -Replace 'AssemblyFileVersion(Attribute)?\(.*\)', "AssemblyFileVersion(`"$AssemblyFileVersion`")" }
     
     Set-Content -Path $Path -Value $content -Encoding UTF8 -Force -ErrorAction Stop
}

Function Build-Solution
{
    param([string]$Target='Rebuild',
          [string]$Configuration='ScriptedRelease',
          [string]$Platform=$Global:Platform,
          [string]$AssemblyVersion=$Global:Version,
          [string]$AssemblyFileVersion=$Global:Version)

    Update-AssemblyVersion -Path 'ExeAssemblyVersion.cs' -AssemblyVersion $AssemblyVersion -AssemblyFileVersion $AssemblyFileVersion
    Update-AssemblyVersion -Path 'AssemblyVersion.cs' -AssemblyVersion '1.0.0.0' -AssemblyFileVersion $AssemblyFileVersion

    Write-Host "---------------------------------------------------"
    Write-Host "Building Solution"
    Write-Host " Target: $Target"
    Write-Host " Platform: $Platform"
    Write-Host "---------------------------------------------------"

    Exec { & $NuGetPath restore $SolutionFileName }
    Exec { & $MSBuild $SolutionFileName /t:$Target /p:Configuration=$Configuration`;Platform=$Platform`;NoWarn=1591 }
}

Function Test-Build
{
    param([string]$ArtifactsPath=$Global:ArtifactsPath)

    Write-Host "---------------------------------------------------"
    Write-Host "Testing Build"
    Write-Host " ArtifactsPath: $ArtifactsPath"
    Write-Host "---------------------------------------------------"
    
    $NUnitPath = 'packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe'
    Exec { & $NUnitPath .\HFM.Client.Tests\bin\Release\HFM.Client.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Client.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Core.Tests\bin\Release\HFM.Core.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Core.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Forms.Tests\bin\Release\HFM.Forms.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Forms.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Log.Tests\bin\Release\HFM.Log.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Log.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Preferences.Tests\bin\Release\HFM.Preferences.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Preferences.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Proteins.Tests\bin\Release\HFM.Proteins.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Proteins.Tests.Results.xml }
    Exec { & $NUnitPath .\HFM.Queue.Tests\bin\Release\HFM.Queue.Tests.dll --x86 --framework=net-4.5 --result=$ArtifactsPath\HFM.Queue.Tests.Results.xml }
}

Function Analyze-Build
{
    param([string]$ArtifactsPath=$Global:ArtifactsPath,
          [string]$ArtifactsBin=$Global:ArtifactsBin)

    Write-Host "---------------------------------------------------"
    Write-Host "Analyzing Build"
    Write-Host " ArtifactsPath: $ArtifactsPath"
    Write-Host " ArtifactsBin: $ArtifactsBin"
    Write-Host "---------------------------------------------------"

    $FxCopPath = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Enterprise\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe"
    Exec { & $FxCopPath /f:$ArtifactsBin\HFM.exe /f:$ArtifactsBin\HFM.*.dll /rs:=HFM.ruleset /dic:CustomDictionary.xml /d:..\lib\System.Data.SQLite\bin /out:$ArtifactsPath\FxCopReport.xml /gac }
}

Function Clean-Artifacts
{
    [CmdletBinding()]
    param([string]$ArtifactsPath=$Global:ArtifactsPath,
          [string]$ArtifactsBin=$Global:ArtifactsBin,
          [string]$ArtifactsPackages=$Global:ArtifactsPackages)

    Write-Host "---------------------------------------------------"
    Write-Host "Cleaning Artifacts"
    Write-Host " ArtifactsPath: $ArtifactsPath"
    Write-Host " ArtifactsBin: $ArtifactsBin"
    Write-Host " ArtifactsPackages: $ArtifactsPackages"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    if (Test-Path $ArtifactsPath -PathType Container)
    {
        Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Stop -Verbose:$localVerbose
    }
    #New-Item $ArtifactsPath -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    #New-Item $ArtifactsBin -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\SQLite\x86" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\SQLite\AMD64" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\SQLite\Mono" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\Tools" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\Documentation\License" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\CSS" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$ArtifactsBin\XSL" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item $ArtifactsPackages -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
}

Function Deploy-Build
{
    [CmdletBinding()]
    param([string]$ArtifactsBin=$Global:ArtifactsBin)

    Write-Host "---------------------------------------------------"
    Write-Host "Deploying Build"
    Write-Host " ArtifactsBin: $ArtifactsBin"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    # Primary Assemblies
    [string[]]$Assemblies = @(
        "HFM.exe", 
        "HFM.exe.config",
        "HFM.Client.dll",
        "HFM.Core.dll",
        "HFM.Forms.dll",
        "HFM.Log.dll",
        "HFM.Preferences.dll",
        "HFM.Proteins.dll",
        "HFM.Queue.dll",
        "harlam357.Core.dll",
        "harlam357.Windows.Forms.dll",
        "ZedGraph.dll",
        "Castle.Core.dll",
        "Castle.Windsor.dll",
        "protobuf-net.dll",
        "System.Linq.Dynamic.dll",
        "AutoMapper.dll",
        "Newtonsoft.Json.dll"
        )
    $AssemblyFiles = Get-ChildItem -Path 'HFM\bin\Release\*' -Include $Assemblies
    Copy-Item -Path $AssemblyFiles -Destination $ArtifactsBin -ErrorAction Stop -Verbose:$localVerbose
    # SQLite Assemblies
    Copy-Item -Path '..\lib\System.Data.SQLite\bin\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\x86" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\lib\System.Data.SQLite\bin\x64\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\AMD64" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\lib\SQLite.NET\bin\ManagedOnly\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\Mono" -ErrorAction Stop -Verbose:$localVerbose
    # Tools Assemblies
    Copy-Item -Path 'HFM.Client.Tool\bin\ReleaseMerge\HFM.Client.exe' -Destination "$ArtifactsBin\Tools" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path 'HFM.Log.Tool\bin\ReleaseMerge\HFM.Log.exe' -Destination "$ArtifactsBin\Tools" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path 'HFM.Queue.Tool\bin\ReleaseMerge\HFM.Queue.exe' -Destination "$ArtifactsBin\Tools" -ErrorAction Stop -Verbose:$localVerbose
    # Documentation & Licenses
    Copy-Item -Path '..\doc\GPLv2.TXT' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\ZedGraph License.txt' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\src\packages\Castle.Windsor.3.3.0\ASL - Apache Software Foundation License.txt' -Destination "$ArtifactsBin\Documentation\License\Windsor License.txt" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\protobuf-net Licence.txt' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\protoc-license.txt' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\AutoMapper License.txt' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\Json.NET License.txt' -Destination "$ArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    # CSS & XSL
    Copy-Item -Path 'HFM\bin\Release\CSS\*' -Destination "$ArtifactsBin\CSS" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path 'HFM\bin\Release\XSL\*' -Destination "$ArtifactsBin\XSL" -ErrorAction Stop -Verbose:$localVerbose
}

Function Build-Zip
{
    param([string]$ArtifactsBin=$Global:ArtifactsBin,
          [string]$ArtifactsPackages=$Global:ArtifactsPackages,
          [string]$Version=$Global:Version)

    Write-Host "---------------------------------------------------"
    Write-Host "Building Zip Package"
    Write-Host " ArtifactsBin: $ArtifactsBin"
    Write-Host " ArtifactsPackages: $ArtifactsPackages"
    Write-Host " Platform: $Platform"
    Write-Host " Version: $Version"
    Write-Host "---------------------------------------------------"

    Add-Type -assembly "System.IO.Compression.FileSystem"
    [IO.Compression.ZipFile]::CreateFromDirectory($ArtifactsBin, "$ArtifactsPackages\HFM $Version.zip") 
}

Function Build-Msi
{
    [CmdletBinding()]
    param([string]$Target='Rebuild',
          [string]$Configuration='Release',
          [string]$ArtifactsPackages=$Global:ArtifactsPackages,
          [string]$Platform=$Global:Platform,
          [string]$Version=$Global:Version)

    Write-Host "---------------------------------------------------"
    Write-Host "Building Msi Package"
    Write-Host " Target: $Target"
    Write-Host " Configuration: $Configuration"
    Write-Host " ArtifactsPackages: $ArtifactsPackages"
    Write-Host " Platform: $Platform"
    Write-Host " Version: $Version"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    Exec { & $MSBuild $SetupSolutionFileName /t:$Target /p:Configuration=$Configuration }
    Deploy-Msi -Configuration $Configuration -ArtifactsPackages $ArtifactsPackages -Version $Version
}

Function Deploy-Msi
{
    [CmdletBinding()]
    param([string]$Configuration='Release',
          [string]$ArtifactsPackages=$Global:ArtifactsPackages,
          [string]$Version=$Global:Version)

    Write-Host "---------------------------------------------------"
    Write-Host "Deploying Msi Package"
    Write-Host " Configuration: $Configuration"
    Write-Host " ArtifactsPackages: $ArtifactsPackages"
    Write-Host " Version: $Version"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    Copy-Item -Path "HFM.Setup\bin\$Configuration\HFM.Setup.msi" -Destination "$ArtifactsPackages\HFM $Version.msi" -ErrorAction Stop -Verbose:$localVerbose
}

Function Configure-Artifacts
{
    param([string]$Path)

    $Global:ArtifactsPath = $Path
    $Global:ArtifactsBin = "$Path\HFM.NET"
    $Global:ArtifactsPackages = "$Path\Packages"

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Artifacts"
    Write-Host " ArtifactsPath: $Global:ArtifactsPath"
    Write-Host " ArtifactsBin: $Global:ArtifactsBin"
    Write-Host " ArtifactsPackages: $Global:ArtifactsPackages"
    Write-Host "---------------------------------------------------"
}

Function Configure-Version
{
    param([string]$Version = $Global:Version, [switch]$CommitCount)

    if ($CommitCount)
    {
        $Commits = $(git rev-list HEAD --count | Out-String).Trim()
        $Version = $Version.Substring(0, $Version.LastIndexOf('.'))
        $Version = "$Version.$Commits"
    }

    $Global:Version = $Version

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Version"
    Write-Host " Version: $Version"
    Write-Host "---------------------------------------------------"
}

Function Configure-Platform
{
    param([string]$Platform)

    $Global:Platform = $Platform

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Platform"
    Write-Host " Platform: $Platform"
    Write-Host "---------------------------------------------------"
}

Configure-Artifacts -Path "$PSScriptRoot\Artifacts"
Configure-Version -Version '0.9.12.0'
Configure-Platform -Platform 'Any CPU'
