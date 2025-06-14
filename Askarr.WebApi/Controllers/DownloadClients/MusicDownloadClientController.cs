﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  Askarr.WebApi.config;
using  Askarr.WebApi.Controllers.DownloadClients.Lidarr;
using  Askarr.WebApi. AskarrBot.DownloadClients;
using  Askarr.WebApi. AskarrBot.DownloadClients.Radarr;
using  Askarr.WebApi. AskarrBot.DownloadClients.Sonarr;
using  Askarr.WebApi. AskarrBot.Locale;
using  Askarr.WebApi. AskarrBot.Movies;
using  Askarr.WebApi. AskarrBot.Music;
using  Askarr.WebApi. AskarrBot.TvShows;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace  Askarr.WebApi.Controllers.DownloadClients
{
    [ApiController]
    [Authorize]
    [Route("/api/music")]
    public class MusicDownloadClientController : ControllerBase
    {
        private readonly MusicSettings _musicSettings;
        private readonly MoviesSettings _moviesSettings;
        private readonly TvShowsSettings _tvShowsSettings;
        private readonly DownloadClientsSettings _downloadClientsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MusicDownloadClientController(
            IHttpClientFactory httpClientFactory,
            MusicSettingsProvider musicSettingsProvider,
            MoviesSettingsProvider moviesSettingsProvider,
            TvShowsSettingsProvider tvShowsSettingsProvider,
            DownloadClientsSettingsProvider downloadClientsSettingsProvider )
        {
            _httpClientFactory = httpClientFactory;
            _musicSettings = musicSettingsProvider.Provide();
            _moviesSettings = moviesSettingsProvider.Provide();
            _tvShowsSettings = tvShowsSettingsProvider.Provide();
            _downloadClientsSettings = downloadClientsSettingsProvider.Provide();
        }


        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            List<string> otherCategories = new List<string>();
            switch (_moviesSettings.Client)
            {
                case "Radarr":
                    foreach (string category in _downloadClientsSettings.Radarr.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    break;
                case "Overseerr":
                    foreach (string category in _downloadClientsSettings.Overseerr.Movies.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    if (otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
            }

            switch (_tvShowsSettings.Client)
            {
                case "Sonarr":
                    foreach (string category in _downloadClientsSettings.Sonarr.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    break;
                case "Overseerr":
                    foreach (string category in _downloadClientsSettings.Overseerr.TvShows.Categories)
                    {
                        otherCategories.Add(category.ToLower());
                    }
                    if (otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
            }

            return Ok(new MusicSettingsModel
            {
                Client = _musicSettings.Client,
                Lidarr = new LidarrSettingsModel
                {
                    Hostname = _downloadClientsSettings.Lidarr.Hostname,
                    BaseUrl = _downloadClientsSettings.Lidarr.BaseUrl,
                    Port = _downloadClientsSettings.Lidarr.Port,
                    ApiKey = _downloadClientsSettings.Lidarr.ApiKey,
                    Categories = _downloadClientsSettings.Lidarr.Categories
                        .Select(x => new LidarrSettingsCategory { Name = x })
                        .ToArray(),
                    UseSSL = _downloadClientsSettings.Lidarr.UseSSL,
                    SearchNewRequests = _downloadClientsSettings.Lidarr.SearchNewRequests,
                    MonitorNewRequests = _downloadClientsSettings.Lidarr.MonitorNewRequests,
                    Version = _downloadClientsSettings.Lidarr.Version
                },
                OtherCategories = otherCategories.ToArray()
            });
        }


        [HttpPost("disable")]
        public async Task<IActionResult> SaveAsync()
        {
            _musicSettings.Client = DownloadClient.Disabled;
            DownloadClientsSettingsRepository.SetDisabledClient(_musicSettings);
            return Ok(new { ok = true });
        }
    }
}
