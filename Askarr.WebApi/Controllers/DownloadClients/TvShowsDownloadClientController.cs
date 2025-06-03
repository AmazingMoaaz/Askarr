using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Askarr.WebApi.config;
using Askarr.WebApi.Controllers.DownloadClients.Ombi;
using Askarr.WebApi.Controllers.DownloadClients.Overseerr;
using Askarr.WebApi.Controllers.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.Locale;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.Music;
using Askarr.WebApi.AskarrBot.TvShows;
using SonarrSettingsCategory = Askarr.WebApi.Controllers.DownloadClients.Sonarr.SonarrSettingsCategory;
using BotSonarrCategory = Askarr.WebApi.AskarrBot.DownloadClients.Sonarr.SonarrCategory;
using BotLidarrCategory = Askarr.WebApi.AskarrBot.DownloadClients.Lidarr.LidarrCategory;
using BotRadarrCategory = Askarr.WebApi.AskarrBot.DownloadClients.Radarr.RadarrCategory;
using BotOverseerrMovieCategory = Askarr.WebApi.AskarrBot.DownloadClients.Overseerr.OverseerrMovieCategory;

namespace Askarr.WebApi.Controllers.DownloadClients
{
    [ApiController]
    [Authorize]
    [Route("/api/tvshows")]
    public class TvShowsDownloadClientController : ControllerBase
    {
        private readonly MoviesSettings _moviesSettings;
        private readonly TvShowsSettings _tvShowsSettings;
        private readonly MusicSettings _musicSettings;
        private readonly DownloadClientsSettings _downloadClientsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public TvShowsDownloadClientController(
            IHttpClientFactory httpClientFactory,
            MoviesSettingsProvider moviesSettingsProvider,
            TvShowsSettingsProvider tvShowsSettingsProvider,
            MusicSettingsProvider musicSettingsProvider,
            DownloadClientsSettingsProvider downloadClientsSettingsProvider)
        {
            _moviesSettings = moviesSettingsProvider.Provide();
            _tvShowsSettings = tvShowsSettingsProvider.Provide();
            _musicSettings = musicSettingsProvider.Provide();
            _downloadClientsSettings = downloadClientsSettingsProvider.Provide();
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            List<string> otherCategories = new List<string>();
            switch (_moviesSettings.Client)
            {
                case "Radarr":
                    // Using the string array from the config settings temporarily
                    foreach (string category in _downloadClientsSettings.Radarr.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    break;
                case "Overseerr":
                    // Using the string array from the config settings temporarily
                    foreach (string category in _downloadClientsSettings.Overseerr.Movies.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    if(otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
            }

            switch (_musicSettings.Client)
            {
                case "Lidarr":
                    // Using the string array from the config settings temporarily
                    foreach (string category in _downloadClientsSettings.Lidarr.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    break;
            }

            // Build the response model using our config settings
            return Ok(new TvShowsSettingsModel
            {
                Client = _tvShowsSettings.Client,
                Sonarr = new SonarrSettingsModel
                {
                    Hostname = _downloadClientsSettings.Sonarr.Hostname,
                    BaseUrl = _downloadClientsSettings.Sonarr.BaseUrl,
                    Port = _downloadClientsSettings.Sonarr.Port,
                    ApiKey = _downloadClientsSettings.Sonarr.ApiKey,
                    Categories = _downloadClientsSettings.Sonarr.Categories
                        .Select(x => new SonarrSettingsCategory { Name = x })
                        .ToArray(),
                    UseSSL = _downloadClientsSettings.Sonarr.UseSSL,
                    SearchNewRequests = _downloadClientsSettings.Sonarr.SearchNewRequests,
                    MonitorNewRequests = _downloadClientsSettings.Sonarr.MonitorNewRequests,
                    Version = _downloadClientsSettings.Sonarr.Version
                },
                Ombi = new OmbiTVSettings
                {
                    Hostname = _downloadClientsSettings.Ombi.Hostname,
                    BaseUrl = _downloadClientsSettings.Ombi.BaseUrl,
                    Port = _downloadClientsSettings.Ombi.Port,
                    ApiKey = _downloadClientsSettings.Ombi.ApiKey,
                    ApiUsername = _downloadClientsSettings.Ombi.ApiUsername,
                    UseSSL = _downloadClientsSettings.Ombi.UseSSL,
                    Version = _downloadClientsSettings.Ombi.Version,
                    UseTVIssue = _downloadClientsSettings.Ombi.UseTVIssue
                },
                Overseerr = _downloadClientsSettings.Overseerr.ToOverseerrBotSettings(),
                Restrictions = _tvShowsSettings.Restrictions,
                OtherCategories = otherCategories.ToArray()
            });
        }

        [HttpPost("disable")]
        public async Task<IActionResult> SaveAsync()
        {
            _tvShowsSettings.Client = DownloadClient.Disabled;
            DownloadClientsSettingsRepository.SetDisabledClient(_tvShowsSettings);
            return Ok(new { ok = true });
        }
    }
}
