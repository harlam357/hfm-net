$MSBuild = "${Env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
$NuGetPath = '.\.nuget\NuGet.exe'
$SolutionFileName = 'HFM.All.sln'
$SetupSolutionFileName = 'HFM.Setup.sln'

[string]$Global:ArtifactsPath = ''
[string]$Global:ArtifactsBin = ''
[string]$Global:ArtifactsPackages = ''
[string]$Global:Version = ''
[string]$Global:Platform = ''

Function Update-AssemblyVersion
{
   param([string]$Path, 
         [string]$AssemblyVersion=$Global:Version, 
         [string]$AssemblyFileVersion=$Global:Version)

   $content = Get-Content -Path $Path |
    %{ $_ -Replace 'AssemblyVersion(Attribute)?\(.*\)', "AssemblyVersion(`"$AssemblyVersion`")" } |
    %{ $_ -Replace 'AssemblyFileVersion(Attribute)?\(.*\)', "AssemblyFileVersion(`"$AssemblyFileVersion`")" }

    Set-Content -Path $Path -Value $content -Encoding UTF8 -Force
}

Function Build-Solution
{
    param([string]$Target='Rebuild',
          [string]$Configuration='ScriptedRelease',
          [string]$Platform=$Global:Platform,
          [string]$DelaySign='true',
          [string]$AssemblyOriginatorKeyFile="$PSScriptRoot\harlam357public.snk",
          [string]$AssemblyVersion=$Global:Version,
          [string]$AssemblyFileVersion=$Global:Version)

    Update-AssemblyVersion -Path 'ExeAssemblyVersion.cs' -AssemblyVersion $AssemblyVersion -AssemblyFileVersion $AssemblyFileVersion
    Update-AssemblyVersion -Path 'AssemblyVersion.cs' -AssemblyVersion '1.0.0.0' -AssemblyFileVersion $AssemblyFileVersion

    . $NuGetPath restore $SolutionFileName
    . $MSBuild $SolutionFileName /t:$Target /p:Configuration=$Configuration`;Platform=$Platform`;DelaySign=$DelaySign`;AssemblyOriginatorKeyFile=$AssemblyOriginatorKeyFile`;NoWarn=1591
}

Function Test-Build
{
    param([string]$ArtifactsPath=$Global:ArtifactsPath)

    $NUnitPath = 'packages\NUnit.Runners.2.6.4\tools\nunit-console-x86.exe'
    . $NUnitPath .\HFM.Client.Tests\bin\Release\HFM.Client.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Client.Tests.Results.xml
    . $NUnitPath .\HFM.Core.Tests\bin\Release\HFM.Core.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Core.Tests.Results.xml
    . $NUnitPath .\HFM.Forms.Tests\bin\Release\HFM.Forms.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Forms.Tests.Results.xml
    . $NUnitPath .\HFM.Log.Tests\bin\Release\HFM.Log.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Log.Tests.Results.xml
    . $NUnitPath .\HFM.Preferences.Tests\bin\Release\HFM.Preferences.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Preferences.Tests.Results.xml
    . $NUnitPath .\HFM.Proteins.Tests\bin\Release\HFM.Proteins.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Proteins.Tests.Results.xml
    . $NUnitPath .\HFM.Queue.Tests\bin\Release\HFM.Queue.Tests.dll /framework=net-4.0 /xml=$ArtifactsPath\HFM.Queue.Tests.Results.xml
}

Function Analyze-Build
{
    param([string]$ArtifactsPath=$Global:ArtifactsPath,
          [string]$ArtifactsBin=$Global:ArtifactsBin)

    $FxCopPath = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio 14.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe"
    . $FxCopPath /f:$ArtifactsBin\HFM.exe /f:$ArtifactsBin\HFM.*.dll /rs:=HFM.ruleset /dic:CustomDictionary.xml /d:..\lib\System.Data.SQLite\bin /out:$ArtifactsPath\FxCopReport.xml /gac
}

Function Clean-Artifacts
{
    param([string]$ArtifactsPath=$Global:ArtifactsPath,
          [string]$ArtifactsBin=$Global:ArtifactsBin,
          [string]$ArtifactsPackages=$Global:ArtifactsPackages)

    if (Test-Path $ArtifactsPath -PathType Container)
    {
        Remove-Item $ArtifactsPath -Recurse -Force
    }
    #New-Item $ArtifactsPath -ItemType Directory > $null
    #New-Item $ArtifactsBin -ItemType Directory > $null
    New-Item "$ArtifactsBin\SQLite\x86" -ItemType Directory > $null
    New-Item "$ArtifactsBin\SQLite\AMD64" -ItemType Directory > $null
    New-Item "$ArtifactsBin\SQLite\Mono" -ItemType Directory > $null
    New-Item "$ArtifactsBin\Tools" -ItemType Directory > $null
    New-Item "$ArtifactsBin\Documentation\License" -ItemType Directory > $null
    New-Item "$ArtifactsBin\CSS" -ItemType Directory > $null
    New-Item "$ArtifactsBin\XSL" -ItemType Directory > $null
    New-Item $ArtifactsPackages -ItemType Directory > $null
}

