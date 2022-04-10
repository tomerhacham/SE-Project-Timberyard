using ETL.Repository.DTO;
using System;
using System.Collections.Generic;

namespace ETL.DataObjects
{
    public class Log
    {
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
        public List<Testcase> TESTS { get; set; }

        public LogDTO GetDTO()
        {
            var startTime = Date.Add(StartTime);
            var endTime = EndTime.CompareTo(StartTime) < 0 ? Date.AddDays(1).Add(EndTime) : Date.Add(EndTime);
            return new LogDTO()
            {
                CardRev = CardRev,
                CardName = CardName,
                SwRev = SwRev,
                DBRev = DBRev,
                Date = Date,
                StartTime = startTime,
                EndTime = endTime,
                NetTime = NetTime,
                SN = SN,
                Catalog = Catalog,
                Station = Station,
                Operator = Operator,
                DBMode = DBMode,
                ContinueOnFail = ContinueOnFail,
                TECHMode = TECHMode,
                ABORT = ABORT,
                FinalResult = FinalResult,
            };
        }
    }
}
