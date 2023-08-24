using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NGSBlazor.Client.LocalItems;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Shared.Constants.Localization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NGSBlazor.Client.Extensions
{
    public static class WebAssemblyHostExtensions
    {
        public static async Task SetLanguage(this WebAssemblyHost host)
        {
            ILocalItemStorageService? coockieService = host.Services.GetService<ILocalItemStorageService>();
            if (coockieService != null)
            {
                LanguageLocalItem? langCookie = await coockieService.GetLocalItem<LanguageLocalItem>();
                if (langCookie != null)
                {
                    if (langCookie.Code == null)
                    {
                        langCookie.Code = LocalizationConstants.SupportedLanguages.First().Code;
                        _=coockieService.SetLocalItem(langCookie);
                        SetDefault();
                    }
                    else
                    {
                        string[] supportedCultures = LocalizationConstants.SupportedLanguages.Select(l => l.Code).ToArray();
                        if (supportedCultures.Contains(langCookie.Code))
                        {
                            CultureInfo culture = new(langCookie.Code);
                            CultureInfo.DefaultThreadCurrentCulture = culture;
                            CultureInfo.DefaultThreadCurrentUICulture = culture;
                        }
                    }                    
                }
                else
                {
                    SetDefault();
                }
            }
            else
            {
                SetDefault();
            }            
        }
        static void SetDefault()
        {
            CultureInfo culture = new(LocalizationConstants.SupportedLanguages.First().Code);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
