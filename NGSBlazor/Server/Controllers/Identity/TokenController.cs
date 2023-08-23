using Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGSBlazor.Server.Interfaces.Services;
using NGSBlazor.Server.Seeders;
using NGSBlazor.Shared.Identities.Requests;
using Serilog;

namespace NGSBlazor.Server.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    [ApiVersionNeutral]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
            
        }

        /// <summary>
        /// Get Token by login.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<ActionResult> Get(LoginRequest loginRequest)
        {           
            var response = await _tokenService.LoginAsync(loginRequest);
            return Ok(response);
        }
        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh(RefreshTokenRequest refreshTokenRequest)
        {
            var response = await _tokenService.GetRefreshTokenAsync(refreshTokenRequest);
            return Ok(response);
        }
    }
}
