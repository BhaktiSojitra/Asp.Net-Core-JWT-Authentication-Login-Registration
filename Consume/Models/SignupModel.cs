using System.ComponentModel.DataAnnotations;

namespace Consume.Models
{
    public class SignupModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "UserName is required.")]
        [MinLength(10, ErrorMessage = "UserName must be at least 10 characters long.")]
        [MaxLength(50, ErrorMessage = "UserName must not exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "UserName must not contain special characters. Only letters, numbers, and underscores are allowed.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).*$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number should be 10 digits.")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be numeric and exactly 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        public string Address { get; set; }

        public string Role { get; set; } = "";
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}
