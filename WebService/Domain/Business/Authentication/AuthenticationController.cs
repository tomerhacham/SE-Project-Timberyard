using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Utils;

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

        public async Task<Result<JWTtoken>> SystemAdminLogin(string email, string password)
        {
            var recordResult = await AlarmsAndUsersRepository.GetUserRecord(email);
            if (recordResult.Status)
            {
                var record = recordResult.Data;
                var sha256 = new SHA256CryptoServiceProvider();
                var hash_pass = sha256.ComputeHash(Encoding.ASCII.GetBytes(record.Password));

                if (password.Equals(Encoding.ASCII.GetString(hash_pass)))
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
                var random_number = new Random().Next(100000, 999999).ToString();
                var message = $"Use verification code {random_number} for Timberyard authentication";
                Task.Run(async () => await SMTPClient.SendEmail("Timberyard authentication", message, new List<string>() { recordResult.Data.Email }));
                return new Result<bool>(true, true, "Verification code send successfuly");
            }

            Logger.Warning(recordResult.Message);
            return new Result<bool>(false, false, recordResult.Message);
        }

        private JWTtoken GenerateToken(DataAccess.DTO.UserDTO record)
        {
            throw new NotImplementedException();
        }
    }
}
