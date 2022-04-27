using ETL.DataObjects;
using ETL.Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace Timberyard_UnitTests.Stubs
{
    public class InMemoryLogsAndTestsRepository : ILogsAndTestsRepository
    {
        public Dictionary<int, Log> Data { get; set; }

        public InMemoryLogsAndTestsRepository()
        {
            Data = new Dictionary<int, Log>();
        }
        public async Task<Result<List<LogDTO>>> GetAllLogsInTimeInterval(DateTime startTime, DateTime endTime)
        {
            var logs = Data.Values.Where(log => log.Date <= endTime && log.Date >= startTime).Select(log => log.GetDTO()).ToList();
            return new Result<List<LogDTO>>(true, logs);
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(CardYield cardYield)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(StationsYield stationsYield)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(NoFailureFound noFailureFound)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(StationAndCardYield stationAndCardYield)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(CardTestDuration cardTestDuration)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<dynamic>>> ExecuteQuery(TesterLoad testerLoad)
        {
            throw new NotImplementedException();
        }
    }
}
