using System.ComponentModel.DataAnnotations;

namespace Askarr.WebApi.Controllers.DownloadClients.Overseerr
{
    public class TestOverseerrSettingsModel
    {
        [Required]
        public string Hostname { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public bool UseSSL { get; set; }
        public string DefaultApiUserID { get; set; }
        [Required]
        public string Version { get; set; }
    }
}