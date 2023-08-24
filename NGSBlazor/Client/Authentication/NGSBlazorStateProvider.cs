using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Client.LocalItems;
using NGSBlazor.Shared.Constants.Application;
using NGSBlazor.Shared.Constants.Permission;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace NGSBlazor.Client.Authentication
{
    public class NGSBlazorStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalItemStorageService _localStorage;

        public NGSBlazorStateProvider(IHttpClientFactory httpClient, ILocalItemStorageService localStorage)
        {
            _httpClient = httpClient.CreateClient();
            _localStorage = localStorage;
        }
        public Task StateChangedAsync()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return Task.CompletedTask;
        }

        public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        {
            AuthenticationState state = await GetAuthenticationStateAsync();
            ClaimsPrincipal authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            BearerLocalItem? savedToken = await _localStorage.GetLocalItem<BearerLocalItem>();
            if (string.IsNullOrWhiteSpace(savedToken?.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(savedToken.Token);
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken.Token);
            AuthenticationState state = new(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "Bearer")));

            return state;

        }
    }
}