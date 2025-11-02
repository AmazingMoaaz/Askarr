#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Bump version for Askarr project
.DESCRIPTION
    This script bumps the version number in version.txt files and creates a git tag.
    Usage: .\bump-version.ps1 [major|minor|patch|hotfix]
.EXAMPLE
    .\bump-version.ps1 patch
    Bumps from 2.0.0 to 2.0.1
.EXAMPLE
    .\bump-version.ps1 minor
    Bumps from 2.0.0 to 2.1.0
.EXAMPLE
    .\bump-version.ps1 major
    Bumps from 2.0.0 to 3.0.0
.EXAMPLE
    .\bump-version.ps1 hotfix
    Bumps from 2.0.0 to 2.0.0-hf1 (or increments hotfix number)
#>

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('major', 'minor', 'patch', 'hotfix')]
    [string]$BumpType
)

# Function to read version from file
function Get-Version {
    param([string]$FilePath)
    return (Get-Content $FilePath -Raw).Trim()
}

# Function to write version to file
function Set-Version {
    param(
        [string]$FilePath,
        [string]$Version
    )
    Set-Content -Path $FilePath -Value $Version -NoNewline
}

# Function to parse version
function Parse-Version {
    param([string]$Version)
    
    $parts = $Version -split '-'
    $versionPart = $parts[0]
    $hotfixPart = if ($parts.Length -gt 1) { $parts[1] } else { $null }
    
    $versionNumbers = $versionPart -split '\.'
    $major = [int]$versionNumbers[0]
    $minor = [int]$versionNumbers[1]
    $patch = [int]$versionNumbers[2]
    
    $hotfixNumber = 0
    if ($hotfixPart -and $hotfixPart -match 'hf(\d+)') {
        $hotfixNumber = [int]$matches[1]
    }
    
    return @{
        Major = $major
        Minor = $minor
        Patch = $patch
        Hotfix = $hotfixNumber
    }
}

# Function to bump version
function Get-NewVersion {
    param(
        [hashtable]$CurrentVersion,
        [string]$BumpType
    )
    
    switch ($BumpType) {
        'major' {
            return "$($CurrentVersion.Major + 1).0.0"
        }
        'minor' {
            return "$($CurrentVersion.Major).$($CurrentVersion.Minor + 1).0"
        }
        'patch' {
            return "$($CurrentVersion.Major).$($CurrentVersion.Minor).$($CurrentVersion.Patch + 1)"
        }
        'hotfix' {
            $base = "$($CurrentVersion.Major).$($CurrentVersion.Minor).$($CurrentVersion.Patch)"
            $newHotfix = $CurrentVersion.Hotfix + 1
            return "$base-hf$newHotfix"
        }
    }
}

# Main script
try {
    Write-Host "🚀 Askarr Version Bump Script" -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Check if we're in a git repository
    $gitStatus = git status 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Error: Not in a git repository" -ForegroundColor Red
        exit 1
    }
    
    # Check for uncommitted changes
    $changes = git status --porcelain
    if ($changes) {
        Write-Host "⚠️  Warning: You have uncommitted changes:" -ForegroundColor Yellow
        Write-Host $changes
        Write-Host ""
        $response = Read-Host "Do you want to continue? (y/N)"
        if ($response -ne 'y' -and $response -ne 'Y') {
            Write-Host "Aborted." -ForegroundColor Yellow
            exit 0
        }
    }
    
    # Read current version from root version.txt
    $versionFile = "version.txt"
    $webApiVersionFile = "Askarr.WebApi/version.txt"
    
    if (-not (Test-Path $versionFile)) {
        Write-Host "❌ Error: version.txt not found" -ForegroundColor Red
        exit 1
    }
    
    $currentVersionString = Get-Version $versionFile
    Write-Host "📋 Current version: $currentVersionString" -ForegroundColor White
    
    # Parse and bump version
    $currentVersion = Parse-Version $currentVersionString
    $newVersionString = Get-NewVersion $currentVersion $BumpType
    
    Write-Host "📈 New version: $newVersionString" -ForegroundColor Green
    Write-Host ""
    
    # Confirm
    $response = Read-Host "Proceed with version bump? (Y/n)"
    if ($response -eq 'n' -or $response -eq 'N') {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
    
    # Update version files
    Write-Host "📝 Updating version files..." -ForegroundColor Cyan
    Set-Version $versionFile $newVersionString
    
    if (Test-Path $webApiVersionFile) {
        Set-Version $webApiVersionFile $newVersionString
    }
    
    Write-Host "✅ Version files updated" -ForegroundColor Green
    
    # Git operations
    Write-Host ""
    Write-Host "📦 Creating git commit and tag..." -ForegroundColor Cyan
    
    git add $versionFile
    if (Test-Path $webApiVersionFile) {
        git add $webApiVersionFile
    }
    
    git commit -m "chore: bump version to $newVersionString"
    git tag -a "v$newVersionString" -m "Release version $newVersionString"
    
    Write-Host "✅ Git commit and tag created" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎉 Version bump complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Review the changes with: git show" -ForegroundColor White
    Write-Host "2. Push the commit: git push origin main" -ForegroundColor White
    Write-Host "3. Push the tag: git push origin v$newVersionString" -ForegroundColor White
    Write-Host ""
    Write-Host "Or push both at once:" -ForegroundColor Cyan
    Write-Host "git push origin main --tags" -ForegroundColor White
    Write-Host ""
    Write-Host "This will trigger the GitHub Actions workflow to:" -ForegroundColor Yellow
    Write-Host "  • Build and publish Docker images" -ForegroundColor Yellow
    Write-Host "  • Create a GitHub release" -ForegroundColor Yellow
    Write-Host "  • Generate release artifacts" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit 1
}

