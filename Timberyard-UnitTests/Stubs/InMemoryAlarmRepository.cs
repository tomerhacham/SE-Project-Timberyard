using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.DataAccess;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;

namespace Timberyard_UnitTests.Stubs
{
    public class InMemoryAlarmRepository : IAlarmsRepository
    {
        public Dictionary<int, Alarm> Data { get; set; }

        public InMemoryAlarmRepository()
        {
            Data = new Dictionary<int, Alarm>();
        }

        public async Task<Result<Alarm>> DeleteAlarm(Alarm alarm)
        {
            var result = Data.Remove(alarm.Id);
            return new Result<Alarm>(result, alarm);
        }

        public async Task<Result<List<Alarm>>> GetAllActiveAlarms()
        {
            var activeAlarms = Data.Values.Where(alarm => alarm.Active == true).ToList();
            return new Result<List<Alarm>>(true, activeAlarms);
        }

        public async Task<Result<List<Alarm>>> GetAllAlarms()
        {
            var allAlarms = Data.Values.ToList();
            return new Result<List<Alarm>>(true, allAlarms);
        }

        public async Task<Result<Alarm>> InsertAlarm(Alarm alarm)
        {
            var result = Data.TryAdd(alarm.Id, alarm);
            return new Result<Alarm>(result, alarm);
        }

        public async Task<Result<Alarm>> UpdateAlarm(Alarm alarm)
        {
            Data[alarm.Id] = alarm;
            return new Result<Alarm>(true, alarm);

        }

        public Task<Result<UserDTO>> GetUserRecord(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> UpdateUser(UserDTO record)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> AddUser(UserDTO record)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> RemoveUser(string email)
        {
            throw new NotImplementedException();
        }
    }
}
