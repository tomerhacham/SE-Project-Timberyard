using System;
using System.ComponentModel.DataAnnotations;
using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class DatesModel
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateGreaterEqualThanAttribute("StartDate")]
        public DateTime EndDate { get; set; }
    }
}
