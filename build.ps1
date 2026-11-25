[CmdletBinding()]
param(
    [string]$DependencyDir = (Join-Path $PSScriptRoot 'vendor\managed'),
    [string]$RuntimeDir,
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

$bundle = Join-Path $PSScriptRoot 'vendor\runtime.zip'
if ($RuntimeDir) {
    $buildDir = New-Item -Type Directory -Force (Join-Path $PSScriptRoot 'artifacts')
    $bundle = Join-Path $buildDir.FullName 'runtime-custom.zip'
    $names = 'gtav_aes_key.dat','gtav_ng_key.dat','gtav_hash_lut.dat','gtav_ng_decrypt_tables.dat',
             'gtav_ng_encrypt_tables.dat','gtav_ng_encrypt_luts.dat','embedded_timer.awc','embedded_cut_iaaj.ypt',
             'minimal_ydr.stub','minimal_yft.stub','minimal_ybn.stub'
    $paths = foreach ($name in $names) { (Get-Item -LiteralPath (Join-Path $RuntimeDir $name)).FullName }
    Compress-Archive -LiteralPath $paths -DestinationPath $bundle -Force
}
& (Join-Path $framework 'MSBuild.exe') (Join-Path $PSScriptRoot 'ShrinkerV.sln') /nologo /v:minimal /p:Configuration=Release /p:Platform=x64 "/p:FrameworkPathOverride=$framework" "/p:ShrinkerVDependencyDir=$managed" "/p:RuntimeBundle=$bundle"
if ($LASTEXITCODE) { throw "Build failed ($LASTEXITCODE)" }

$out = Join-Path $PSScriptRoot 'artifacts\Release'
if ($Test) {
    $previous = $env:SHRINKER_V_TEST_INPUT
    try {
        if ($FixtureDir) { $env:SHRINKER_V_TEST_INPUT = (Resolve-Path -LiteralPath $FixtureDir).Path }
        & (Join-Path $out 'ShrinkerV.Tests.exe')
        if ($LASTEXITCODE) { throw "Tests failed ($LASTEXITCODE)" }
    }
    finally { $env:SHRINKER_V_TEST_INPUT = $previous }
}
