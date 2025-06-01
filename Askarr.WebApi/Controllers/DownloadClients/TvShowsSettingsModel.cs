using System.ComponentModel.DataAnnotations;
using Askarr.WebApi.Controllers.DownloadClients.Ombi;
using Askarr.WebApi.Controllers.DownloadClients.Overseerr;
using Askarr.WebApi.Controllers.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;

namespace Askarr.WebApi.Controllers.DownloadClients
{
    public class TvShowsSettingsModel
    {
        [Required]
        public string Client { get; set; }
        [Required]
        public SonarrSettingsModel Sonarr { get; set; }
        [Required]
        public OmbiTVSettings Ombi { get; set; }
        [Required]
        public OverseerrSettings Overseerr { get; set; }
        [Required]
        public string Restrictions { get; set; }

        [Required]
        public string[] OtherCategories { get; set; }
    }
}