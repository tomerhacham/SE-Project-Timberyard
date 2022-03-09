using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.DataAccess
{
    public class AlarmsAndUsersRepository
    {
        //Properties
        DatabaseSettings DatabaseSettings { get; }
        ILogger Logger { get; }

        //Constructor
        public AlarmsAndUsersRepository(IOptions<DatabaseSettings> databaseSettings, ILogger logger)
        {
            DatabaseSettings = databaseSettings.Value;
            Logger = logger;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DatabaseSettings.ConnectionString)
            {
                DataSource = DatabaseSettings.DbServer,
                Password = DatabaseSettings.DbPassword,
                UserID = DatabaseSettings.DbUsername
            };
            DatabaseSettings.ConnectionString = builder.ToString();
        }
    }
}
