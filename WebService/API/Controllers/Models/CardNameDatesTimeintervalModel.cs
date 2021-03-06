using System;
using System.ComponentModel.DataAnnotations;
using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class CardNameDatesTimeintervalModel
    {
        [StringIsNotNullOrEmpty]
        public string? CardName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateGreaterEqualThan("StartDate")]
        public DateTime EndDate { get; set; }

        [PositiveNumber]
        public int TimeInterval { get; set; }
    }
}
