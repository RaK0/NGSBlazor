using Blazored.LocalStorage;
using NGSBlazor.Client.LocalItems;
using NGSBlazor.Client.Interfaces;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Shared.Wrapper.Result;
using System.Reflection;

namespace NGSBlazor.Client.Services
{
    public class LocalStorageService : Interfaces.Services.ILocalStorageService
    {
        private readonly Blazored.LocalStorage.ILocalStorageService _localStorageService;

        public LocalStorageService(Blazored.LocalStorage.ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public void ClearAllCookies()
        {
            _ = _localStorageService.ClearAsync();
        }

        public async Task<T?> GetCookie<T>() where T : class, ILocalItem
        {
            Type type = typeof(T);
            PropertyInfo? nameProperty = type.GetProperty(nameof(type.Name));
            if (nameProperty == null)
                return null;
            T cookie = await _localStorageService.GetItemAsync<T>(nameProperty.Name);

            return cookie ?? null;
        }

        public async Task SetCookie<T>(T value) where T : class, ILocalItem
        {
            await _localStorageService.SetItemAsync(value.Name, value);
        }
    }
}
