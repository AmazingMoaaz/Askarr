param (
    [Parameter(Mandatory=$true)]
    [ValidateSet("major", "minor", "patch", "hotfix")]
    [string]$VersionType
)

# Read current version
$currentVersion = Get-Content -Path "version.txt" -Raw
$currentVersion = $currentVersion.Trim()

# Parse version components
$versionParts = $currentVersion -split '\.'
$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

# Handle version types
switch ($VersionType) {
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "patch" {
        $patch++
    }
    "hotfix" {
        # For hotfixes, we'll add a -hf suffix if it doesn't exist, or increment the number if it does
        if ($patch -match "-hf(\d+)$") {
            $hfNumber = [int]$Matches[1]
            $hfNumber++
            $patch = $patch -replace "-hf\d+$", "-hf$hfNumber"
        } else {
            $patch = "$patch-hf1"
        }
    }
}

# Create new version string
$newVersion = "$major.$minor.$patch"

# Update version.txt
Set-Content -Path "version.txt" -Value $newVersion

# Output results
Write-Host "Version bumped from $currentVersion to $newVersion"
Write-Host "To release this version, run:"
Write-Host "git add version.txt"
Write-Host "git commit -m 'Bump version to $newVersion'"
Write-Host "git push origin main"
Write-Host "git tag v$newVersion"
Write-Host "git push origin v$newVersion" 