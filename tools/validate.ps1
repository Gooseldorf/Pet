[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Context")]
    [string]$Mode
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$artifactDirectory = Join-Path $repositoryRoot ".artifacts/validation/$Mode"
$summaryPath = Join-Path $artifactDirectory "summary.txt"

New-Item -ItemType Directory -Force -Path $artifactDirectory | Out-Null

function Test-RepositoryPath {
    param(
        [string]$Path,
        [string]$Source
    )

    if ($Path -match "^(https?:|#|mailto:)") {
        return
    }

    $normalizedPath = $Path.Split("#")[0].Replace("/", [IO.Path]::DirectorySeparatorChar)
    $candidate = if ([IO.Path]::IsPathRooted($normalizedPath)) {
        $normalizedPath
    }
    else {
        Join-Path $repositoryRoot $normalizedPath
    }

    if (-not (Test-Path -LiteralPath $candidate)) {
        $script:failures.Add("$Source -> $Path")
    }
}

function Test-MarkdownLinks {
    param([IO.FileInfo]$File)

    $content = Get-Content -LiteralPath $File.FullName -Raw
    foreach ($match in [regex]::Matches($content, "(?m)\[[^\]]+\]\(([^)\s]+)(?:\s+[^)]*)?\)")) {
        $target = $match.Groups[1].Value
        if ($target -notmatch "^(https?:|#|mailto:)") {
            $candidate = Join-Path $File.DirectoryName $target.Split("#")[0]
            if (-not (Test-Path -LiteralPath $candidate)) {
                $script:failures.Add("$($File.FullName.Substring($repositoryRoot.Length + 1)) -> $target")
            }
        }
    }
}

function Test-ContextPaths {
    param([IO.FileInfo]$File)

    $content = Get-Content -LiteralPath $File.FullName -Raw
    $pattern = '(?<![A-Za-z0-9_.-])(?:AGENTS\.md|THIRD-PARTY-ASSETS\.md|docs/[A-Za-z0-9_./-]+|tools/[A-Za-z0-9_./-]+|Assets/[A-Za-z0-9_./-]+|Packages/[A-Za-z0-9_./-]+|ProjectSettings/[A-Za-z0-9_./-]+|\.github/[A-Za-z0-9_./-]+|\.opencode/[A-Za-z0-9_./-]+)'
    foreach ($match in [regex]::Matches($content, $pattern)) {
        Test-RepositoryPath -Path $match.Value.TrimEnd([char[]]@('.', ',', ';', ':', ')', ']')) -Source $File.FullName.Substring($repositoryRoot.Length + 1)
    }
}

function Test-SkillPackages {
    $skillsRoot = Join-Path $repositoryRoot ".opencode/skills"
    foreach ($directory in Get-ChildItem -LiteralPath $skillsRoot -Directory) {
        $skillPath = Join-Path $directory.FullName "SKILL.md"
        if (-not (Test-Path -LiteralPath $skillPath)) {
            $script:failures.Add("$($directory.FullName.Substring($repositoryRoot.Length + 1)) is missing SKILL.md")
            continue
        }

        $content = Get-Content -LiteralPath $skillPath -Raw
        if ($content -notmatch '(?s)^---\s*\r?\nname:\s*([a-z0-9-]+)\s*\r?\ndescription:\s*(\S.+?)\s*\r?\n---') {
            $script:failures.Add("$($skillPath.Substring($repositoryRoot.Length + 1)) has invalid required frontmatter")
            continue
        }

        if ($Matches[1] -ne $directory.Name) {
            $script:failures.Add("$($skillPath.Substring($repositoryRoot.Length + 1)) name '$($Matches[1])' does not match '$($directory.Name)'")
        }

        if ([string]::IsNullOrWhiteSpace($Matches[2])) {
            $script:failures.Add("$($skillPath.Substring($repositoryRoot.Length + 1)) has an empty description")
        }
    }
}

$failures = [System.Collections.Generic.List[string]]::new()

try {
    $markdownFiles = @(
        Get-Item -LiteralPath (Join-Path $repositoryRoot "AGENTS.md")
        Get-ChildItem -LiteralPath (Join-Path $repositoryRoot "docs") -Recurse -File -Filter "*.md"
        Get-ChildItem -LiteralPath (Join-Path $repositoryRoot ".opencode/skills") -Recurse -File -Filter "*.md"
    )
    foreach ($file in $markdownFiles) {
        Test-MarkdownLinks -File $file
        Test-ContextPaths -File $file
    }

    Test-SkillPackages

    if ($failures.Count -gt 0) {
        $summary = @("Context validation failed.", "") + $failures
        Set-Content -LiteralPath $summaryPath -Value $summary
        $summary | ForEach-Object { Write-Error $_ }
        exit 1
    }

    $summary = "Context validation passed."
    Set-Content -LiteralPath $summaryPath -Value $summary
    Write-Output $summary
}
catch {
    $summary = "Context validation failed: $($_.Exception.Message)`n$($_.ScriptStackTrace)"
    Set-Content -LiteralPath $summaryPath -Value $summary
    Write-Error $summary
    exit 1
}
