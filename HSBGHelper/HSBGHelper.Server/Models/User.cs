using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HSBGHelper.Server.Models
{
    public class User : IdentityUser<int>
    {
    }
}