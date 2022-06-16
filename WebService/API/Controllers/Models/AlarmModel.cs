using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebService.API.Controllers.ModelValidation;
using WebService.Domain.Business.Alarms;

namespace WebService.API.Controllers.Models
{
    public class PartialAlarmModel
    {
        [StringIsNotNullOrEmpty]
        public string? Name { get; set; }
        [StringIsNotNullOrEmpty]
        public string? Objective { get; set; }
        [EnumDataType(typeof(Field))]
        [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
        public Field Field { get; set; }
        [PositiveNumber]
        public int Threshold { get; set; }
        [ValidEmails]
        public List<string>? Receivers { get; set; }
    }
    public class FullAlarmModel
    {
        [Required]
        public int Id { get; set; }
        [StringIsNotNullOrEmpty]
        public string? Name { get; set; }
        [StringIsNotNullOrEmpty]
        public string? Objective { get; set; }
        [EnumDataType(typeof(Field))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public Field Field { get; set; }
        [PositiveNumber]
        public int Threshold { get; set; }
        [Required]
        public bool Active { get; set; }
        [ValidEmails]
        public List<string>? Receivers { get; set; }
    }

    public class AlarmToRemoveModel
    {
        [Required]
        public int Id { get; set; }
    }
}
