using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using NGSBlazor.Client.Authentication;
using NGSBlazor.Client.Extensions;
using NGSBlazor.Client.Interfaces.Authentication;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Client.LocalItems;
using NGSBlazor.Client.Routes;
using NGSBlazor.Shared.Constants.Application;
using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;

namespace NGSBlazor.Client.Services.Authentication
{
    internal class AuthenticationManager : IAuthenticationManager
    {
        readonly ILocalItemStorageService _localItemStorageService;
        readonly HttpClient _httpClient;
        readonly NGSBlazorStateProvider _authenticationStateProvider;
        readonly IStringLocalizer<AuthenticationManager> _localizer;

        public AuthenticationManager(ILocalItemStorageService localItemStorageService, IHttpClientFactory clientFactory, AuthenticationStateProvider authenticationStateProvider, IStringLocalizer<AuthenticationManager> localizer)
        {
            _localItemStorageService = localItemStorageService;
            _httpClient = clientFactory.CreateClient();
            _authenticationStateProvider = (NGSBlazorStateProvider)authenticationStateProvider;
            _localizer = localizer;
        }

        public async Task<ClaimsPrincipal> CurrentUser()
        {
            AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<IResult> Login(LoginRequest loginRequest)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Get, loginRequest);
            IResult<LoginResponse>? result = await response.ToResult<LoginResponse>();
            if (result is not null && result.Succeeded && result.Data is not null)
            {
                string token = result.Data.Token;
                string refreshToken = result.Data.RefreshToken;
                BearerLocalItem localBearer = new BearerLocalItem()
                {
                    Token = token,
                    RefreshToken = refreshToken
                };

                await _localItemStorageService.SetLocalItem(localBearer);

                await _authenticationStateProvider.StateChangedAsync();

                return await Result.SuccessAsync();
            }
            else
            {
                if (result is not null)
                    return await Result.FailAsync(result.Messages);
                return await Result.FailAsync(_localizer["Error"]);
            }
        }

        public async Task<IResult> Logout()
        {
            await _localItemStorageService.ClearItem<BearerLocalItem>();
            await _authenticationStateProvider.StateChangedAsync();
            //_authenticationStateProvider.MarkUserAsLoggedOut();
            return await Result.SuccessAsync();
        }

        public async Task<IResult<BearerLocalItem>> RefreshToken()
        {
            BearerLocalItem? bearer = await _localItemStorageService.GetLocalItem<BearerLocalItem>();
            if (bearer is not null && bearer.Token is not null && bearer.RefreshToken is not null)
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Refresh, new RefreshTokenRequest { Token = bearer.Token, RefreshToken = bearer.RefreshToken });
                IResult<RefreshTokenResponse>? result = await response.ToResult<RefreshTokenResponse>();
                if (result?.Succeeded == false)
                {
                    AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                    if (authState is not null && authState.User?.Identity?.IsAuthenticated == true)
                    {
                        await Logout();
                    }
                    await Result<BearerLocalItem>.FailAsync(result.Messages);
                }
                else if(result?.Data is not null)
                {
                    BearerLocalItem localBearer = new()
                    {
                        Token = result.Data.Token,
                        RefreshToken = result.Data.RefreshToken
                    };

                    await _localItemStorageService.SetLocalItem(localBearer);
                    
                    return await Result<BearerLocalItem>.SuccessAsync(localBearer);
                }
            }

            return await Result<BearerLocalItem>.FailAsync();
        }
        public async Task<IResult<BearerLocalItem>> CheckTokenNeedRefresh(CancellationToken cancellationToken)
        {
            BearerLocalItem? bearer = await _localItemStorageService.GetLocalItem<BearerLocalItem>();
            if (bearer is not null && bearer.Token is not null && bearer.RefreshToken is not null)
            {
                AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                if (authState is not null)
                {
                    string? ticks = authState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Expiration)?.Value;
                    if (ticks is not null)
                    {
                        DateTime expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(ticks)).UtcDateTime;
                        if ((expDate - DateTime.UtcNow).Minutes <= 30)
                        {
                            return await RefreshToken();
                        }
                    }
                }
            }
            return await Result<BearerLocalItem>.FailAsync();
        }
    }
}
