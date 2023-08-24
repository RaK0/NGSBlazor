using NGSBlazor.Client.LocalItems;
using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Wrapper.Result;
using System.Security.Claims;

namespace NGSBlazor.Client.Interfaces.Authentication
{
    internal interface IAuthenticationManager
    {
        Task<IResult> Login(LoginRequest loginRequest);

        Task<IResult> Logout();

        Task<IResult<BearerLocalItem>> RefreshToken();
        Task <IResult<BearerLocalItem>> CheckTokenNeedRefresh(CancellationToken cancellationToken);
        Task<ClaimsPrincipal> CurrentUser();
    }
}
