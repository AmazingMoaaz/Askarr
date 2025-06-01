using System.ComponentModel.DataAnnotations;

namespace Askarr.WebApi.Controllers.DownloadClients.Sonarr
{
    public class SaveSonarrSettingsModel : SonarrSettingsModel
    {
        [Required]
        public string Restrictions { get; set; }
    }
}