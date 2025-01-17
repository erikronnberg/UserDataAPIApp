﻿using System.ComponentModel.DataAnnotations;

namespace UserDataAPIApp.Models
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "SSN is required")]
        public string SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
