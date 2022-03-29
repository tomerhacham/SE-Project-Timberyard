using Dapper.Contrib.Extensions;
using System;

namespace ETL.Repository.DTO
{
    [Table("Logs")]
    public class LogDTO
    {
        [Key]
        public int Id { get; set; }
        public string CardRev { get; set; }
        public string CardName { get; set; }
        public string SwRev { get; set; }
        public string DBRev { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan NetTime { get; set; }
        public string SN { get; set; }
        public string Catalog { get; set; }
        public string Station { get; set; }
        public string Operator { get; set; }
        public string DBMode { get; set; }
        public string ContinueOnFail { get; set; }
        public string TECHMode { get; set; }
        public string ABORT { get; set; }
        public string FinalResult { get; set; }

    }
}
