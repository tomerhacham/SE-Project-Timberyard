using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Services;
using WebService.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Timberyard_UnitTests.Stubs;
using WebService.Domain.DataAccess;
using System.Linq;
using ETL.DataObjects;
using WebService.Domain.Business.Authentication;
using WebService.Domain.DataAccess.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebService.Utils.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using WebService.Utils.ExtentionMethods;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class AuthenticationTests : TestSuit
    {
        AuthenticationController AuthenticationController { get; set; }
        InMemoryAlarmsAndUsersRepository UsersRepository { get; set; }

        private readonly string Secret;

        public AuthenticationTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests", inMemoryAlarmsRepository: true, inMemoryLogsAndTestRepository: true);
            AuthenticationController = serviceProvider.GetService<AuthenticationController>();
            UsersRepository = serviceProvider.GetService<IAlarmsAndUsersRepository>() as InMemoryAlarmsAndUsersRepository;
            Secret = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariablesForTesting("IntegrationTests").Build()
                        .GetSection("AuthenticationSettings").Get<AuthenticationSettings>().Secret;

        }

        #region CRUD Alarms Scenarios
        [Fact]
        public async void AddUser()
        {
            string email = "addUserTest@timberyard.com";
            var result = await AuthenticationController.AddUser(email);
            Assert.True(result.Status);
            Assert.NotEmpty(UsersRepository.Users);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(UsersRepository.Users[email].Role, Role.RegularUser);
            Assert.Equal(email, UsersRepository.Users.Keys.First());
        }

        [Fact]
        public async void RemoveUser()
        {
            int totalUsers = 3;
            for (int i = 1; i <= totalUsers; i++)
            {
                var insert_result = await AuthenticationController.AddUser($"removeUserTest{i}@timberyard.com");
                Assert.True(insert_result.Status);
                Assert.Equal(i, UsersRepository.Users.Count);
            }

            Assert.Equal(totalUsers, UsersRepository.Users.Count);
            // Add and Remove user from exists repository
            string emailToRemove = "removeUserTest1@timberyard.com";
            var remove_result = await AuthenticationController.RemoveUser(emailToRemove);

            Assert.True(remove_result.Status);
            Assert.False(UsersRepository.Users.ContainsKey(emailToRemove));
            Assert.Equal(totalUsers - 1, UsersRepository.Users.Count);
            Assert.False(UsersRepository.Users.TryGetValue(emailToRemove, out UserDTO user));
        }

        [Fact]
        public async void RemoveUser_notExists()
        {
            string emailToRemove = "userToRemoveNotExists@timberyard.com";
            bool result = UsersRepository.Users.ContainsKey(emailToRemove);

            if (!result)
            {
                var remove_result = await AuthenticationController.RemoveUser(emailToRemove);
                Assert.False(remove_result.Status);
                Assert.False(UsersRepository.Users.ContainsKey(emailToRemove));
            }
        }

        [Fact]
        public async void ChangeSystemAdminPassword_worngPass()
        {
            string email = "ChangeSystemAdminPassword_worngPass@timberyard.com";
            var insert_result = await AuthenticationController.AddSystemAdmin(email);
            Assert.True(insert_result.Status);
            Assert.True(UsersRepository.Users.ContainsKey(email));
            string pass_generated = UsersRepository.Users[email].Password;
            Assert.NotEmpty(pass_generated);

            // put old password to be "worngPass" that cant be generated when adding new system admin
            var remove_result = await AuthenticationController.ChangeSystemAdminPassword(email, "worngPass", "123456");
            Assert.False(remove_result.Status);
            Assert.True(UsersRepository.Users.ContainsKey(email));
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(pass_generated, user.Password);
        }

        [Fact]
        public async void AddSystemAdmin()
        {
            string email = "addSystemAdminTest@timberyard.com";
            var result = await AuthenticationController.AddSystemAdmin(email);

            Assert.True(result.Status);
            Assert.NotEmpty(UsersRepository.Users);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(Role.Admin, UsersRepository.Users[email].Role);
            Assert.NotEmpty(UsersRepository.Users[email].Password);
            Assert.Equal(email, UsersRepository.Users.Keys.First());
        }

        [Fact]
        public async void AddSystemAdmin_userExists()
        {
            string email = "addSystemAdminUserExists@timberyard.com";
            var insert_result = await UsersRepository.AddUser(new UserDTO() { Email = email, Role = Role.RegularUser });
            Assert.True(insert_result.Status);

            var result = await AuthenticationController.AddSystemAdmin(email);

            Assert.True(result.Status);
            Assert.NotEmpty(UsersRepository.Users);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(Role.Admin, UsersRepository.Users[email].Role);
            Assert.NotEmpty(UsersRepository.Users[email].Password);
            Assert.Equal(email, UsersRepository.Users.Keys.First());
        }

        #endregion

        #region Login and Password

        [Fact]
        public async void Login()
        {
            string email = "login@timberyard.com";
            string password = "testPassword";
            // TODO on resular User timeStamp
            var insert_result = await UsersRepository.AddUser(new UserDTO() { Email = email, Password = password.HashString(), Role = Role.Admin });
            Assert.True(insert_result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO newUser));

            var result = await AuthenticationController.Login(email, password);
            Assert.True(result.Status);
            Assert.NotNull(result.Data);

            // Validate token
            JWTtoken token = result.Data;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var emailFromToken = jwtToken.Claims.First(x => x.Type == "Email").Value;
            var role = Enum.Parse(typeof(Role), jwtToken.Claims.First(x => x.Type == "Role").Value);
            Assert.Equal(email, emailFromToken);
            Assert.Equal(Role.Admin, role);
        }

        [Fact]
        public async void Login_worngPass()
        {
            string email = "loginWorngPass@timberyard.com";
            string password = "testPass";
            var insert_result = await UsersRepository.AddUser(new UserDTO() { Email = email, Password = password });
            Assert.True(insert_result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO newUser));
            Assert.Equal(password, newUser.Password);

            string worng_password = "worngPass";
            var result = await AuthenticationController.Login(email, worng_password);
            Assert.False(result.Status);
            Assert.Null(result.Data);
            Assert.Equal(password, UsersRepository.Users[email].Password);
        }

        [Fact]
        public async void Login_notExists()
        {
            string email = "notExists@timberyard.com";
            string password = "testPass";
            Assert.False(UsersRepository.Users.TryGetValue(email, out UserDTO user));

            var result = await AuthenticationController.Login(email, password);
            Assert.False(result.Status);
            Assert.Null(result.Data);
        }

        [Fact]
        public async void ChangeSystemAdminPassword()
        {
            string email = "ChangeSystemAdminPasswordTest@timberyard.com";
            string hash_pass = "?T?)\u0015H??\a/?[??\u001b? 4B\u0002\u0006-???O\u0003?By?Z"; // "oldPass" after encyption
            //var result = await AuthenticationController.AddSystemAdmin(email);
            var result = await UsersRepository.AddUser(new UserDTO { Email = email, Password = hash_pass, Role = Role.Admin });
            Assert.True(result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO userAdded));

            var update_result = await AuthenticationController.ChangeSystemAdminPassword(email, "NewPass", "OldPass");
            Assert.True(update_result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO userEdited));
            Assert.NotNull(userEdited);
            Assert.Equal(email, userEdited.Email);
            Assert.Equal("NewPass".HashString(), userEdited.Password);
        }

        [Fact]
        public async void ChangeSystemAdminPassword_notExists()
        {
            string email = "ChangeSystemAdminPassword_notExistsTest@timberyard.com";
            bool result = UsersRepository.Users.ContainsKey(email);

            if (!result)
            {
                var remove_result = await AuthenticationController.ChangeSystemAdminPassword(email, "Test", "notExists");
                Assert.False(remove_result.Status);
                Assert.False(UsersRepository.Users.ContainsKey(email));
            }
        }

        [Fact]
        public async void ForgetPassword()
        {
            string email = "forgetPassword@timberyard.com";
            var insert_result = await AuthenticationController.AddUser(email);
            Assert.True(insert_result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            string oldPass = user.Password;

            var result = await AuthenticationController.ForgetPassword(email);
            Assert.True(result.Status);
            Assert.True(UsersRepository.Users.ContainsKey(email));
            Assert.NotEqual(oldPass, UsersRepository.Users[email].Password);
        }

        [Fact]
        public async void ForgetPassword_notExists()
        {
            string email = "forgetPassword_notExists@timberyard.com";
            bool result = UsersRepository.Users.ContainsKey(email);

            if (!result)
            {
                Assert.False(UsersRepository.Users.TryGetValue(email, out UserDTO user));
                var forget_result = await AuthenticationController.ForgetPassword(email);

                Assert.False(forget_result.Status);
                Assert.False(UsersRepository.Users.ContainsKey(email));
            }
        }

        #endregion
    }
}

