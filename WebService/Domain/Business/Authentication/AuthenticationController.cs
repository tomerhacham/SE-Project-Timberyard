using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;
using WebService.Utils.ExtentionMethods;
using WebService.Utils.Models;

namespace WebService.Domain.Business.Authentication
{
    public class AuthenticationController
    {
        ISMTPClient SMTPClient { get; }
        ILogger Logger { get; }
        IAlarmsAndUsersRepository AlarmsAndUsersRepository { get; }
        private readonly IOptions<AuthenticationSettings> Settings;
        private readonly DefaultSystemAdmin DefaultSystemAdmin;


        public AuthenticationController(ISMTPClient sMTPClient, ILogger logger, IAlarmsAndUsersRepository alarmsAndUsersRepository, IOptions<AuthenticationSettings> settings, IOptions<DefaultSystemAdmin> defaultSystemAdmin)
        {
            SMTPClient = sMTPClient;
            Logger = logger;
            AlarmsAndUsersRepository = alarmsAndUsersRepository;
            Settings = settings;
            DefaultSystemAdmin = defaultSystemAdmin.Value;
            var systemAdminRegistrationResult = AlarmsAndUsersRepository.UpdateOrInsert(new UserDTO { Email = DefaultSystemAdmin.Email, Password = DefaultSystemAdmin.Password.HashString(), Role = Role.Admin, ExpirationTimeStamp = DateTime.UtcNow }).Result;
            if (systemAdminRegistrationResult.Status)
            {
                Logger.Info($"Default system admin registration status:{systemAdminRegistrationResult.Message}", new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
            }
            else
            {
                Logger.Warning($"Default system admin registration status:{systemAdminRegistrationResult.Message}");
            }
        }

        /// <summary>
        /// Login to the system. the provided pasword will be validate with the persisted information regarding the user.
        /// If validation succeed a JWT token will be issued for the user.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Result<JWTtoken>> Login(string email, string password)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;

                bool condition = record.Role == Role.RegularUser ? DateTime.UtcNow.CompareTo(record.ExpirationTimeStamp) < 0 : true;

                if (password.HashString().Equals(record.Password) && condition)
                {
                    JWTtoken token = GenerateToken(record);
                    return new Result<JWTtoken>(!token.Token.Equals(String.Empty), token, !token.Token.Equals(String.Empty) ? "Login success" : "Login failed");
                }
                else
                {
                    Logger.Info($"User {email} tried to login with Incorrect Password");
                    return new Result<JWTtoken>(false, null, "Incorrect Password");
                }
            }

            Logger.Warning($"The users {email} attempt to login failed. {recordResult.Message}");
            return new Result<JWTtoken>(false, null, recordResult.Message);
        }

        /// <summary>
        /// Sends OTP (One Time Password) to the provided user if is listed as user in the system
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<bool>> RequestVerificationCode(string email)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;
                string verification_code = GenerateAndSendPassword(email, "verification code", "Timberyard authentication");
                record.Password = verification_code;
                record.ExpirationTimeStamp = DateTime.UtcNow.AddMinutes(Settings.Value.Minutes);

