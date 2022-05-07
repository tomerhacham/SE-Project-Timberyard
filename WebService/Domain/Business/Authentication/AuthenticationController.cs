﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;
using WebService.Utils.ExtentionMethods;

namespace WebService.Domain.Business.Authentication
{
    public class AuthenticationController
    {
        ISMTPClient SMTPClient { get; }
        ILogger Logger { get; }
        IAlarmsRepository AlarmsAndUsersRepository { get; }

        public AuthenticationController(ISMTPClient sMTPClient, ILogger logger, IAlarmsRepository alarmsAndUsersRepository)
        {
            SMTPClient = sMTPClient;
            Logger = logger;
            AlarmsAndUsersRepository = alarmsAndUsersRepository;
        }

        public async Task<Result<JWTtoken>> Login(string email, string password)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;

                bool condition = record.Role == Role.RegularUser ? DateTime.Now.CompareTo(record.ExperationTimeStamp) < 0 : true;

                if (password.HashString().Equals(record.Password) && condition)
                {
                    JWTtoken token = GenerateToken(record);
                    return new Result<JWTtoken>(true, token, "Login success");
                }
                else
                {
                    Logger.Warning($"User {email} tried to login with Incorrect Password");
                    return new Result<JWTtoken>(false, null, "Incorrect Password");
                }
            }

            Logger.Warning(recordResult.Message);
            return new Result<JWTtoken>(false, null, recordResult.Message);
        }

        public async Task<Result<bool>> RequestVerificationCode(string email)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;
                var random_number = new Random().Next(100000, 999999).ToString();
                var message = $"Use verification code {random_number} for Timberyard authentication";
                Task.Run(async () => await SMTPClient.SendEmail("Timberyard authentication", message, new List<string>() { record.Email }));
                record.Password = random_number.HashString();
                record.ExperationTimeStamp = DateTime.Now.AddMinutes(5);

                Result<bool> updateResult = await AlarmsAndUsersRepository.UpdateUser(record);

                return new Result<bool>(true, true, "Verification code send successfuly");
            }

            Logger.Warning(recordResult.Message);
            return new Result<bool>(false, false, recordResult.Message);
        }

        public async Task<Result<bool>> AddUser(string email)
        {
            // create new User
            UserDTO user = new UserDTO() { Email = email, Role = Role.RegularUser };

            Result<bool> result = await AlarmsAndUsersRepository.AddUser(user);
            if (!result.Status)
            {
                Logger.Warning(result.Message);
            }
            return result;
        }

        public async Task<Result<bool>> RemoveUser(string email)
        {
            Result<bool> result = await AlarmsAndUsersRepository.RemoveUser(email);
            if (!result.Status)
            {
                Logger.Warning(result.Message);
            }
            return result;
        }

        public async Task<Result<bool>> ChangeSystemAdminPassword(string email, string newPassword, string oldPassword)
        {
            Result<UserDTO> record = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (record.Status)
            {
                UserDTO user = record.Data;
                if (user.Password == oldPassword.HashString())
                {
                    user.Password = newPassword.HashString();
                    return await AlarmsAndUsersRepository.UpdateUser(user);
                }

                Logger.Warning("User password don't match");
                return new Result<bool>(false, false, "User password don't match");
            }

            Logger.Warning("User doesn't exist");
            return new Result<bool>(false, false, "User doesn't exist");
        }

        public async Task<Result<bool>> AddSystemAdmin(string newSystemAdminEmail)
        {
            // create new User
            var random_number = new Random().Next(100000, 999999).ToString();
            var message = $"You added as system admin on Timberyard ! your temporary passord is {random_number} for Timberyard authentication.";
            Task.Run(async () => await SMTPClient.SendEmail("Timberyard system admin authentication", message, new List<string>() { newSystemAdminEmail }));
            string tempPassword = random_number.HashString();

            UserDTO user = new UserDTO(newSystemAdminEmail, tempPassword);
            Result<bool> result = await AlarmsAndUsersRepository.AddUser(user);
            if (!result.Status)
            {
                Logger.Warning(result.Message);
            }
            return result;
        }
        public async Task<Result<bool>> ForgetPassword(string email)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);

            if (recordResult.Status)
            {
                UserDTO user = recordResult.Data;
                var random_number = new Random().Next(100000, 999999).ToString();
                var message = $"Your temporary passord is {random_number} for Timberyard authentication.";
                Task.Run(async () => await SMTPClient.SendEmail("Timberyard forget password authentication", message, new List<string>() { email }));
                user.Password = random_number.HashString();
                return await AlarmsAndUsersRepository.UpdateUser(user);
            }

            Logger.Warning("User doesn't exist");
            return new Result<bool>(false, false, "User doesn't exist");
        }

        private JWTtoken GenerateToken(DataAccess.DTO.UserDTO record)
        {
            throw new NotImplementedException();
        }


    }
}
