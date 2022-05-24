using WebService.API.Controllers.ModelValidation;

namespace WebService.API.Controllers.Models
{
    public class LoginModel
    {
        [ValidEmail]
        public string Email { get; set; }
        [StringIsNotNullOrEmpty]
        public string Password { get; set; }
    }
}
