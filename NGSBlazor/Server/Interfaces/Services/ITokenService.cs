using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Server.Interfaces.Services
{
    public interface ITokenService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest);

        Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
