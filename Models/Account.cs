using Microsoft.AspNetCore.Identity;

namespace UserDataAPIApp.Models
{
    public class Account : IdentityUser
    {
        public int SocialSecurityNumber { get; set; }
    }
}