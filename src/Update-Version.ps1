$versionFile = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "Version.cs";

$ErrorActionPreference = "Stop";

function validateLastExitCode()
{
	$updateStatus = $LASTEXITCODE
    Write-Debug "Cetrea_GitRevision.updateStatus: $updateStatus";
    if ($updateStatus -ne 0)
    {
        throw "Unable to get git revision. Last exit code: $updateStatus";
    }
}

git fetch --tags origin;
validateLastExitCode;

$gitDescribe = git describe --always;
validateLastExitCode;

$commitsSinceLastTag = 0;
if ($gitDescribe -match ".*-(?<commits>\d+)-.*")
{
    $commitsSinceLastTagStr = $Matches["commits"];
    Write-Debug "commitsSinceLastTagStr: $commitsSinceLastTagStr"

    $commitsSinceLastTag = [int]::Parse("0$commitsSinceLastTagStr");
}
else
{
    $currentBranch = git rev-parse --abbrev-ref HEAD;

    $commitsSinceLastTagStr = git rev-list origin/master..$currentBranch --count;
    Write-Debug "commitsSinceLastTagStr: $commitsSinceLastTagStr"

    $commitsSinceLastTag = [int]::Parse("0$commitsSinceLastTagStr");
}

Write-Debug "commitsSinceLastTag: $commitsSinceLastTag"

$versionContent = Get-Content $versionFile | Out-String;
Write-Debug "versionContent: $versionContent";

$updatedVersionContent = $versionContent -replace "\.\d+`"",".$commitsSinceLastTag`"";
Write-Debug "updatedVersionContent: $updatedVersionContent";

Set-Content -Value $updatedVersionContent -Path $versionFile;