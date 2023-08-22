namespace NGSBlazor.Client.Interfaces.Services
{
    public interface ITokenService
    {
        void Init();
        void AddTokenCredentials(string token, string refreshToken);
    }
}
