using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Alarms
{
    public class AlarmsController
    {
        ISMTPClient SMTPClient { get; }
        ILogger Logger { get; }
        LogsAndTestsRepository LogsAndTestsRepository { get; }
        AlarmsAndUsersRepository AlarmsAndUsersRepository { get; }

        public AlarmsController(ISMTPClient smtpClient, ILogger logger, LogsAndTestsRepository logsAndTestsRepository, AlarmsAndUsersRepository alarmsAndUsersRepository)
        {
            SMTPClient = smtpClient;
            Logger = logger;
            LogsAndTestsRepository = logsAndTestsRepository;
            AlarmsAndUsersRepository = alarmsAndUsersRepository;
        }

        public async Task<Result<Alarm>> AddNewAlarm(string name, Field field, int threshold, List<string> receivers)
        {
            var newAlarm = new Alarm(name, field, threshold, true, receivers);
            return await AlarmsAndUsersRepository.InsertAlarm(newAlarm);
        }
        public async Task<Result<Alarm>> EditAlarm(Alarm alarmToEdit)
        {
            throw new NotImplementedException();
        }
        public async Task<Result<Alarm>> RemoveAlarm(Alarm alarmToRemove)
        {
            throw new NotImplementedException();

        }
        public void CheckForAlarmsCondition()
        {
            throw new NotImplementedException();
        }
    }
}
