using Domain.Entities.Identity;
using Domain.NGSContexts;
using Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NGSBlazor.Server.Configurations;
using NGSBlazor.Server.Interfaces.Context;
using NGSBlazor.Server.Interfaces.Identities;
using NGSBlazor.Server.Interfaces.Repositories;
using NGSBlazor.Server.Interfaces.Services;
using NGSBlazor.Server.Localization;
using NGSBlazor.Server.Repositories;
using NGSBlazor.Server.Seeders;
using NGSBlazor.Server.Services;
using NGSBlazor.Shared.Constants.Application;
using NGSBlazor.Shared.Constants.Permission;
using NGSBlazor.Shared.Wrapper.JSonSerializer;
using NGSBlazor.Shared.Wrapper.Result;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;


namespace NGSBlazor.Server.Extensions
{
    internal static class ServiceCollectionExtension
    {
        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }
        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NGSContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));           

            return services;
        }
        internal static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJsonSerializer, JSonSerializerNGS>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
        internal static IServiceCollection AddDatabaseSeeders(this IServiceCollection services)
        {
            IEnumerable<Type> seederTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(IDatabaseSeeder).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

            foreach (var seederType in seederTypes)
            {
                services.AddScoped(typeof(IDatabaseSeeder), seederType);
            }
            return services;
        }
        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            return services;
        }

        internal static async Task<IStringLocalizer?> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IStringLocalizer<T>? localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();

            return localizer;
        }
        internal static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(async c =>
            {
                
                // include all project's xml comments
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "NGS",
                });

                IStringLocalizer? localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = localizer != null ? localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"] : "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppConfiguration? config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            byte[] key = Encoding.UTF8.GetBytes(config.JWTSecret);
            services
                .AddAuthentication(options => 
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })                
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, bearer =>
                {
                    if (config.Debug)
                        bearer.RequireHttpsMetadata = false;
                    else
                        bearer.RequireHttpsMetadata = true;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,                        
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero                        
                    };
                    IStringLocalizer? localizer = GetRegisteredServerLocalizerAsync<ServerCommonResources>(services).GetAwaiter().GetResult();

                    bearer.Events = new JwtBearerEvents
                    {                       
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                string result = JsonConvert.SerializeObject(Result.Fail(localizer?["The Token is expired."]??"Fail"));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
                                if (config.Debug)
                                {
                                    c.NoResult();
                                    c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    c.Response.ContentType = "text/plain";
                                    return c.Response.WriteAsync(c.Exception.ToString());
                                }
                                else
                                {
                                    c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    c.Response.ContentType = "application/json";
                                    string result = JsonConvert.SerializeObject(Result.Fail(localizer?["An unhandled error has occurred."]??"Fail"));
                                    return c.Response.WriteAsync(result);
                                }                                
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer?["You are not Authorized."]??"Fail"));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer?["You are not authorized to access this resource."] ?? "Fail"));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization(options =>
            {                
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                });
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    object? propertyValue = prop.GetValue(null);
                    if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()))
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
        internal static IServiceCollection AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            IConfigurationSection applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);

            return services;
        }
        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<NGSUser, NGSRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<NGSContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
    internal class ServerCommonResources
    {
        // Used for get it from this class
    }
}
