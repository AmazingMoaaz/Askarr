﻿using  Askarr.WebApi.Controllers.DownloadClients.Lidarr;
using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.DownloadClients
{
    public class MusicSettingsModel
    {
        [Required]
        public string Client { get; set; }

        [Required]
        public LidarrSettingsModel Lidarr { get; set; }

        [Required]
        public string[] OtherCategories { get; set; }
    }
}