                Result<bool> updateResult = await AlarmsAndUsersRepository.UpdateUser(record);
                if (!updateResult.Status)
                {
                    Logger.Warning($"An attempt to update the user {email} with a new verification code failed. {updateResult.Message}");
                    return updateResult;
                }
                return new Result<bool>(true, true, "Verification code send successfuly");
            }

            Logger.Warning($"The users {email} attempt to request for a verification code failed. {recordResult.Message}");
            return new Result<bool>(false, false, recordResult.Message);
        }

        /// <summary>
        /// Adding the provided email as regular user to the system.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<bool>> AddUser(string email)
        {
            // create new User
            UserDTO user = new UserDTO() { Email = email, Password = String.Empty, Role = Role.RegularUser, ExpirationTimeStamp = DateTime.UtcNow };

            Result<bool> result = await AlarmsAndUsersRepository.AddUser(user);
            if (!result.Status)
            {
                Logger.Warning($"An error occurred while attempting to add the user {email}. {result.Message}");
            }
            return result;
        }

        /// <summary>
        /// Removing the provided email address from the system's users
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<bool>> RemoveUser(string email)
        {
            Result<bool> result = await AlarmsAndUsersRepository.RemoveUser(email);
            if (!result.Status)
            {
                Logger.Warning($"An error occurred while attempting to remove the user {email}. {result.Message}");
            }
            return result;
        }

        /// <summary>
        /// Change users password in case the provided oldPassword matches the exisitng one
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
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

                Logger.Warning($"User {email} password cannot be updated since the old password entered is incorrect");
                return new Result<bool>(false, false, "User password don't match");
            }

            Logger.Warning($"An error occurred while attempting to change password for user {email}. {record.Message}");
            return new Result<bool>(false, false, "User doesn't exist");
        }

        /// <summary>
        /// Adding new system admin user for the provided email
        /// </summary>
        /// <param name="newSystemAdminEmail"></param>
        /// <returns></returns>
        public async Task<Result<bool>> AddSystemAdmin(string newSystemAdminEmail)
        {
            // remove user from database if exists
            await AlarmsAndUsersRepository.RemoveUser(newSystemAdminEmail);

            // generate new passord for admin and send to given email
            string tempPassword = GenerateAndSendPassword(newSystemAdminEmail, "temporary passord as system admin", "Timberyard system admin authentication");

            // create new system admin
            UserDTO user = new UserDTO() { Email = newSystemAdminEmail, Password = tempPassword, Role = Role.Admin, ExpirationTimeStamp = DateTime.UtcNow };
            Result<bool> result = await AlarmsAndUsersRepository.AddUser(user);
            if (!result.Status)
            {
                Logger.Warning($"An error occurred while attempting to add a new system admin with email {newSystemAdminEmail}. {result.Message}");
            }
            return result;
        }

        /// <summary>
        /// Reset the provided email password and send it via SMTP
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<bool>> ForgetPassword(string email)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);

            if (recordResult.Status && recordResult.Data.Role == Role.Admin)
            {
                UserDTO user = recordResult.Data;
                string tempPassword = GenerateAndSendPassword(email, "temporary password", "Timberyard forget password authentication");
                user.Password = tempPassword;
                return await AlarmsAndUsersRepository.UpdateUser(user);
            }

            Logger.Warning($"The users {email} attempt to get a new password failed. {recordResult.Message}");
            return new Result<bool>(false, false, "User doesn't exist");
        }

        internal JWTtoken GetToken(string email)
        {
            return GenerateToken(new UserDTO() { Email = email, Role = Role.Admin });
        }

        /// <summary>
        /// Generate JWT token with digital signature and user's claims
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private JWTtoken GenerateToken(UserDTO record)
        {
            try
            {
                // generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Settings.Value.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim("Email", record.Email),
                    new Claim("Role", record.Role.ToString())
                }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var strToken = tokenHandler.WriteToken(token);
                return new JWTtoken() { Token = strToken };
            }
            catch (Exception exception)
            {
                Logger.Warning($"An error occurred while attempting to generate a token for {record.Email}", exception, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new JWTtoken() { Token = string.Empty };
            }
        }

        public async Task<Result<List<UserDTO>>> GetAllUsers()
        {
            return await AlarmsAndUsersRepository.GetAllUsers();
        }

        /// <summary>
        /// Util function to generate random number and sent it via email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="msg_subject"></param>
        /// <param name="email_subject"></param>
        /// <returns></returns>
        private string GenerateAndSendPassword(string email, string msg_subject, string email_subject)
        {
            var random_number = new Random().Next(100000, 999999).ToString();
            SendPassword(email, random_number, msg_subject, email_subject);
            return random_number.HashString();
        }
        /// <summary>
        /// Util function to build email message and send it as backgorund task
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="msg_subject"></param>
        /// <param name="email_subject"></param>
        private void SendPassword(string email, string password, string msg_subject, string email_subject)
        {
            var message = $"Your {msg_subject} is {password} for Timberyard authentication.";
            Task.Run(async () => await SMTPClient.SendEmail(email_subject, message, new List<string>() { email }));
        }
    }
}