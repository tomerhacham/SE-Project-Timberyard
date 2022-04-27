﻿using Swashbuckle.AspNetCore.Filters;
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
}