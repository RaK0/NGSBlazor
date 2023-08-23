using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Server.Interfaces.Identities
{
    public interface IUserService
    {
        Task<Shared.Wrapper.Result.IResult> RegisterAsync(RegisterRequest request, string? origin);
        Task<IResult<UserResponse>> GetAsync();
        Task<Shared.Wrapper.Result.IResult> ConfirmEmailAsync(string userId, string code);
        Task<Shared.Wrapper.Result.IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
        Task<Shared.Wrapper.Result.IResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<IResult<OtherUserProfileResponse>> GetAsync(Guid userId);
    }
}
