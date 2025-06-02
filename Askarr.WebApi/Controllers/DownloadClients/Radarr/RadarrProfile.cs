using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.DownloadClients.Radarr
{
    public class RadarrProfile
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
