using ETL.Repository.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Alarms;
using Xunit;

namespace Timberyard_UnitTests.UnitTests
{
    public class AlarmTests
    {
        [Fact]
        public async void CheckConditionTestCatalog()
        {
            var alarm = new Alarm("Test alarm", Field.Catalog, "TestCatalog", 1, true, new List<string>() { "test.tests@domain.com" });
            var lastRecords = new List<LogDTO>()
            {
                new LogDTO(){Catalog=}
            }
        }
    }
}
