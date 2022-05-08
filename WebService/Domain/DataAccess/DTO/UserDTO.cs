using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.DataAccess.DTO
{
    public enum Role
    {
        RegularUser=0,
        Admin=1
    }

    [Table("Users")]

    public class UserDTO
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public DateTime ExperationTimeStamp { get; set; }

    }
}
