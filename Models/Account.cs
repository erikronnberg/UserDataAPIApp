using Microsoft.AspNetCore.Identity;

namespace UserDataAPIApp.Models
{
    public class Account : IdentityUser
    {
        public long SocialSecurityNumber { get; set; }
    }
}