using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using NGSBlazor.Server.Configurations;
using NGSBlazor.Server.Extensions;
using NGSBlazor.Server.Middlewares;
using NGSBlazor.Shared.Constants.Application;
using Serilog;
using System.Security.Claims;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
 
//builder.Logging.AddSerilog();
//builder.Host.UseSerilog((context, serv, conf) =>
//{
//    conf.ReadFrom.Configuration(context.Configuration);
//});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("/");
        });
});
builder.Services.AddApplicationServices();
builder.Services.AddIdentity();
builder.Services.AddJwtAuthentication(builder.Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>() ?? null);

builder.Services.AddControllers();

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});
builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddAutoMapper();
builder.Services.AddServerLocalization();
builder.Services.AddSwagger();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddDatabaseSeeders();
builder.Services.AddRazorPages();



WebApplication app = builder.Build();
//app.UseSerilogRequestLogging(options =>
//{
//    options.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
    });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRequestLocalizationByCulture();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.InitializeDatabase();
app.InitializeSeeders();

app.Run();