Function Deploy-Build
{
    param([string]$ArtifactsBin=$Global:ArtifactsBin)

    # Primary Assemblies
    [string[]]$Assemblies = @(
        "HFM.exe", 
        "HFM.exe.config",
        "HFM.Client.dll",
        "HFM.Client.xml",
        "HFM.Core.dll",
        "HFM.Core.DataTypes.dll",
        "HFM.Core.DataTypes.xml",
        "HFM.Core.Plugins.dll",
        "HFM.Core.Plugins.xml",
        "HFM.Forms.dll",
        "HFM.Log.dll",
        "HFM.Log.xml",
        "HFM.Preferences.dll",
        "HFM.Proteins.dll",
        "HFM.Proteins.xml",
        "HFM.Queue.dll",
        "HFM.Queue.xml",
        "HTMLparser.dll",
        "harlam357.Core.dll",
        "harlam357.Windows.Forms.dll",
        "ZedGraph.dll",
        "ZedGraph.xml",
        "Castle.Core.dll",
        "Castle.Windsor.dll",
        "protobuf-net.dll",
        "protobuf-net.xml",
        "System.Linq.Dynamic.dll",
        "AutoMapper.dll",
        "Newtonsoft.Json.dll",
        "Newtonsoft.Json.xml"
        )
    $AssemblyFiles = Get-ChildItem -Path 'HFM\bin\Release\*' -Include $Assemblies
    Copy-Item -Path $AssemblyFiles -Destination $ArtifactsBin
    # SQLite Assemblies
    Copy-Item -Path '..\lib\System.Data.SQLite\bin\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\x86"
    Copy-Item -Path '..\lib\System.Data.SQLite\bin\x64\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\AMD64"
    Copy-Item -Path '..\lib\SQLite.NET\bin\ManagedOnly\System.Data.SQLite.dll' -Destination "$ArtifactsBin\SQLite\Mono"
    # Tools Assemblies
    Copy-Item -Path 'HFM.Client.Tool\bin\ReleaseMerge\HFM.Client.exe' -Destination "$ArtifactsBin\Tools"
    Copy-Item -Path 'HFM.Log.Tool\bin\ReleaseMerge\HFM.Log.exe' -Destination "$ArtifactsBin\Tools"
    Copy-Item -Path 'HFM.Queue.Tool\bin\ReleaseMerge\HFM.Queue.exe' -Destination "$ArtifactsBin\Tools"
    # Documentation & Licenses
    Copy-Item -Path '..\doc\GPLv2.TXT' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\lib\HTMLparser2\HTMLparser License.txt' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\doc\ZedGraph License.txt' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\src\packages\Castle.Windsor.3.3.0\ASL - Apache Software Foundation License.txt' -Destination "$ArtifactsBin\Documentation\License\Windsor License.txt"
    Copy-Item -Path '..\doc\protobuf-net Licence.txt' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\doc\protoc-license.txt' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\doc\AutoMapper License.txt' -Destination "$ArtifactsBin\Documentation\License"
    Copy-Item -Path '..\doc\Json.NET License.txt' -Destination "$ArtifactsBin\Documentation\License"
    # CSS & XSL
    Copy-Item -Path 'HFM\bin\Release\CSS\*' -Destination "$ArtifactsBin\CSS"
    Copy-Item -Path 'HFM\bin\Release\XSL\*' -Destination "$ArtifactsBin\XSL"
}

Function Build-Zip
{
    param([string]$ArtifactsBin=$Global:ArtifactsBin,
          [string]$ArtifactsPackages=$Global:ArtifactsPackages,
          [string]$Platform=$Global:Platform,
          [string]$Version=$Global:Version)

    Add-Type -assembly "System.IO.Compression.FileSystem"
    [IO.Compression.ZipFile]::CreateFromDirectory($ArtifactsBin, "$ArtifactsPackages\HFM $Platform $Version.zip") 
}

Function Build-Msi
{
    param([string]$Target='Rebuild',
          [string]$Configuration='Release',
          [string]$ArtifactsPackages=$Global:ArtifactsPackages,
          [string]$Platform=$Global:Platform,
          [string]$Version=$Global:Version)

    . $MSBuild $SetupSolutionFileName /t:$Target /p:Configuration=$Configuration
    Copy-Item -Path "HFM.Setup\bin\$Configuration\HFM.Setup.msi" -Destination "$ArtifactsPackages\HFM $Platform $Version.msi"
}

Function Configure-Artifacts
{
    param([string]$Path)

    $Global:ArtifactsPath = $Path
    $Global:ArtifactsBin = "$Path\HFM.NET"
    $Global:ArtifactsPackages = "$Path\Packages"
    Write-Host "ArtifactsPath: $Global:ArtifactsPath"
    Write-Host "ArtifactsBin: $Global:ArtifactsBin"
    Write-Host "ArtifactsPackages: $Global:ArtifactsPackages"
}

Function Configure-Version
{
    param([string]$Version)

    $Global:Version = $Version
    Write-Host "Version: $Version"
}

Function Configure-Platform
{
    param([string]$Platform)

    $Global:Platform = $Platform
    Write-Host "Platform: $Platform"
}

Configure-Artifacts -Path "$PSScriptRoot\Artifacts"
Configure-Version -Version '0.9.8.0'
Configure-Platform -Platform 'Any CPU'
