﻿using System.ComponentModel.DataAnnotations;

namespace Requestrr.WebApi.Controllers.DownloadClients.Ombi
{
    public class SaveOmbiMoviesSettingsModel : OmbiSettingsModel
    {
        [Required]
        public bool UseMovieIssue { get; set; }
    }
}