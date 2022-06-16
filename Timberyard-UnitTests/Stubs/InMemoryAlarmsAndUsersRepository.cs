using System.Collections.Generic;
using System.Linq;
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

        public Task<Result<List<Alarm>>> GetAllActiveAlarms()
        {
            var activeAlarms = Alarms.Values.Where(alarm => alarm.Active == true).ToList();
            return Task.FromResult(new Result<List<Alarm>>(true, activeAlarms));
        }

        public Task<Result<List<Alarm>>> GetAllAlarms()
        {
            var allAlarms = Alarms.Values.ToList();
            return Task.FromResult(new Result<List<Alarm>>(true, allAlarms));
        }

        public Task<Result<Alarm>> InsertAlarm(Alarm alarm)
        {
            var result = Alarms.TryAdd(alarm.Id, alarm);
            return Task.FromResult(new Result<Alarm>(result, alarm));
        }

        public Task<Result<Alarm>> UpdateAlarm(Alarm alarm)
        {
            Alarms[alarm.Id] = alarm;
            return Task.FromResult(new Result<Alarm>(true, alarm));

        }

        public Task<Result<bool>> DeleteAlarm(int Id)
        {
            var result = Alarms.Remove(Id);
            return Task.FromResult(new Result<bool>(result, result));

        }
        #endregion

        #region Authentication
        public Task<Result<UserDTO>> GetUserRecord(string email)
        {
            var record = Users.TryGetValue(email, out UserDTO user);
            return Task.FromResult(new Result<UserDTO>(record, user));
        }

        public Task<Result<bool>> UpdateUser(UserDTO record)
        {
            Users[record.Email] = record;
            return Task.FromResult(new Result<bool>(true, true));
        }

        public Task<Result<bool>> AddUser(UserDTO record)
        {
            var result = Users.TryAdd(record.Email, record);
            return Task.FromResult(new Result<bool>(result, result));
        }

        public Task<Result<bool>> RemoveUser(string email)
        {
            var result = Users.Remove(email);
            return Task.FromResult(new Result<bool>(result, result));
        }

        public Task<Result<List<UserDTO>>> GetAllUsers()
        {
            var result = Users.Values.ToList();
            return Task.FromResult(new Result<List<UserDTO>>(true, result));
        }

        public async Task<Result<bool>> UpdateOrInsert(UserDTO user)
        {
            var insertResult = await AddUser(user);
            if (!insertResult.Status)
            {
                return await UpdateUser(user);
            }
            else
            {
                return insertResult;
            }
        }

        #endregion
    }
}
