using System.ComponentModel.DataAnnotations;

namespace Consume.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
    public class JwtResponse
    {
        public string Token { get; set; }
        public string Expiration { get; set; }
    }
}
