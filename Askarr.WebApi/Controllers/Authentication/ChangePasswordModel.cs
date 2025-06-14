﻿using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.Authentication
{
    public class ChangePasswordModel
    {
        [Required]
        public string ExistingPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string NewPasswordConfirmation { get; set; }
    }
}
