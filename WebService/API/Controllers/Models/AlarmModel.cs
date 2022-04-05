using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebService.API.Controllers.ModelValidation;
using WebService.Domain.Business.Alarms;

namespace WebService.API.Controllers.Models
{
    public class AlarmModel
    {
        public int Id { get; set; }
        [StringIsNotNullOrEmpty]
        public string Name { get; set; }
        [StringIsNotNullOrEmpty]
        public string Objective { get; set; }
        [EnumDataType(typeof(Field))]
        [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
        public Field Field { get; set; }
        [Required]
        public int Threshold { get; set; }
        [Required]
        public bool Active { get; set; }
        [ValidEmails]
        public List<string> Receivers { get; set; }
    }
}
