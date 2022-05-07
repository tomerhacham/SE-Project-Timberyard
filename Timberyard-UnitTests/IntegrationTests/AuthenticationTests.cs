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

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class AuthenticationTests : TestSuit
    {
        Mock<ISMTPClient> SmtpClient { get; set; }
        AuthenticationController AuthenticationController { get; set; }
        InMemoryAlarmsAndUsersRepository UsersRepository { get; set; }
        public AuthenticationTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests", inMemoryAlarmsRepository: true, inMemoryLogsAndTestRepository: true);
            AuthenticationController = serviceProvider.GetService<AuthenticationController>();
            UsersRepository = serviceProvider.GetService<IAlarmsAndUsersRepository>() as InMemoryAlarmsAndUsersRepository;
        }

        #region CRUD Alarms Scenarios
        [Fact]
        public async void AddUser()
        {
            string email = "addUserTest@timberyard.com";
            var result = await AuthenticationController.AddUser(email);
            Assert.True(result.Status);
            Assert.NotEmpty(UsersRepository.Users);
            Assert.Single(UsersRepository.Users);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(UsersRepository.Users[email].Role, Role.RegularUser);
            Assert.Equal("integrationtest@timberyard.com", UsersRepository.Users.Keys.First());
        }

        [Fact]
        public async void RemoveUser()
        {
            int totalUsers = 3;
            for (int i = 1; i <= totalUsers; i++)
            {
                var insert_result = await AuthenticationController.AddUser($"TestUser{i}@timberyard.com");
                Assert.True(insert_result.Status);
                Assert.Equal(i, UsersRepository.Users.Count);
            }

            Assert.Equal(totalUsers, UsersRepository.Users.Count);
            // Add and Remove user from exists repository
            string emailToRemove = "userToRemove@timberyard.com";
            var remove_result = await AuthenticationController.RemoveUser(emailToRemove);

            Assert.True(remove_result.Status);
            Assert.False(UsersRepository.Users.ContainsKey(emailToRemove));
            Assert.Equal(totalUsers, UsersRepository.Users.Count);
            Assert.False(UsersRepository.Users.TryGetValue(emailToRemove, out UserDTO user));
        }

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
        public async void AddSystemAdmin()
        {
            string email = "addSystemAdminTest@timberyard.com";
            var result = await AuthenticationController.AddSystemAdmin(email);
            Assert.True(result.Status);
            Assert.NotEmpty(UsersRepository.Users);
            Assert.Single(UsersRepository.Users);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));
            Assert.Equal(Role.Admin, UsersRepository.Users[email].Role);
            Assert.NotEmpty(UsersRepository.Users[email].Password);
            Assert.Equal("integrationtest@timberyard.com", UsersRepository.Users.Keys.First());
        }

        [Fact]
        public async void ChangeSystemAdminPassword()
        {
            string email = "ChangeSystemAdminPasswordTest@timberyard.com";
            var result = await AuthenticationController.AddSystemAdmin(email);
            Assert.True(result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO userAdded));

            string pass = UsersRepository.Users[email].Password;
            var update_result = await AuthenticationController.ChangeSystemAdminPassword(email, "TestPass", pass);

            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO userEdited));
            Assert.NotNull(userEdited);
            Assert.Equal("TestPass", userEdited.Password);
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

        #endregion

        #region Login and Password
        [Fact]
        public async void Login()
        {
        }

        [Fact]
        public async void Login_worngPass()
        {
        }

        [Fact]
        public async void RequestVerificationCode()
        {
        }

        [Fact]
        public async void ForgetPassword()
        {
            string email = "forgetPassword@timberyard.com";
            var insert_result = await AuthenticationController.AddUser(email);
            Assert.True(insert_result.Status);
            Assert.True(UsersRepository.Users.TryGetValue(email, out UserDTO user));

            var result = await AuthenticationController.ForgetPassword(email);
            Assert.True(result.Status);
            Assert.True(UsersRepository.Users.ContainsKey(email));
            Assert.NotEqual(user.Password, UsersRepository.Users[email].Password);
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

