using System.ComponentModel.DataAnnotations;

namespace UserDataAPIApp.Models
{
    public class GetUser
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

    }
}