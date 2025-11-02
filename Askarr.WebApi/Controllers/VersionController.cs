using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Askarr.WebApi.Controllers
{
    [ApiController]
    [Route("/api/version")]
    public class VersionController : ControllerBase
    {
        private readonly ILogger<VersionController> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string GITHUB_REPO = "AmazingMoaaz/Askarr";
        
        public VersionController(ILogger<VersionController> logger)
        {
            _logger = logger;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Askarr/1.0");
        }

        [HttpGet]
        public async Task<IActionResult> GetVersion()
        {
            try
            {
                // Read local version
                string localVersion = "1.0.0";
                try
                {
                    var versionPath = Program.CombindPath("version.txt");
                    if (System.IO.File.Exists(versionPath))
                    {
                        localVersion = (await System.IO.File.ReadAllTextAsync(versionPath)).Trim();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not read version.txt, using default version");
                }

                // Check GitHub for latest release
                string latestVersion = localVersion;
                string downloadUrl = $"https://github.com/{GITHUB_REPO}/releases/latest";
                bool updateAvailable = false;
                
                try
                {
                    var response = await _httpClient.GetAsync($"https://api.github.com/repos/{GITHUB_REPO}/releases/latest");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);
                        
                        if (doc.RootElement.TryGetProperty("tag_name", out var tagName))
                        {
                            latestVersion = tagName.GetString()?.TrimStart('v') ?? localVersion;
                        }
                        
                        if (doc.RootElement.TryGetProperty("html_url", out var htmlUrl))
                        {
                            downloadUrl = htmlUrl.GetString() ?? downloadUrl;
                        }
                        
                        // Compare versions
                        updateAvailable = IsNewerVersion(latestVersion, localVersion);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not check for updates from GitHub");
                }

                return Ok(new
                {
                    currentVersion = localVersion,
                    latestVersion = latestVersion,
                    updateAvailable = updateAvailable,
                    downloadUrl = downloadUrl,
                    githubUrl = $"https://github.com/{GITHUB_REPO}",
                    releasesUrl = $"https://github.com/{GITHUB_REPO}/releases",
                    wikiUrl = $"https://github.com/{GITHUB_REPO}/wiki",
                    issuesUrl = $"https://github.com/{GITHUB_REPO}/issues"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version information");
                return Ok(new
                {
                    currentVersion = "1.0.0",
                    latestVersion = "1.0.0",
                    updateAvailable = false,
                    downloadUrl = $"https://github.com/{GITHUB_REPO}/releases/latest",
                    githubUrl = $"https://github.com/{GITHUB_REPO}",
                    releasesUrl = $"https://github.com/{GITHUB_REPO}/releases",
                    wikiUrl = $"https://github.com/{GITHUB_REPO}/wiki",
                    issuesUrl = $"https://github.com/{GITHUB_REPO}/issues"
                });
            }
        }

        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            try
            {
                // Remove 'v' prefix if present
                latestVersion = latestVersion.TrimStart('v');
                currentVersion = currentVersion.TrimStart('v');
                
                // Handle hotfix versions (e.g., 1.0.0-hf1)
                var latestParts = latestVersion.Split('-');
                var currentParts = currentVersion.Split('-');
                
                var latestVersionNumbers = latestParts[0].Split('.');
                var currentVersionNumbers = currentParts[0].Split('.');
                
                // Compare major, minor, patch
                for (int i = 0; i < Math.Min(latestVersionNumbers.Length, currentVersionNumbers.Length); i++)
                {
                    if (int.TryParse(latestVersionNumbers[i], out int latest) && 
                        int.TryParse(currentVersionNumbers[i], out int current))
                    {
                        if (latest > current) return true;
                        if (latest < current) return false;
                    }
                }
                
                // If base versions are equal, check for hotfix
                if (latestParts.Length > 1 && currentParts.Length > 1)
                {
                    // Both have hotfixes, compare them
                    var latestHotfix = latestParts[1].Replace("hf", "");
                    var currentHotfix = currentParts[1].Replace("hf", "");
                    
                    if (int.TryParse(latestHotfix, out int latestHf) && 
                        int.TryParse(currentHotfix, out int currentHf))
                    {
                        return latestHf > currentHf;
                    }
                }
                else if (latestParts.Length > 1 && currentParts.Length == 1)
                {
                    // Latest has hotfix, current doesn't - latest is newer
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

