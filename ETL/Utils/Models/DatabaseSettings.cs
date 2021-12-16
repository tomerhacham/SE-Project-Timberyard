using System;
using System.Collections.Generic;
using System.Text;

namespace ETL.Utils.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DbServer { get; set; }
        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
    }
}
