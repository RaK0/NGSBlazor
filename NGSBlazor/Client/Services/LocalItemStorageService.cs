using Blazored.LocalStorage;
using NGSBlazor.Client.Interfaces;
using NGSBlazor.Client.Interfaces.Services;
using System.Reflection;

namespace NGSBlazor.Client.Services
{
    public class LocalItemStorageService : ILocalItemStorageService
    {
        private readonly ILocalStorageService _localStorageService;

        public LocalItemStorageService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;

        }

        public void ClearAllLocalItems() => _localStorageService?.ClearAsync();

        public async Task<T?> GetLocalItem<T>() where T : class, ILocalItem
        {
            Type type = typeof(T);
            PropertyInfo? nameProperty = type.GetProperty(nameof(type.Name));
            if (nameProperty == null)
                return null;
            T localItem = await _localStorageService.GetItemAsync<T>(nameProperty.Name);

            return localItem ?? null;
        }

        public async Task SetLocalItem<T>(T value) where T : class, ILocalItem
        {
            await _localStorageService.SetItemAsync(value.Name, value);
        }

        public async Task ClearItem<T>() where T : class, ILocalItem
        {
            T? item = (T?)Activator.CreateInstance(typeof(T));
            if (item is not null)
            {
                await _localStorageService.SetItemAsync(item.Name, item);
            }
        }
    }
}
