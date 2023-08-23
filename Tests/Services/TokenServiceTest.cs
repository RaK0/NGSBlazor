using AutoFixture;
using Castle.Core.Logging;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NGSBlazor.Server.Configurations;
using NGSBlazor.Server.Interfaces.Services;
using NGSBlazor.Server.Services;
using NGSBlazor.Shared.Identities.Requests;
using NGSBlazor.Shared.Identities.Response;
using NGSBlazor.Shared.Wrapper.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
    [TestClass]
    public class TokenServiceTest
    {
        readonly Fixture _fixture;
        readonly Mock<ILogger<TokenService>> logger = new();
        readonly Mock<IOptions<AppConfiguration>> opt = new();
        readonly Mock<IStringLocalizer<TokenService>> localize = new();
        readonly NGSUser user;

        public TokenServiceTest()
        {
            _fixture = new Fixture();
            opt.Setup(x => x.Value).Returns(new AppConfiguration() { Debug = false, JWTSecret = string.Join("", Enumerable.Range(0, 128).Select(_ => Guid.NewGuid().ToString("N"))) });
            user = _fixture.Create<NGSUser>();
            user.EmailConfirmed = true;
            user.Deleted = false;
            user.Email = "UserTest@mail.pl";
        }

        [TestMethod]
        public async Task GetToken()
        {
            string pass = "testpass";
            Mock<UserManager<NGSUser>> userManager = new(Mock.Of<IUserStore<NGSUser>>(), null, null, null, null, null, null, null, null);
            Mock<RoleManager<NGSRole>> roleManager = new(Mock.Of<IRoleStore<NGSRole>>(), null, null, null, null);

            ITokenService tokenService = new TokenService(logger.Object, userManager.Object, roleManager.Object, opt.Object, localize.Object);                        

            userManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            userManager.Setup(x => x.CheckPasswordAsync(user, pass)).ReturnsAsync(true);
            Result<LoginResponse> result = await tokenService.LoginAsync(new LoginRequest() { Username = user.Email, Password = pass });

            Result<LoginResponse> resultShouldFailNoUser = await tokenService.LoginAsync(new LoginRequest() { Username = "random", Password = pass });
            Result<LoginResponse> resultShouldFailWrongPassword = await tokenService.LoginAsync(new LoginRequest() { Username = user.Email, Password = "wrongpassword" });

            Assert.IsTrue(result.Succeeded,"Good credentials");
            Assert.IsTrue(!string.IsNullOrEmpty(result?.Data?.Token));
            Assert.IsTrue(!string.IsNullOrEmpty(result?.Data?.RefreshToken));
            Assert.IsTrue(result?.Data?.RefreshTokenExpiryTime is not null);

            Assert.IsFalse(resultShouldFailNoUser.Succeeded, "Wrong User");
            Assert.IsFalse(resultShouldFailWrongPassword.Succeeded, "Wrong password");

        }
    }
}
