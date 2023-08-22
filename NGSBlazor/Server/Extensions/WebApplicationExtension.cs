using Domain.NGSContexts;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using NGSBlazor.Server.Interfaces.Context;
using NGSBlazor.Server.Middlewares;
using NGSBlazor.Shared.Constants.Localization;
using Serilog;
using System.Globalization;

namespace NGSBlazor.Server.Extensions
{
    public static class WebApplicationExtension
    {

        internal static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            NGSContext? context = serviceScope.ServiceProvider.GetService<NGSContext>();

            if (context is not null && context.Database.IsSqlServer())
            {
#if DEBUG
                context.Database.EnsureDeleted();
#endif
                context.Database.Migrate();
            }

            return app;
        }
        internal static IApplicationBuilder InitializeSeeders(this IApplicationBuilder app)
        {
            IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            IEnumerable<IDatabaseSeeder> initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
        internal static IApplicationBuilder UseRequestLocalizationByCulture(this IApplicationBuilder app)
        {
            CultureInfo[] supportedCultures = LocalizationConstants.SupportedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();
            app.UseRequestLocalization(options =>
            {
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.ApplyCurrentCultureToResponseHeaders = true;
            });
            //? potrzebne? zastapic zadaniem do api
            app.UseMiddleware<RequestCultureMiddleware>();

            return app;
        }
    }
}
