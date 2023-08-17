using AutoMapper;
using AutoMapper.Internal;
using Domain.Entities.Identity;
using Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Identities;
using NGSBlazor.Shared.Constants.Role;
using NGSBlazor.Shared.DTOModels.Identities.Requests;
using NGSBlazor.Shared.DTOModels.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;
using System.Text;
using System.Text.Encodings.Web;

namespace NGSBlazor.Server.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<NGSUser> _userManager;
        readonly IStringLocalizer<UserService> _localizer;
        readonly ICurrentUserService _currentUserService;
        readonly IMapper _mapper;

        public UserService(UserManager<NGSUser> userManager, IStringLocalizer<UserService> localizer, ICurrentUserService currentUserService, IMapper mapper)
        {
            _userManager = userManager;
            _localizer = localizer;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Shared.Wrapper.Result.IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            NGSUser? userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            NGSUser? userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameUserName != null)
                return await Result.FailAsync(_localizer["Username is already taken."]);
            if (userWithSameEmail != null)
                return await Result.FailAsync(_localizer["Email is already registered."]);
            NGSUser user = new()
            {
                UserName = request.UserName,
                Email = request.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
                string verificationUri = await GetVerificationEmailLink(user, origin);
                //TODO send email

                return await Result<string>.SuccessAsync(_localizer["User Registered. Please check your Mailbox to verify!"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
        }
        public async Task<Shared.Wrapper.Result.IResult> ConfirmEmailAsync(string userId, string code)
        {
            NGSUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return await Result.SuccessAsync(_localizer["Account Confirmed"]);
            else
                return await Result.FailAsync(_localizer["Not Allowed."]);
        }

        public async Task<Shared.Wrapper.Result.IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            NGSUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))            
                return await Result.FailAsync(_localizer["Not Allowed."]);
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string route = "account/reset-password";
            Uri endpointUri = new(string.Concat($"{origin}/", route));
            string passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            //TODO send email
            
            return await Result.SuccessAsync(_localizer["Password Reset has been sent check your Email."]);
        }
        public async Task<Shared.Wrapper.Result.IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            NGSUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)            
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
                return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);           
            else
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            
        }
        public async Task<IResult<UserResponse>> GetAsync()
        {
            NGSUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId);
            if(user == null)
                return await Result<UserResponse>.FailAsync(_localizer["An Error has occured!"]);
            UserResponse result = _mapper.Map<UserResponse>(user);
            return await Result<UserResponse>.SuccessAsync(result);
        }
        async Task<string> GetVerificationEmailLink(NGSUser user, string origin)
        {
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string route = "api/identity/user/confirm-email/";
            Uri endpointUri = new (string.Concat($"{origin}/", route));
            string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }
        public async Task<IResult<OtherUserProfileResponse>> GetAsync(Guid userId)
        {
            NGSUser? user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if(user == null)
                return await Result<OtherUserProfileResponse>.FailAsync(_localizer["An Error has occured!"]);
            OtherUserProfileResponse result = _mapper.Map<OtherUserProfileResponse>(user);
            return await Result<OtherUserProfileResponse>.SuccessAsync(result);
        }

    }
}
