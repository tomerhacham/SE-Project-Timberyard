using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.DataAccess.DTO
{
    public enum Role
    {
        Admin,
        RegularUser
    }
    public class UserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
