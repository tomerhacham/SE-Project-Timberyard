using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        }

        public async Task<Result<JWTtoken>> Login(string email, string password)
        {
            Result<JWTtoken> CheckForDefaultSystemAdmin(string email, string password)
            {
                if (email.Equals(DefaultSystemAdmin.Email) && password.Equals(DefaultSystemAdmin.Password))
                {
                    return new Result<JWTtoken>(true, GenerateToken(new UserDTO { Email = DefaultSystemAdmin.Email, Role = Role.Admin }), "Login succees");
                }
                else
                {
                    return new Result<JWTtoken>(false, null);
                }
            }
            var isDefaultSysAdmin = CheckForDefaultSystemAdmin(email, password);
            //Default system admin is logging in
            if (isDefaultSysAdmin.Status)
            {
                return isDefaultSysAdmin;
            }

            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;

                bool condition = record.Role == Role.RegularUser ? DateTime.UtcNow.CompareTo(record.ExperationTimeStamp) < 0 : true;

                if (password.HashString().Equals(record.Password) && condition)
                {
                    JWTtoken token = GenerateToken(record);
                    return new Result<JWTtoken>(!token.Token.Equals(String.Empty), token, !token.Token.Equals(String.Empty) ? "Login success" : "Login failed");
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
                string verification_code = GenerateAndSendPassword(email, "verification code", "Timberyard authentication");
                record.Password = verification_code;
                record.ExperationTimeStamp = DateTime.UtcNow.AddMinutes(Settings.Value.Minutes);

                Result<bool> updateResult = await AlarmsAndUsersRepository.UpdateUser(record);

                return new Result<bool>(true, true, "Verification code send successfuly");
            }

            Logger.Warning(recordResult.Message);
            return new Result<bool>(false, false, recordResult.Message);
        }

        public async Task<Result<bool>> AddUser(string email)
        {
            // create new User
            UserDTO user = new UserDTO() { Email = email, Password = String.Empty, Role = Role.RegularUser, ExperationTimeStamp = DateTime.UtcNow };

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
            // remove user from database if exists
            await AlarmsAndUsersRepository.RemoveUser(newSystemAdminEmail);

            // generate new passord for admin and send to given email
            string tempPassword = GenerateAndSendPassword(newSystemAdminEmail, "temporary passord as system admin", "Timberyard system admin authentication");

            // create new system admin
            UserDTO user = new UserDTO() { Email = newSystemAdminEmail, Password = tempPassword, Role = Role.Admin, ExperationTimeStamp = DateTime.UtcNow };
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

            if (recordResult.Status && recordResult.Data.Role == Role.Admin)
            {
                UserDTO user = recordResult.Data;
                string tempPassword = GenerateAndSendPassword(email, "temporary passord", "Timberyard forget password authentication");
                user.Password = tempPassword;
                return await AlarmsAndUsersRepository.UpdateUser(user);
            }

            Logger.Warning("User doesn't exist");
            return new Result<bool>(false, false, "User doesn't exist");
        }

        internal JWTtoken GetToken(string email)
        {
            return GenerateToken(new UserDTO() { Email = email, Role = Role.Admin });
        }

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
                return new JWTtoken() { Token = string.Empty };
            }
        }

        public async Task<Result<List<UserDTO>>> GetAllUsers()
        {
            return await AlarmsAndUsersRepository.GetAllUsers();
        }

        private string GenerateAndSendPassword(string email, string msg_subject, string email_subject)
        {
            var random_number = new Random().Next(100000, 999999).ToString();
            SendPassword(email, random_number, msg_subject, email_subject);
            return random_number.HashString();
        }
        private void SendPassword(string email, string password, string msg_subject, string email_subject)
        {
            var message = $"Your {msg_subject} is {password} for Timberyard authentication.";
            Task.Run(async () => await SMTPClient.SendEmail(email_subject, message, new List<string>() { email }));
        }
    }
}
