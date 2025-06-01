using System.ComponentModel.DataAnnotations;

namespace Askarr.WebApi.Controllers.DownloadClients.Sonarr
{
    public class SonarrProfile
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
