using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Askarr.WebApi.config;
using Askarr.WebApi.Controllers.DownloadClients.Ombi;
using Askarr.WebApi.Controllers.DownloadClients.Overseerr;
using Askarr.WebApi.Controllers.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.Locale;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.Music;
using Askarr.WebApi.AskarrBot.TvShows;
using RadarrSettingsCategory = Askarr.WebApi.Controllers.DownloadClients.Radarr.RadarrSettingsCategory;

namespace Askarr.WebApi.Controllers.DownloadClients
{
    [ApiController]
    [Authorize]
    [Route("/api/movies")]
    public class MovieDownloadClientController : ControllerBase
    {
        private readonly MoviesSettings _moviesSettings;
        private readonly TvShowsSettings _tvShowsSettings;
        private readonly MusicSettings _musicSettings;
        private readonly DownloadClientsSettings _downloadClientsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieDownloadClientController(
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
            switch(_tvShowsSettings.Client)
            {
                case "Sonarr":
                    foreach(SonarrCategory category in _downloadClientsSettings.Sonarr.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    break;
                case "Overseerr":
                    foreach (AskarrBot.DownloadClients.Overseerr.OverseerrTvShowCategory category in _downloadClientsSettings.Overseerr.TvShows.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    if(otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
            }

            switch(_musicSettings.Client)
            {
                case "Lidarr":
                    foreach (LidarrCategory category in _downloadClientsSettings.Lidarr.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    break;
            }


            return Ok(new MovieSettingsModel
            {
                Client = _moviesSettings.Client,
                Radarr = new RadarrSettingsModel
                {
                    Hostname = _downloadClientsSettings.Radarr.Hostname,
                    BaseUrl = _downloadClientsSettings.Radarr.BaseUrl,
                    Port = _downloadClientsSettings.Radarr.Port,
                    ApiKey = _downloadClientsSettings.Radarr.ApiKey,
                    Categories = _downloadClientsSettings.Radarr.Categories.Select(x => new RadarrSettingsCategory
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MinimumAvailability = x.MinimumAvailability,
                        ProfileId = x.ProfileId,
                        RootFolder = x.RootFolder,
                        Tags = x.Tags
                    }).ToArray(),
                    UseSSL = _downloadClientsSettings.Radarr.UseSSL,
                    SearchNewRequests = _downloadClientsSettings.Radarr.SearchNewRequests,
                    MonitorNewRequests = _downloadClientsSettings.Radarr.MonitorNewRequests,
                    Version = _downloadClientsSettings.Radarr.Version
                },
                Ombi = new OmbiMovieSettings
                {
                    Hostname = _downloadClientsSettings.Ombi.Hostname,
                    BaseUrl = _downloadClientsSettings.Ombi.BaseUrl,
                    Port = _downloadClientsSettings.Ombi.Port,
                    ApiKey = _downloadClientsSettings.Ombi.ApiKey,
                    ApiUsername = _downloadClientsSettings.Ombi.ApiUsername,
                    UseSSL = _downloadClientsSettings.Ombi.UseSSL,
                    Version = _downloadClientsSettings.Ombi.Version,
                    UseMovieIssue = _downloadClientsSettings.Ombi.UseMovieIssue
                },
                Overseerr = _downloadClientsSettings.Overseerr,
                OtherCategories = otherCategories.ToArray()
            });
        }

        [HttpPost("disable")]
        public async Task<IActionResult> SaveAsync()
        {
            _moviesSettings.Client = DownloadClient.Disabled;
            DownloadClientsSettingsRepository.SetDisabledClient(_moviesSettings);
            return Ok(new { ok = true });
        }
    }
}
