using Microsoft.AspNetCore.Identity;
using HSBGHelper.Server.Models;

namespace HSBGHelper.Server.Services
{
    public class UserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserService(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task createAdmin()
        {
            User adminUser = new User()
            {
                Name = "admin",
                Email = "hsbghelper@gmail.com",
                Password = "AdminPassword_1234"
            };

            await _userManager.CreateAsync(adminUser);
        }
    }
}
