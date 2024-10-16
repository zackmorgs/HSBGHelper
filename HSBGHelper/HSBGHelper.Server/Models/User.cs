using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HSBGHelper.Server.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public required override string Email { get; set; }
        public required string Password { get; set; }
    }
}