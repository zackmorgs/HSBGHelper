
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HSBGHelper.Server.Models
{

    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly UserManager<User> _userManager;
        private readonly ISessionStorageService _sessionStorageService;

        public AuthProvider(
            UserManager<User> userManager,
            ISessionStorageService sessionStorageService
        )
        {
            _userManager = userManager;
            _sessionStorageService = sessionStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(); ;

            var userId = await _sessionStorageService
                .GetItemAsync<int>("userId");

            if (userId > 0)
            {
                var user = await _userManager
                    .FindByIdAsync(userId.ToString());

                if (user is not null)
                {
                    identity = new ClaimsIdentity(new[]
                    { new Claim("UserAccountId", user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email ?? ""), new Claim(ClaimTypes.Email, user.Email) }, "CardOrgAuth");
                }
                else
                {
                    // can't find the user, kill the session
                    await _sessionStorageService.RemoveItemAsync("userId");
                }
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}