$MSBuild = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
$SolutionFileName = 'HFM.All.sln'
$SetupSolutionFileName = 'HFM.Setup.sln'
$EntryProjectPath = ".\HFM\HFM.csproj"
$DotNetWindows = 'net6.0-windows'
$DotNet = 'net6.0'

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

Function Update-AssemblyVersionContent
{
    [CmdletBinding()]
    param([string]$Path, 
          [string]$AssemblyVersion=$Global:Version, 
          [string]$AssemblyFileVersion=$Global:Version)

    $content = Get-Content -Path $Path -ErrorAction Stop |
     %{ $_ -Replace 'AssemblyVersion(Attribute)?\(.*\)', "AssemblyVersion(`"$AssemblyVersion`")" } |
     %{ $_ -Replace 'AssemblyFileVersion(Attribute)?\(.*\)', "AssemblyFileVersion(`"$AssemblyFileVersion`")" }
     
     Set-Content -Path $Path -Value $content -Encoding UTF8 -Force -ErrorAction Stop
}

Function Update-AssemblyVersion
{
    [CmdletBinding()]
    param([string]$AssemblyVersion=$Global:Version, 
          [string]$AssemblyFileVersion=$Global:Version)

    Write-Host "---------------------------------------------------"
    Write-Host "Updating Assembly Version"
    Write-Host " AssemblyVersion: $AssemblyVersion"
    Write-Host " AssemblyFileVersion: $AssemblyFileVersion"
    Write-Host "---------------------------------------------------"

    Update-AssemblyVersionContent -Path 'ExeAssemblyVersion.cs' -AssemblyVersion $AssemblyVersion -AssemblyFileVersion $AssemblyFileVersion
    Update-AssemblyVersionContent -Path 'AssemblyVersion.cs' -AssemblyVersion '10.0.0.0' -AssemblyFileVersion $AssemblyFileVersion
}

Function Build-Solution
{
    param([string]$Configuration=$Global:Configuration,
          [string]$AssemblyVersion=$Global:Version,
          [string]$AssemblyFileVersion=$Global:Version)

    Update-AssemblyVersion -AssemblyVersion $AssemblyVersion -AssemblyFileVersion $AssemblyFileVersion

    Write-Host "---------------------------------------------------"
    Write-Host "Building Solution"
    Write-Host " Configuration: $Configuration"
    Write-Host "---------------------------------------------------"

    Exec { & dotnet restore $SolutionFileName --verbosity normal }
    Exec { & dotnet build $SolutionFileName -c $Configuration --verbosity normal --no-incremental --no-restore }
    Exec { & dotnet publish $EntryProjectPath -f $DotNetWindows -c $Configuration --verbosity normal --no-build }
}

Function Test-Build
{
    param([string]$Configuration=$Global:Configuration,
          [string]$ArtifactsPath=$Global:ArtifactsPath)

    Write-Host "---------------------------------------------------"
    Write-Host "Testing Build"
    Write-Host " Configuration: $Configuration"
    Write-Host " ArtifactsPath: $ArtifactsPath"
    Write-Host "---------------------------------------------------"
    
    Exec { & dotnet test .\HFM.Core.Tests\HFM.Core.Tests.csproj --no-build -c $Configuration -l "trx;LogFileName=HFM.Core.Tests.Results.trx" -r $ArtifactsPath }
    Exec { & dotnet test .\HFM.Forms.Tests\HFM.Forms.Tests.csproj --no-build -c $Configuration -l "trx;LogFileName=HFM.Forms.Tests.Results.trx" -r $ArtifactsPath }
    Exec { & dotnet test .\HFM.Preferences.Tests\HFM.Preferences.Tests.csproj --no-build -c $Configuration -l "trx;LogFileName=HFM.Preferences.Tests.Results.trx" -r $ArtifactsPath }
}

