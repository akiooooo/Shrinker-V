[CmdletBinding()]
param(
    [string]$DependencyDir = (Join-Path $PSScriptRoot 'local\managed'),
    [string]$RuntimeDir = (Join-Path $PSScriptRoot 'local\runtime'),
    [switch]$Test,
    [string]$FixtureDir
)

$ErrorActionPreference = 'Stop'
$framework = Join-Path $env:windir 'Microsoft.NET\Framework64\v4.0.30319'
$managed = (Resolve-Path -LiteralPath $DependencyDir).Path
foreach ($name in 'CodeWalker.Core.dll', 'SharpDX.dll', 'SharpDX.Mathematics.dll') {
    if (!(Test-Path -LiteralPath (Join-Path $managed $name) -PathType Leaf)) {
        throw "Missing dependency in $managed - $name"
    }
}

& (Join-Path $framework 'MSBuild.exe') (Join-Path $PSScriptRoot 'WakaShrinker.sln') /nologo /v:minimal /p:Configuration=Release /p:Platform=x64 "/p:FrameworkPathOverride=$framework" "/p:WakaDependencyDir=$managed"
if ($LASTEXITCODE) { throw "Build failed ($LASTEXITCODE)" }

$out = Join-Path $PSScriptRoot 'artifacts\Release'
if (Test-Path -LiteralPath $RuntimeDir -PathType Container) {
    $target = New-Item -ItemType Directory -Force (Join-Path $out 'runtime')
    $files = 'gtav_aes_key.dat','gtav_ng_key.dat','gtav_ng_decrypt_tables.dat','gtav_ng_encrypt_tables.dat',
             'gtav_ng_encrypt_luts.dat','gtav_hash_lut.dat','embedded_timer.awc','embedded_cut_iaaj.ypt',
             'minimal_ydr.stub','minimal_yft.stub','minimal_ybn.stub'
    foreach ($name in $files) {
        $source = Join-Path $RuntimeDir $name
        if (Test-Path -LiteralPath $source -PathType Leaf) { Copy-Item -LiteralPath $source -Destination $target.FullName }
    }
}
if ($Test) {
    $previous = $env:WAKA_TEST_INPUT
    try {
        if ($FixtureDir) { $env:WAKA_TEST_INPUT = (Resolve-Path -LiteralPath $FixtureDir).Path }
        & (Join-Path $out 'Waka.Tests.exe')
        if ($LASTEXITCODE) { throw "Tests failed ($LASTEXITCODE)" }
    }
    finally { $env:WAKA_TEST_INPUT = $previous }
}
