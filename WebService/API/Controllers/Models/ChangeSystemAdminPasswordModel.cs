using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class ChangeSystemAdminPasswordModel
    {
        [StringIsNotNullOrEmpty]
        public string? OldPassword { get; set; }
        [StringIsNotNullOrEmpty]
        public string? NewPassword { get; set; }
    }
}