Function Clean-Artifacts
{
    [CmdletBinding()]
    param([string]$ArtifactsPath=$Global:ArtifactsPath,
          [string]$DotNetWindowsArtifactsBin=$Global:DotNetWindowsArtifactsBin,
          [string]$DotNetWindowsArtifactsPackages=$Global:DotNetWindowsArtifactsPackages)

    Write-Host "---------------------------------------------------"
    Write-Host "Cleaning Artifacts"
    Write-Host " ArtifactsPath: $ArtifactsPath"
    Write-Host " DotNetWindowsArtifactsBin: $DotNetWindowsArtifactsBin"
    Write-Host " DotNetWindowsArtifactsPackages: $DotNetWindowsArtifactsPackages"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    if (Test-Path $ArtifactsPath -PathType Container)
    {
        Remove-Item $ArtifactsPath -Recurse -Force -ErrorAction Stop -Verbose:$localVerbose
    }

    New-Item "$DotNetWindowsArtifactsBin\Tools" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$DotNetWindowsArtifactsBin\Documentation\License" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$DotNetWindowsArtifactsBin\CSS" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$DotNetWindowsArtifactsBin\XSL" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$DotNetWindowsArtifactsBin\runtimes\win-x86" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item "$DotNetWindowsArtifactsBin\runtimes\win-x64" -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
    New-Item $DotNetWindowsArtifactsPackages -ItemType Directory -ErrorAction Stop -Verbose:$localVerbose > $null
}

Function Deploy-Build
{
    [CmdletBinding()]
    param([string]$Configuration=$Global:Configuration,
          [string]$DotNetWindowsArtifactsBin=$Global:DotNetWindowsArtifactsBin)

    Write-Host "---------------------------------------------------"
    Write-Host "Deploying Build"
    Write-Host " Configuration: $Configuration"
    Write-Host " DotNetWindowsArtifactsBin: $DotNetWindowsArtifactsBin"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    # Primary Assemblies
    [string[]]$Assemblies = @(
        "AutoMapper.dll",
        "EntityFramework.dll",
        "EntityFramework.SqlServer.dll",
        "HFM.Client.dll",
        "HFM.Core.dll",
        "HFM.deps.json", 
        "HFM.dll", 
        "HFM.dll.config", 
        "HFM.exe", 
        "HFM.Forms.dll",
        "HFM.Log.dll",
        "HFM.Preferences.dll",
        "HFM.Proteins.dll",
        "HFM.runtimeconfig.json",
        "LightInject.dll",
        "LightInject.Microsoft.DependencyInjection.dll",
        "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
        "Newtonsoft.Json.dll",
        "protobuf-net.Core.dll",
        "protobuf-net.dll",
        "ZedGraph.dll"
        )

    $hfmBinRoot = "HFM\bin\$Configuration\$DotNetWindows\publish"

    $AssemblyFiles = Get-ChildItem -Path "$hfmBinRoot\*" -Include $Assemblies
    Copy-Item -Path $AssemblyFiles -Destination $DotNetWindowsArtifactsBin -ErrorAction Stop -Verbose:$localVerbose
    # SQLite Assemblies
    Copy-Item -Path "$hfmBinRoot\runtimes\win-x86\*" -Destination "$DotNetWindowsArtifactsBin\runtimes\win-x86" -Recurse -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path "$hfmBinRoot\runtimes\win-x64\*" -Destination "$DotNetWindowsArtifactsBin\runtimes\win-x64" -Recurse -ErrorAction Stop -Verbose:$localVerbose
    # Tools Assemblies
    #Copy-Item -Path 'HFM.Client.Tool\bin\ReleaseMerge\HFM.Client.exe' -Destination "$DotNetWindowsArtifactsBin\Tools" -ErrorAction Stop -Verbose:$localVerbose
    #Copy-Item -Path 'HFM.Log.Tool\bin\ReleaseMerge\HFM.Log.exe' -Destination "$DotNetWindowsArtifactsBin\Tools" -ErrorAction Stop -Verbose:$localVerbose
    # Documentation & Licenses
    Copy-Item -Path '..\doc\AutoMapper License.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\GPLv2.TXT' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\Json.NET License.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\LightInject License.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\protobuf-net Licence.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\protoc-license.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path '..\doc\ZedGraph License.txt' -Destination "$DotNetWindowsArtifactsBin\Documentation\License" -ErrorAction Stop -Verbose:$localVerbose
    # CSS & XSL
    Copy-Item -Path "$hfmBinRoot\CSS\*" -Destination "$DotNetWindowsArtifactsBin\CSS" -ErrorAction Stop -Verbose:$localVerbose
    Copy-Item -Path "$hfmBinRoot\XSL\*" -Destination "$DotNetWindowsArtifactsBin\XSL" -ErrorAction Stop -Verbose:$localVerbose
}

