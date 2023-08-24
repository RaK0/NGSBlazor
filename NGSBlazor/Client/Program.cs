using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using NGSBlazor.Client;
using NGSBlazor.Client.Authentication;
using NGSBlazor.Client.Extensions;
using NGSBlazor.Client.Interfaces.Authentication;
using NGSBlazor.Client.Interfaces.Services;
using NGSBlazor.Client.Services;
using NGSBlazor.Client.Services.Authentication;
using NGSBlazor.Shared.Constants.Application;

var builder = WebAssemblyHostBuilder.CreateDefault(args)
.AddClientServices();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services
    .AddHttpClient(ApplicationConstants.HttpClientName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthetnicateDelegatingHandler>();
builder.Services
    .AddHttpClient(Options.DefaultName, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ApplicationConstants.HttpClientName));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Options.DefaultName));



WebAssemblyHost host = builder.Build();

await host.SetLanguage();

await host.RunAsync();

