using ETL.Repository.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        //Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public Field Field { get; set; }
        public String Objective { get; set; }
        public int Threshold { get; set; }
        public bool Active { get; set; }
        public List<string> Receivers { get; set; }

        //Constructors
        public Alarm(string name, Field field, String objective, int threshold, bool active, List<string> receivers)
        {
            Name = name;
            Field = field;
            Objective = objective;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        internal Alarm(int id, string name, Field field, String objective, int threshold, bool active, List<string> receivers)
        {
            Id = id;
            Name = name;
            Field = field;
            Objective = objective;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }

        //Methods
        public async Task<bool> CheckCondition(List<LogDTO> logs, ISMTPClient iSMTPClient)
        {
            var raiseCondition = false;
            var count = 0;
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
            if (raiseCondition)
            {
                var message = GetAlarmMessage(DateTime.UtcNow.Date.ToShortDateString(), count);
                await iSMTPClient.SendEmail($"Alarm Notification - {Name}", message, Receivers);
            }
            return raiseCondition;
        }
        private string GetAlarmMessage(string date, int log_count)
        {
            var message = "Notice :\n";
            switch (Field)
            {
                case Field.Catalog:
                    message += $"Catalog {Objective} ";
                    break;

                case Field.Station:
                    message += $"Station {Objective} ";
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
        internal AlarmDTO GetDTO(bool attachId = false)
        {
            var dto = new AlarmDTO()
            {
                Id = attachId ? Id : default,
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
