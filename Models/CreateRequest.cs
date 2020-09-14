using System.ComponentModel.DataAnnotations;

namespace UserDataAPIApp.Models
{
    public class CreateRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string SocialSecurityNumber { get; set; }

        [Required]
        [EnumDataType(typeof(Policies))]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(10)]
        public string Password { get; set; }
    }
}
