using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NGSBlazor.Client;
using NGSBlazor.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args)
.AddClientStuff();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("NGSBlazor.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NGSBlazor.ServerAPI"));

WebAssemblyHost host = builder.Build();

await host.SetLanguage();

await host.RunAsync();

