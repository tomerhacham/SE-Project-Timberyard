using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess;
using WebService.Utils;
using WebService.Utils.ExtentionMethods;

namespace WebService.Domain.Business.Alarms
{
    public class AlarmsController
    {
        ISMTPClient SMTPClient { get; }
        ILogger Logger { get; }
        ILogsAndTestsRepository LogsAndTestsRepository { get; }
        IAlarmsAndUsersRepository AlarmsAndUsersRepository { get; }

        public AlarmsController(ISMTPClient smtpClient, ILogger logger, ILogsAndTestsRepository logsAndTestsRepository, IAlarmsAndUsersRepository alarmsAndUsersRepository)
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
            Result<Alarm> inputValidation = IsValidInputs(field, threshold, receivers);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
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
            Result<Alarm> inputValidation = IsValidInputs(alarmToEdit.Field, alarmToEdit.Threshold, alarmToEdit.Receivers);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            Result<Alarm> result = await AlarmsAndUsersRepository.UpdateAlarm(alarmToEdit);
            if (!result.Status)
            {
                Logger.Warning($"An error occurred while attempting to edit an Alarm. {result.Message}");
            }
            return result;
        }
        /// <summary>
        /// Removing alarm from the system
        /// </summary>
        /// <param name="alarmToRemove"></param>
        /// <returns></returns>
        public async Task<Result<bool>> RemoveAlarm(int Id)
        {
            Result<bool> result = await AlarmsAndUsersRepository.DeleteAlarm(Id);
            if (!result.Status)
            {
                Logger.Warning($"An error occurred while attempting to remove an Alarm. {result.Message}");
            }
            return result;
        }

        /// <summary>
        /// Returning all the system alarms
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<Alarm>>> GetAllAlarms()
        {
            return await AlarmsAndUsersRepository.GetAllAlarms();
        }

        /// <summary>
        /// Function to deserialize all active alarms from the DB and instate each one of them.
        /// Each alarm will be check for raise condition and trigger notification if needed
        /// </summary>
        public async Task<int> CheckForAlarmsCondition()
        {
            var activeAlarmsResult = await AlarmsAndUsersRepository.GetAllActiveAlarms();
            var activatedAlarms = 0;
            if (activeAlarmsResult.Status)
            {
                var currentTime = DateTime.UtcNow;
                var logsQueryResult = await LogsAndTestsRepository.GetAllLogsInTimeInterval(currentTime.AddHours(-24), currentTime);
                if (logsQueryResult.Status && logsQueryResult.Data.Count > 0)
                {

                    Parallel.ForEach(activeAlarmsResult.Data, async (Alarm alarm) =>
                    {
                        var result = await alarm.CheckCondition(logsQueryResult.Data, SMTPClient);
                        if (result)
                        {
                            Interlocked.Increment(ref activatedAlarms);
                        }
                    });
                }
            }
            return activatedAlarms;
        }

        private Result<Alarm> IsValidInputs(Field field, int threshold, List<string> receivers)
        {
            if (field < 0 || (int)field >= Enum.GetNames(typeof(Field)).Length)
            {
                Logger.Warning($"The field {field} is invalid", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Alarm>(false, null, "Invalid field\n");
            }
            if (threshold < 0)
            {
                Logger.Warning($"The threshold {threshold} is invalid. Threshold can not be a negative number", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Alarm>(false, null, "Invalid threshold. Threshold can not be a negative number\n");
            }
            if (!Extensions.IsValidEmail(receivers))
            {
                Logger.Warning("One or more of the entered emails are invalid", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Alarm>(false, null, "One or more of the entered emails are invalid\n");
            }
            return new Result<Alarm>(true, null, "All inputs are valid\n");
        }
    }
}
