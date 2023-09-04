using Chinook.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Services
{
    public class UserService : IUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> GetUserId()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }
    }
}
