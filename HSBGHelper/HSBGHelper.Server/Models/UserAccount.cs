using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HSBGHelper.Server.Models
{
    public class UserAccount: IdentityUser<int>
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}