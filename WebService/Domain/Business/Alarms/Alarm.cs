using ETL.Repository.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WebService.Domain.Business.Services;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;

namespace WebService.Domain.Business.Alarms
{
    public enum Field
    {
        Catalog,
        Station
    }

    public class Alarm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Field Field { get; set; }
        public String Objective { get; set; }
        public int Threshold { get; set; }
        public bool Active { get; set; }
        public List<string> Receivers { get; set; }

        public Alarm(string name, Field field, String objective, int threshold, bool active, List<string> receivers)
        {
            Name = name;
            Field = field;
            Objective = objective;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        private Alarm(int id, string name, Field field, String objective, int threshold, bool active, List<string> receivers)
        {
            Id = id;
            Name = name;
            Field = field;
            Objective = objective;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        public async void CheckCondition(List<LogDTO> logs, ISMTPClient iSMTPClient)
        {
            bool raiseCondition = false;
            int count = 0;
            switch (Field)
            {
                case Field.Catalog:
                    count = logs.Where(log => log.Catalog.Equals(Objective) && log.FinalResult.Equals("FAIL")).Count();
                    raiseCondition = count >= Threshold ? true : false;
                    break;
                case Field.Station:
                    count = logs.Where(log => log.Station.Equals(Objective) && log.FinalResult.Equals("FAIL")).Count();
                    raiseCondition = count >= Threshold ? true : false;
                    break;
            }

            string message = GetAlarmMessage(DateTime.Now.Date.ToShortDateString(), count);
            await iSMTPClient.SendEmail($"Alarm Notification - {Name}", message, Receivers);
        }

        private string GetAlarmMessage(string date, int log_count)
        {
            string message = "Notice :\n";
            switch (Field)
            {
                case Field.Catalog:
                    message += $"Catalog {Objective}";
                    break;

                case Field.Station:
                    message += $"Station {Objective}";
                    break;
            }
            message += $"has passed the Threshold ({Threshold})\n";
            message += $"Details :\nDate {date}\nNumber of faild tests are {log_count}.\n";
            return message;
        }

        public static Result<Alarm> ConstructFromDTO(AlarmDTO dto)
        {
            try
            {
                var receivers = JsonConvert.DeserializeObject<List<string>>(dto.Receivers);
                var alarm = new Alarm(dto.Id, dto.Name, dto.Field, dto.Objective, dto.Threshold, dto.Active, receivers);
                return new Result<Alarm>(true, alarm);
            }
            catch (Exception e)
            {
                return new Result<Alarm>(false, null, e.Message);
            }
        }

        internal AlarmDTO GetDTO()
        {
            var dto = new AlarmDTO()
            {
                Name = Name,
                Field = Field,
                Objective = Objective,
                Threshold = Threshold,
                Active = Active,
                Receivers = JsonConvert.SerializeObject(Receivers)
            };
            return dto;
        }

    }
}
