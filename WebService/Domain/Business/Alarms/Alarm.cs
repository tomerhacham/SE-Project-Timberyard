using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public int Threshold { get; set; }
        public bool Active { get; set; }
        public List<string> Receivers { get; set; }

        public Alarm(string name, Field field, int threshold, bool active, List<string> receivers)
        {
            Name = name;
            Field = field;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        private Alarm(int id,string name, Field field, int threshold, bool active, List<string> receivers)
        {
            Id = id;
            Name = name;
            Field = field;
            Threshold = threshold;
            Active = active;
            Receivers = receivers;
        }
        public bool CheckCondition() { throw new NotImplementedException(); }

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
