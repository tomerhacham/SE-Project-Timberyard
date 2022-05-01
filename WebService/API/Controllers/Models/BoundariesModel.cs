﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.ModelValidation;


namespace WebService.API.Controllers.Models
{
    public class BoundariesModel
    {
        [Required]
        [StringIsNotNullOrEmpty]
        public string Catalog { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateGreaterEqualThan("StartDate")]
        public DateTime EndDate { get; set; }
    }
}
