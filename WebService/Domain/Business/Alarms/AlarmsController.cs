using ETL.Repository.DTO;
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

        public async Task<Result<Alarm>> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var newAlarm = new Alarm(name, field, objective, threshold, true, receivers);
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
        public async void CheckForAlarmsCondition()
        {
            // var latestLogs = await LogsAndTestsRepository.GetAllLogsInTimeInterval(currentTime.AddHours(-24), currentTime);
            #region CPS
            /*            var activeAlarmsResult = await AlarmsAndUsersRepository.GetAllActiveAlarms();
                        activeAlarmsResult.ContinueWith<List<Alarm>>(
                                success: async (List<Alarm> alarms) =>
                                {
                                    var currentTime = DateTime.Now;
                                    var logsQueryResult = await LogsAndTestsRepository.GetAllLogsInTimeInterval(currentTime.AddHours(-24), currentTime);
                                    logsQueryResult.ContinueWith<List<LogDTO>>(
                                           success: (List<LogDTO> latestLogs) =>
                                           {
                                               Parallel.ForEach(alarms, (Alarm alarm) => alarm.CheckCondition(latestLogs, SMTPClient));

                                           },
                                           fail: (List<LogDTO> _) =>
                                           {
                                               Logger.Warning(logsQueryResult.Message);
                                           });
                                }
                                , fail: (List<Alarm> _) => { Logger.Warning(activeAlarmsResult.Message); });*/
            #endregion

            var activeAlarmsResult = await AlarmsAndUsersRepository.GetAllActiveAlarms();
            if (activeAlarmsResult.Status)
            {
                var currentTime = DateTime.Now;
                var logsQueryResult = await LogsAndTestsRepository.GetAllLogsInTimeInterval(currentTime.AddHours(-24), currentTime);
                if (logsQueryResult.Status && logsQueryResult.Data.Count>0)
                {
                    Parallel.ForEach(activeAlarmsResult.Data, async (Alarm alarm) => alarm.CheckCondition(logsQueryResult.Data, SMTPClient));

                }
                else { Logger.Warning(logsQueryResult.Message); }
            }
            else { Logger.Warning(activeAlarmsResult.Message); }


        }
    }
}
