using NGSBlazor.Shared.Wrapper.Result;

namespace NGSBlazor.Client.Interfaces.Services
{
    public interface ILocalItemStorageService
    {
        Task SetLocalItem<T>(T value) where T : class, ILocalItem;
        Task<T?> GetLocalItem<T>() where T : class, ILocalItem;
        void ClearAllLocalItems();
        Task ClearItem<T>() where T : class, ILocalItem;
    }
}
