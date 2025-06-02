using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.DownloadClients.Radarr
{
    public class RadarrPath
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
