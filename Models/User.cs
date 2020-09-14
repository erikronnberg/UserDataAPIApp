using Microsoft.AspNetCore.Identity;

namespace UserDataAPIApp.Models
{
    public class User : IdentityUser
    {
        public int SocialSecurityNumber { get; set; }
    }
}