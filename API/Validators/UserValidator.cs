using API.Models;
using FluentValidation;

namespace API.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator() 
        {
            RuleFor(user => user.UserName)
                .NotNull().NotEmpty().WithMessage("UserName is required.")
                .MinimumLength(10).WithMessage("UserName must be at least 10 characters long.")
                .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("UserName must not contain special characters. " +
                        "Only letters, numbers, and underscores are allowed.");

            RuleFor(user => user.Password)
                .NotNull().NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(user => user.Email)
                .NotNull().NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(user => user.PhoneNumber)
                .NotNull().NotEmpty().WithMessage("PhoneNumber is required.")
                .Length(10).WithMessage("Phone number should be 10 digit");

            RuleFor(user => user.Address)
                .NotNull().NotEmpty().WithMessage("Address is required.")
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");
        }
    }
}
