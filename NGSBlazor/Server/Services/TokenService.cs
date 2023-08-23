using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NGSBlazor.Server.Configurations;
using NGSBlazor.Server.Controllers.Identity;
using NGSBlazor.Server.Interfaces.Services;
using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NGSBlazor.Server.Services
{
    internal class TokenService : ITokenService
    {
        private readonly UserManager<NGSUser> _userManager;
        private readonly RoleManager<NGSRole> _roleManager;
        private readonly AppConfiguration _appConfig;
        private readonly IStringLocalizer<TokenService> _localizer;
        readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger, UserManager<NGSUser> userManager, RoleManager<NGSRole> roleManager, IOptions<AppConfiguration> appConfig, IStringLocalizer<TokenService> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfig = appConfig.Value;
            _localizer = localizer;
            _logger = logger;
        }
        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            NGSUser? user = await _userManager.FindByEmailAsync(loginRequest.Username);
            user ??= await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
                return await Result<LoginResponse>.FailAsync(_localizer["User Not Found."]);

            bool passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!passwordValid)
                return await Result<LoginResponse>.FailAsync(_localizer["Invalid Password."]);
            if (user.Deleted)
                return await Result<LoginResponse>.FailAsync(_localizer["User deleted."]);
            if (!user.EmailConfirmed)
                return await Result<LoginResponse>.FailAsync(_localizer["E-Mail not confirmed."]);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(10);

            await _userManager.UpdateAsync(user);

            string token = await GenerateJwtAsync(user);
            var response = new LoginResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<LoginResponse>.SuccessAsync(response);
        }

        public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
                return await Result<RefreshTokenResponse>.FailAsync(_localizer["Invalid Client Token."]);

            ClaimsPrincipal? userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            if (userPrincipal == null)
                return await Result<RefreshTokenResponse>.FailAsync(_localizer["Invalid Credentials."]);

            string? userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return await Result<RefreshTokenResponse>.FailAsync(_localizer["User Not Found."]);
            NGSUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return await Result<RefreshTokenResponse>.FailAsync(_localizer["User Not Found."]);
            if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<RefreshTokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            string token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(10);
            await _userManager.UpdateAsync(user);

            var response = new RefreshTokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<RefreshTokenResponse>.SuccessAsync(response);
        }
        private async Task<string> GenerateJwtAsync([NotNull] NGSUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, [NotNull] IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddHours(5),
               signingCredentials: signingCredentials);
            JwtSecurityTokenHandler tokenHandler = new();
            string encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }
        private async Task<IEnumerable<Claim>> GetClaimsAsync([NotNull] NGSUser user)
        {
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            userClaims ??= new List<Claim>();
            IList<string> roles = await _userManager.GetRolesAsync(user);
            roles ??= new List<string>();
            List<Claim> roleClaims = new();
            List<Claim> permissionClaims = new();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                NGSRole? thisRole = await _roleManager.FindByNameAsync(role);
                if (thisRole is not null)
                {
                    IList<Claim> allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                    permissionClaims.AddRange(allPermissionsForThisRoles);
                }
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.JWTSecret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            if (tokenHandler.CanReadToken(token))
                return null;
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JWTSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(_localizer["Invalid token"]);
            }

            return principal;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
