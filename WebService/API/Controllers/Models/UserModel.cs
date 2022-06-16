using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class UserModel
    {
        [ValidEmail]
        public string? Email { get; set; }
        [ValidRoleNumber]
        public int Role { get; set; }
    }
}
