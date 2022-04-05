using ETL.Repository.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private Alarm(int id,string name, Field field, String objective, int threshold, bool active, List<string> receivers)
        {
            Id = id;
            Name = name;
            Field = field;
            Objective = objective;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        public bool CheckCondition(List<LogDTO> logs, ISMTPClient iSMTPClient) 
        {
            int log_count = 0;
            bool condition = false;
            foreach(LogDTO log in logs)
            {
                if((Field == Field.Catalog && log.Catalog == Objective) ||
                    (Field == Field.Station && log.Station == Objective))
                {
                    log_count++;
                }
            }

            if(log_count >= Threshold)
            {
                condition = true;
                String subject = "Alarm Notification - " + Name;
                String message = GetAlarmMessage(logs, log_count);
                iSMTPClient.SendEmail(subject, message, Receivers);
            }

            return condition;
        }

        private String GetAlarmMessage(List<LogDTO> logs, int log_count)
        {
            LogDTO log = logs[0];
            String message = "Notice :\n";
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
            message += $"Details :\nDate {log.Date}\nNumber of faild tests are {log_count}.\n";
            return message;
        }

        public static Result<Alarm> ConstructFromDTO(AlarmDTO dto)
        {
            try
            {
                var receivers = JsonConvert.DeserializeObject<List<string>>(dto.Receivers);
                var alarm = new Alarm(dto.Id, dto.Name, dto.Field, dto.Threshold, dto.Active, receivers);
                return new Result<Alarm>(true, alarm);
            }
            catch(Exception e) 
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
                Threshold = Threshold,
                Active = Active,
                Receivers = JsonConvert.SerializeObject(Receivers)
            };
            return dto;
        }

    }
}
