﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.TvShows;

namespace Askarr.WebApi.Controllers.DownloadClients.Ombi
{

    [ApiController]
    [Authorize]
    public class OmbiClientController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OmbiClient> _logger;

        public OmbiClientController(
            IHttpClientFactory httpClientFactory,
            ILogger<OmbiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("/api/movies/ombi/test")]
        [HttpPost("/api/tvshows/ombi/test")]
        public async Task<IActionResult> TestOmbiSettings([FromBody]TestOmbiSettingsModel model)
        {
            try
            {
                await OmbiClient.TestConnectionAsync(_httpClientFactory.CreateClient(), _logger, new AskarrBot.DownloadClients.Ombi.OmbiSettings
                {
                    ApiKey = model.ApiKey.Trim(),
                    Hostname = model.Hostname.Trim(),
                    Port = model.Port,
                    BaseUrl = model.BaseUrl.Trim(),
                    UseSSL = model.UseSSL,
                    Version = model.Version,
                });

                return Ok(new { ok = true });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/movies/ombi")]
        public async Task<IActionResult> SaveMoviesAsync([FromBody]SaveOmbiMoviesSettingsModel model)
        {
            var movieSettings = new MoviesSettings
            {
                Client = DownloadClient.Ombi
            };

            var ombiSettings = Sanitize(model);

            DownloadClientsSettingsRepository.SetOmbi(movieSettings, ombiSettings);

            return Ok(new { ok = true });
        }

        [HttpPost("/api/tvshows/ombi")]
        public async Task<IActionResult> SaveTvShowsAsync([FromBody]SaveOmbiTvShowsSettingsModel model)
        {
            var tvShowsSettings = new TvShowsSettings
            {
                Client = DownloadClient.Ombi,
                Restrictions = model.Restrictions
            };

            var ombiSettings = Sanitize(model);

            DownloadClientsSettingsRepository.SetOmbi(tvShowsSettings, ombiSettings);

            return Ok(new { ok = true });
        }

        private static SaveOmbiMoviesSettingsModel Sanitize(SaveOmbiMoviesSettingsModel model)
        {
            return new SaveOmbiMoviesSettingsModel
            {
                Hostname = model.Hostname.Trim(),
                ApiKey = model.ApiKey.Trim(),
                ApiUsername = model.ApiUsername.Trim(),
                Port = model.Port,
                BaseUrl = model.BaseUrl.Trim(),
                UseSSL = model.UseSSL,
                Version = model.Version,
                UseMovieIssue = model.UseMovieIssue
            };
        }


        private static SaveOmbiTvShowsSettingsModel Sanitize(SaveOmbiTvShowsSettingsModel model)
        {
            return new SaveOmbiTvShowsSettingsModel
            {
                Hostname = model.Hostname.Trim(),
                ApiKey = model.ApiKey.Trim(),
                ApiUsername = model.ApiUsername.Trim(),
                Port = model.Port,
                BaseUrl = model.BaseUrl.Trim(),
                UseSSL = model.UseSSL,
                Version = model.Version,
                UseTVIssue = model.UseTVIssue
            };
        }
    }
}