using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class EmailModel
    {
        [ValidEmail]
        public string Email { get; set; }
    }

    public class UserChangeSystemAdminPasswordModel
    {
        [StringIsNotNullOrEmpty]
        public string OldPassword { get; set; }
        [StringIsNotNullOrEmpty]
        public string NewPassword { get; set; }
    }
}
