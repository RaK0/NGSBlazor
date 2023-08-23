using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace NGSBlazor.Server.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            StringValues? cultureQuery = context.Request.Query["culture"];
            if (cultureQuery.HasValue && !string.IsNullOrWhiteSpace(cultureQuery.Value) && cultureQuery.Value != StringValues.Empty && cultureQuery.ToString()!=null)
            {
                CultureInfo culture = new(cultureQuery.ToString()??"");

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            else if (context.Request.Headers.ContainsKey("Accept-Language"))
            {
                StringValues cultureHeader = context.Request.Headers["Accept-Language"];
                if (cultureHeader.Any())
                {
                    CultureInfo culture = new(cultureHeader.First().Split(',').First().Trim());

                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
            }

            await _next(context);
        }
    }
}
