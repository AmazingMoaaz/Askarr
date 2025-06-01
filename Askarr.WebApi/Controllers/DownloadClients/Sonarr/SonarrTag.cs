using System.ComponentModel.DataAnnotations;

namespace Askarr.WebApi.Controllers.DownloadClients.Sonarr
{
    public class SonarrTag
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
