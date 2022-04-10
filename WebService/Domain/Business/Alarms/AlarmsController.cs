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

        /// <summary>
        /// Adding new alarm to the sysmten
        /// </summary>
        /// <param name="name">Name of the alarm</param>
        /// <param name="field">The field which the alarm monitors</param>
        /// <param name="objective">Name of the field instance e.g for Catalog 'x-123df' or Station 'L1'</param>
        /// <param name="threshold">Threshold for the alarm to be set on</param>
        /// <param name="receivers">List of email addresses to recieve the alarm notification</param>
        /// <returns></returns>
        public async Task<Result<Alarm>> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var newAlarm = new Alarm(name, field, objective, threshold, true, receivers);
            return await AlarmsAndUsersRepository.InsertAlarm(newAlarm);
        }
        /// <summary>
        /// Editing any property of an exisitng alarm
        /// </summary>
        /// <param name="alarmToEdit"></param>
        /// <returns></returns>
        public async Task<Result<Alarm>> EditAlarm(Alarm alarmToEdit)
        {
            return await AlarmsAndUsersRepository.UpdateAlarm(alarmToEdit);
        }
        /// <summary>
        /// Removing alarm from the system
        /// </summary>
        /// <param name="alarmToRemove"></param>
        /// <returns></returns>
        public async Task<Result<Alarm>> RemoveAlarm(Alarm alarmToRemove)
        {
            return await AlarmsAndUsersRepository.DeleteAlarm(alarmToRemove);

        }
        /// <summary>
        /// Function to deserialize all active alarms from the DB and instate each one of them.
        /// Each alarm will be check for raise condition and trigger notification if needed
        /// </summary>
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
                if (logsQueryResult.Status && logsQueryResult.Data.Count > 0)
                {
                    Parallel.ForEach(activeAlarmsResult.Data, async (Alarm alarm) => alarm.CheckCondition(logsQueryResult.Data, SMTPClient));

                }
                else { Logger.Warning(logsQueryResult.Message); }
            }
            else { Logger.Warning(activeAlarmsResult.Message); }


        }
    }
}
