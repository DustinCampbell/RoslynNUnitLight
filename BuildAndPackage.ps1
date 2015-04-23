$CurrentPath = $(Get-Location)
$BinariesPath = Join-Path $CurrentPath "Binaries\Release"
$PackagePath = Join-Path $BinariesPath "Package"
$LibPath = Join-Path $PackagePath "lib"
$PortableLibPath = Join-Path $LibPath "portable-net45+win8"
$NuGet = Join-Path $CurrentPath "Tools\NuGet\NuGet.exe"
$Solution = Join-Path $CurrentPath "RoslynNUnitLight.sln"
$NuSpec = Join-Path $CurrentPath "RoslynNUnitLight.nuspec"
$DllName = "RoslynNUnitLight.dll"
$DllPath = Join-Path $BinariesPath "RoslynNUnitLight"
$DllPath = Join-Path $DllPath $DllName

# Perform NuGet package restore
Invoke-Expression "$NuGet restore $Solution"
""

# Build solution with MSBuild in Release
Invoke-Expression "msbuild.exe /m $Solution /p:Configuration=Release"
""

# Create the package path. If it already exists, delete the old package path first.
"Preparing files for packaging..."

if ((Test-Path $PackagePath) -eq $True)
{
    Remove-Item -Recurse -Force $PackagePath
}

New-Item $PackagePath -Type Directory | Out-Null

"  * Created `"Package`" directory"

New-Item $LibPath -Type Directory | Out-Null

"  * Created `"lib`" directory"

New-Item $PortableLibPath -Type Directory | Out-Null

"  * Created `"lib\portable-net45+win8`" directory"

Copy-Item -Path $DllPath -Destination (Join-Path $PortableLibPath $DllName)

"  * Copied `"$DllName`" to `"lib`" directory"
""

# Package NuGet
Invoke-Expression "$NuGet pack $NuSpec -BasePath $PackagePath -OutputDirectory $PackagePath"