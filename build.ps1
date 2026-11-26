param([switch]$Test, [string]$FixtureDir, [string]$CompilerDir)
$ErrorActionPreference = 'Stop'
$framework = Join-Path $env:windir 'Microsoft.NET\Framework64\v4.0.30319'
if (!$CompilerDir) { $CompilerDir = Join-Path $PSScriptRoot 'tools\Microsoft.Net.Compilers.Toolset\tasks\net472' }
$CompilerDir = (Resolve-Path -LiteralPath $CompilerDir).Path
$compiler = Join-Path $CompilerDir 'csc.exe'
$exe = Join-Path $PSScriptRoot 'dist\Shrinker V by akiooo.exe'
& "$framework\MSBuild.exe" "$PSScriptRoot\ShrinkerV.csproj" /nologo /verbosity:minimal /p:Configuration=Release "/p:FrameworkPathOverride=$framework" "/p:CscToolPath=$CompilerDir"
if ($LASTEXITCODE) { throw 'C# build failed.' }
if ($Test) {
    $testDir = [IO.Directory]::CreateDirectory("$PSScriptRoot\obj\tests").FullName
    Copy-Item -LiteralPath $exe -Destination $testDir -Force
    & $compiler /nologo /platform:x64 /optimize+ "/out:$testDir\ShrinkerV.Tests.exe" "/r:$exe" "$PSScriptRoot\tests\StudioTests.cs"
    if ($LASTEXITCODE) { throw 'Test build failed.' }
    $previous = $env:SHRINKER_V_TEST_INPUT
    try {
        if ($FixtureDir) { $env:SHRINKER_V_TEST_INPUT = (Resolve-Path -LiteralPath $FixtureDir).Path }
        & "$testDir\ShrinkerV.Tests.exe"
        if ($LASTEXITCODE) { throw 'Tests failed.' }
    } finally { $env:SHRINKER_V_TEST_INPUT = $previous }
}
