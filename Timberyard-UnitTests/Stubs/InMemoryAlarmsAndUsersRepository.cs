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
    public class InMemoryAlarmsAndUsersRepository : IAlarmsAndUsersRepository
    {
        public Dictionary<int, Alarm> Alarms { get; set; }
        public Dictionary<string, UserDTO> Users { get; set; }

        public InMemoryAlarmsAndUsersRepository()
        {
            Alarms = new Dictionary<int, Alarm>();
            Users = new Dictionary<string, UserDTO>();
        }

        #region Alarms

        public async Task<Result<List<Alarm>>> GetAllActiveAlarms()
        {
            var activeAlarms = Alarms.Values.Where(alarm => alarm.Active == true).ToList();
            return new Result<List<Alarm>>(true, activeAlarms);
        }

        public async Task<Result<List<Alarm>>> GetAllAlarms()
        {
            var allAlarms = Alarms.Values.ToList();
            return new Result<List<Alarm>>(true, allAlarms);
        }

        public async Task<Result<Alarm>> InsertAlarm(Alarm alarm)
        {
            var result = Alarms.TryAdd(alarm.Id, alarm);
            return new Result<Alarm>(result, alarm);
        }

        public async Task<Result<Alarm>> UpdateAlarm(Alarm alarm)
        {
            Alarms[alarm.Id] = alarm;
            return new Result<Alarm>(true, alarm);

        }

        public async Task<Result<bool>> DeleteAlarm(int Id)
        {
            var result = Alarms.Remove(Id);
            return new Result<bool>(result, result);

        }
        #endregion

        #region Authentication
        public async Task<Result<UserDTO>> GetUserRecord(string email)
        {
            var record = Users.TryGetValue(email, out UserDTO user);
            return new Result<UserDTO>(record, user);
        }

        public async Task<Result<bool>> UpdateUser(UserDTO record)
        {
            Users[record.Email] = record;
            return new Result<bool>(true, true);
        }

        public async Task<Result<bool>> AddUser(UserDTO record)
        {
            var result = Users.TryAdd(record.Email, record);
            return new Result<bool>(result, result);
        }

        public async Task<Result<bool>> RemoveUser(string email)
        {
            var result = Users.Remove(email);
            return new Result<bool>(result, result);
        }

        public async Task<Result<List<UserDTO>>> GetAllUsers()
        {
            var result = Users.Values.ToList();
            return new Result<List<UserDTO>>(true, result);
        }

        #endregion
    }
}
