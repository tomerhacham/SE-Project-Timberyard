using Dapper.Contrib.Extensions;
using WebService.Domain.Business.Alarms;

namespace WebService.Domain.DataAccess.DTO
{
    [Table("Alarms")]
    public class AlarmDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Objective { get; set; }
        public Field Field { get; set; }
        public int Threshold { get; set; }
        public bool Active { get; set; }
        public string Receivers { get; set; }

    }
}
