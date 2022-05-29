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
    public class JWTtokenModel
    {
        [StringIsNotNullOrEmpty]
        public string Token { get; set; }
    }
}
