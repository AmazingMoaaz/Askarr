﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Askarr.WebApi.config;

namespace Askarr.WebApi.Controllers.Configuration
{
    [ApiController]
    [Route("/api/settings")]
    public class ApplicationSettingsController : ControllerBase
    {
        private readonly ApplicationSettings _applicationSettings;

        public ApplicationSettingsController(ApplicationSettingsProvider applicationSettingsProvider)
        {
            _applicationSettings = applicationSettingsProvider.Provide();
        }

        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(new ApplicationSettingsModel
            {
                Port = _applicationSettings.Port,
                BaseUrl = _applicationSettings.BaseUrl,
                DisableAuthentication = _applicationSettings.DisableAuthentication
            });
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> SaveAsync([FromBody]ApplicationSettingsModel model)
        {
            model.BaseUrl = model.BaseUrl.Trim();

            if(!string.IsNullOrWhiteSpace(model.BaseUrl) && !model.BaseUrl.StartsWith("/"))
            {
                return BadRequest(new { Error = "Base urls must start with /" });
            }

            _applicationSettings.Port = model.Port;
            _applicationSettings.BaseUrl = model.BaseUrl;
            _applicationSettings.DisableAuthentication = model.DisableAuthentication;

            ApplicationSettingsRepository.Update(_applicationSettings);

            return Ok(new { ok = true });
        }
    }
}