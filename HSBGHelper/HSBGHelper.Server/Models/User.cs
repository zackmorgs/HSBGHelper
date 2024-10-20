using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HSBGHelper.Server.Models
{
    public class User : IdentityUser
    {
        public User() : base()
        {
            Name = "";
            Email = "";
            Password = "";
            EmailConfirmed = true;
        }
        public string Name { get; set; }
        public required override string Email { get; set; }
        public required string Password { get; set; }
    }
}