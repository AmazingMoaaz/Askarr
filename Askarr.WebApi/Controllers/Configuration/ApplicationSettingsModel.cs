using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.Configuration
{
    public class ApplicationSettingsModel
    {
        [Required]
        public int Port { get; set; }

        public string BaseUrl { get; set; }

        [Required]
        public bool DisableAuthentication { get; set; }
    }
}