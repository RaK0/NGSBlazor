using Domain.Entities.Identity;
using Domain.NGSContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NGSBlazor.Server.Configurations;
using NGSBlazor.Server.Extensions;
using NGSBlazor.Server.Interfaces.Context;
using NGSBlazor.Shared.Constants.Application;
using NGSBlazor.Shared.Constants.Permission;
using NGSBlazor.Shared.Constants.Role;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace NGSBlazor.Server.Seeders
{
    public class InitialUserRole : IDatabaseSeeder
    {
        readonly ILogger<InitialUserRole> _logger;
        readonly IStringLocalizer<InitialUserRole> _localizer;
        readonly NGSContext _context;
        readonly UserManager<NGSUser> _userManager;
        readonly RoleManager<NGSRole> _roleManager;
        readonly AppConfiguration _appConfig;

        public InitialUserRole(ILogger<InitialUserRole> logger, IStringLocalizer<InitialUserRole> localizer, NGSContext context, UserManager<NGSUser> userManager, RoleManager<NGSRole> roleManager, IOptions<AppConfiguration> appConfig)
        {
            _logger = logger;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfig = appConfig.Value;
        }

        public void Initialize()
        {
            AddAdministrator();
            if (_appConfig.Debug)
                AddBasicUser();            
        }

        void AddAdministrator()
        {
            Task.Run(async () =>
            {
                try
                {
                    NGSRole adminRole = new(RoleConstants.AdministratorRole, _localizer["Administrator, full permissions"]);
                    NGSRole? adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    if (adminRoleInDb == null)
                    {
                        await _roleManager.CreateAsync(adminRole);
                        adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                        _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                    }
                    NGSUser superUser = new()
                    {
                        Email = ApplicationConstants.AdminUser.Email,
                        UserName = ApplicationConstants.AdminUser.Username,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedOn = DateTime.Now,
                    };
                    NGSUser? superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                    if (superUserInDb == null)
                    {
                        if (_appConfig.Debug)
                        {
                            await _userManager.CreateAsync(superUser, ApplicationConstants.UserDebugPassword.Password);
                            IdentityResult result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                            if (result.Succeeded)
                            {
                                _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    _logger.LogError(error.Description);
                                }
                            }
                        }
                        else
                        {
                            //TODO
                        }                        
                    }

                    foreach (var permission in Permissions.GetRegisteredPermissions())
                    {
                        await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(_localizer["Error creating admin."]);
                }
                _context.SaveChanges();
            }).GetAwaiter().GetResult();
        }
        void AddBasicUser()
        {
            Task.Run(async () =>
            {
                try
                {
                    var basicRole = new NGSRole(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);
                    var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                    if (basicRoleInDb == null)
                    {
                        await _roleManager.CreateAsync(basicRole);
                        _logger.LogInformation(_localizer["Seeded Basic Role."]);
                    }
                    //Check if User Exists
                    var basicUser = new NGSUser
                    {
                        Email = ApplicationConstants.BasicUserDebug.Email,
                        UserName = ApplicationConstants.BasicUserDebug.Username,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedOn = DateTime.Now,
                    };
                    var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                    if (basicUserInDb == null)
                    {
                        await _userManager.CreateAsync(basicUser, ApplicationConstants.UserDebugPassword.Password);
                        await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                        _logger.LogInformation(_localizer["Seeded User with Basic Role."]);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(_localizer["Error creating basic."]);
                }
                _context.SaveChanges();
            }).GetAwaiter().GetResult();
        }
    }
}
