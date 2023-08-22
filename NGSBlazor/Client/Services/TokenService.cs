using NGSBlazor.Client.Interfaces.Services;

namespace NGSBlazor.Client.Services
{
    internal class TokenService : ITokenService
    {
        readonly ILocalStorageService _localStorageService;
        string? Token { get; set; }
        string? RefreshToken { get; set; }
        bool _inited { get; set; } = false;

        public void AddTokenCredentials(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
