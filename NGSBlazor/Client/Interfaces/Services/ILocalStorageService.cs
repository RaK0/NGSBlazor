using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Client.Interfaces.Services
{
    public interface ILocalStorageService
    {
        Task SetCookie<T>(T value) where T : class, ILocalItem;
        Task<T?> GetCookie<T>() where T : class, ILocalItem;
        void ClearAllCookies();
    }
}
