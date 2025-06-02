using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.Authentication
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
