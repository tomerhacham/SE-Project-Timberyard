using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.AlarmsController
{
    public class AddNewAlarmRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new PartialAlarmModel()
            {
                Name = "Example Alarm",
                Objective = "X56868",
                Field = Domain.Business.Alarms.Field.Catalog,
                Threshold = 3,
                Receivers = new List<string>() { "zoee@post.bgu.ac.il", "hachamto@post.bgu.ac.il" }
            };
        }
    }
    public class AddNewAlarmResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new FullAlarmModel()
            {
                Id = 1,
                Name = "Example Alarm",
                Objective = "X56868",
                Field = Domain.Business.Alarms.Field.Catalog,
                Threshold = 3,
                Active = true,
                Receivers = new List<string>() { "zoee@post.bgu.ac.il", "hachamto@post.bgu.ac.il" }
            };
        }
    }
    public class FullAlarmRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new FullAlarmModel()
            {
                Id = 1,
                Name = "Example Alarm",
                Objective = "X56868",
                Field = Domain.Business.Alarms.Field.Catalog,
                Threshold = 3,
                Active = true,
                Receivers = new List<string>() { "zoee@post.bgu.ac.il", "hachamto@post.bgu.ac.il" }
            };
        }
    }
    public class GetAllAlarmsResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> {
                new FullAlarmModel()
                {
                    Id = 1,
                    Name = "Example Alarm 1",
                    Objective = "X56868",
                    Field = Domain.Business.Alarms.Field.Catalog,
                    Threshold = 3,
                    Active = true,
                    Receivers = new List<string>() { "zoee@post.bgu.ac.il", "hachamto@post.bgu.ac.il" }
                },
                new FullAlarmModel()
                {
                    Id = 2,
                    Name = "Example Alarm 2",
                    Objective = "L5",
                    Field = Domain.Business.Alarms.Field.Station,
                    Threshold = 10,
                    Active = true,
                    Receivers = new List<string>() { "zoee@post.bgu.ac.il", "hachamto@post.bgu.ac.il" }
                },

            };

            return records;
        }
    }
    public class AlarmToRemoveRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new AlarmToRemoveModel()
            {
                Id = 1
            };
        }
    }
}