Function Build-Zip
{
    [CmdletBinding()]
    param([string]$Version=$Global:Version,
          [string]$DotNetWindowsArtifactsBin=$Global:DotNetWindowsArtifactsBin,
          [string]$DotNetWindowsArtifactsPackages=$Global:DotNetWindowsArtifactsPackages)

    Write-Host "---------------------------------------------------"
    Write-Host "Building Zip Package"
    Write-Host " Version: $Version"
    Write-Host " DotNetWindowsArtifactsBin: $DotNetWindowsArtifactsBin"
    Write-Host " DotNetWindowsArtifactsPackages: $DotNetWindowsArtifactsPackages"
    Write-Host "---------------------------------------------------"

    Add-Type -assembly "System.IO.Compression.FileSystem"
    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    $zipName = "$DotNetWindowsArtifactsPackages\HFM.$Version.zip"
    if ($localVerbose) {
        Write-Verbose "Packing as $zipName"
        Get-ChildItem $DotNetWindowsArtifactsBin -File -Recurse | Select FullName
    }
    [IO.Compression.ZipFile]::CreateFromDirectory($DotNetWindowsArtifactsBin, $zipName) 
}

Function Build-Msi
{
    [CmdletBinding()]
    param([string]$Configuration=$Global:Configuration,
          [string]$Version=$Global:Version,
          [string]$DotNetWindowsArtifactsPackages=$Global:DotNetWindowsArtifactsPackages)

    Write-Host "---------------------------------------------------"
    Write-Host "Building Msi Package"
    Write-Host " Configuration: $Configuration"
    Write-Host " Version: $Version"
    Write-Host " DotNetWindowsArtifactsPackages: $DotNetWindowsArtifactsPackages"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    Exec { & $MSBuild $SetupSolutionFileName /t:Rebuild /p:Configuration=$Configuration }
    Deploy-Msi -Configuration $Configuration -DotNetWindowsArtifactsPackages $DotNetWindowsArtifactsPackages -Version $Version -Verbose:$localVerbose
}

Function Deploy-Msi
{
    [CmdletBinding()]
    param([string]$Configuration=$Global:Configuration,
          [string]$Version=$Global:Version,
          [string]$DotNetWindowsArtifactsPackages=$Global:DotNetWindowsArtifactsPackages)

    Write-Host "---------------------------------------------------"
    Write-Host "Deploying Msi Package"
    Write-Host " Configuration: $Configuration"
    Write-Host " Version: $Version"
    Write-Host " DotNetWindowsArtifactsPackages: $DotNetWindowsArtifactsPackages"
    Write-Host "---------------------------------------------------"

    $localVerbose = $PSBoundParameters["Verbose"].IsPresent -eq $true

    Copy-Item -Path "HFM.Setup\bin\$Configuration\HFM.Setup.msi" -Destination "$DotNetWindowsArtifactsPackages\HFM.$Version.msi" -ErrorAction Stop -Verbose:$localVerbose
}

Function Configure-Artifacts
{
    param([string]$Path)

    $Global:ArtifactsPath = $Path
    $Global:DotNetWindowsArtifactsBin = "$Global:ArtifactsPath\$DotNetWindows\HFM.NET"
    $Global:DotNetWindowsArtifactsPackages = "$Global:ArtifactsPath\$DotNetWindows\Packages"

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Artifacts"
    Write-Host " ArtifactsPath: $Global:ArtifactsPath"
    Write-Host " DotNetWindowsArtifactsBin: $Global:DotNetWindowsArtifactsBin"
    Write-Host " DotNetWindowsArtifactsPackages: $Global:DotNetWindowsArtifactsPackages"
    Write-Host "---------------------------------------------------"
}

Function Configure-Version
{
    param([string]$Version = $Global:Version, [switch]$CommitCount)

    $Commits = '0'
    if ($CommitCount)
    {
        $Commits = $(git rev-list HEAD --count | Out-String).Trim()
    }

    $VersionSplit = $Version.Split('.')
    $Global:Version = [String]::Join('.', $VersionSplit[0], $VersionSplit[1], $Commits, '0')

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Version: $Global:Version"
    Write-Host "---------------------------------------------------"
}

Function Configure-Configuration
{
    param([string]$Configuration)

    $Global:Configuration = $Configuration

    Write-Host "---------------------------------------------------"
    Write-Host "Configuring Configuration: $Global:Configuration"
    Write-Host "---------------------------------------------------"
}

Configure-Version -Version '10.0'
Configure-Artifacts -Path "$PSScriptRoot\Artifacts"
Configure-Configuration -Configuration 'Release'
