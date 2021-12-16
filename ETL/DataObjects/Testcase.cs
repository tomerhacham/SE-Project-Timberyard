using ETL.Repository.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETL.DataObjects
{
    public class Testcase
    {
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

        public TestcaseDTO GetDTO()
        {
            return new TestcaseDTO()
            {
                Type = GetTestType(),
                Task = Task,
                Test = Test,
                Received = Received,
                Expected = Expected,
                Max = Max,
                Min = Min,
                Result = Result,
                TestName = TestName,
                DrationNet = DrationNet,
                DurationGross = DurationGross
            };
        }

        private string GetTestType()
        {
            return "someType";
        }
    }
}
