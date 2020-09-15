using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDataAPIApp.Models
{
    public class UpdateUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Password { get; set; }
    }
}