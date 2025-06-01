using System.ComponentModel.DataAnnotations;
using Askarr.WebApi.Controllers.DownloadClients.Ombi;
using Askarr.WebApi.Controllers.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;

namespace Askarr.WebApi.Controllers.DownloadClients
{
    public class MovieSettingsModel
    {
        [Required]
        public string Client { get; set; }

        [Required]
        public RadarrSettingsModel Radarr { get; set; }

        [Required]
        public OmbiMovieSettings Ombi { get; set; }

        [Required]
        public OverseerrSettings Overseerr { get; set; }

        [Required]
        public string[] OtherCategories { get; set; }
    }
}
