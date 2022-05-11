﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.DataAccess;
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

        public async Task<Result<bool>> DeleteAlarm(int Id)
        {
            var result = Data.Remove(Id);
            return new Result<bool>(true, true);
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
    }
}