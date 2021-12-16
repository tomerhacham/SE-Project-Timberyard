using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETL.Repository.DTO
{
    [Table("Tests")]
    public class TestcaseDTO
    {
        [Key]
        public int Id { get; set; }
        public int LogId { get; set; }
        public string Type { get; set; }
        public string Task { get; set; }
        public string Test { get; set; }
        public string Received { get; set; }
        public string Expected { get; set; }
        public double? Max { get; set; }
        public double? Min { get; set; }
        public string Result { get; set; }
        public string TestName { get; set; }
        public TimeSpan DrationNet { get; set; }
        public TimeSpan DurationGross { get; set; }
    }
}
