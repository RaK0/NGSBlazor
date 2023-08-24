using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Client.Services;
using NGSBlazor.Shared.Constants.Permission;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NGSBlazor.Client.Interfaces.Authentication;
using NGSBlazor.Client.Services.Authentication;
using NGSBlazor.Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace NGSBlazor.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder
                .Services
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddAuthorizationCore(RegisterPermissionClaims)
                .AddBlazoredLocalStorage()
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = true;
                });
            builder.AddServices();
            
            return builder;
        }
        private static WebAssemblyHostBuilder AddServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<ILocalItemStorageService, LocalItemStorageService>();
            builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, NGSBlazorStateProvider>();
            builder.Services.AddScoped<AuthetnicateDelegatingHandler>();

            return builder;
        }       
        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        }
    }
}
