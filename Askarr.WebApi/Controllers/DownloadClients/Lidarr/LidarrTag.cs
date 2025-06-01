using System.ComponentModel.DataAnnotations;

namespace Askarr.WebApi.Controllers.DownloadClients.Lidarr
{
    public class LidarrTag
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
