using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AcceptanceTests.Utils
{

    public class DatabaseSettings
    {
        public string DbServer { get; set; }
        public string DbPassword { get; set; }
        public string DbUsername { get; set; }
        public string ConnectionString { get; set; }
    }
    public class DatabaseUtils
    {
        private readonly string _connString;
        public DatabaseUtils(IOptions<DatabaseSettings> databaseSettings)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(databaseSettings.Value.ConnectionString)
            {
                DataSource = databaseSettings.Value.DbServer,
                Password = databaseSettings.Value.DbPassword,
                UserID = databaseSettings.Value.DbUsername
            };
            _connString = builder.ToString();
        }

        /// <summary>
        /// Util function to insert to the database regular user for test purposes only!
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddOrUpdateRegularUser(string email, string password, CancellationToken cancellationToken = new CancellationToken())
        {
            var sqlCommand = @" IF EXISTS (SELECT * FROM Users WHERE Email = @Email)
	                            UPDATE Users SET Password = @Password, ExpirationTimeStamp=@ExpirationTimeStamp;
                                ELSE
                                INSERT INTO Users (Email,Password,Role,ExpirationTimeStamp) values(@Email, @Password, 0 , @ExpirationTimeStamp)";

            try
            {
                await using (var dbConnection = new SqlConnection(_connString))
                {
                    await dbConnection.OpenAsync(cancellationToken);
                    await using var queryCommand = new SqlCommand(sqlCommand, dbConnection);
                    queryCommand.Parameters.AddWithValue("@Email", email);
                    queryCommand.Parameters.AddWithValue("@Password", password);
                    queryCommand.Parameters.AddWithValue("@ExpirationTimeStamp", DateTime.MaxValue.ToUniversalTime());
                    await queryCommand.ExecuteNonQueryAsync(cancellationToken);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }

}
