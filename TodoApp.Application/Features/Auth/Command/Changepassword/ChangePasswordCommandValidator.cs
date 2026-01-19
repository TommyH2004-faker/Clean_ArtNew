using FluentValidation;

namespace TodoApp.Application.Features.Auth.Command.Changepassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User ID is required");

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Old password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required")
                .MinimumLength(6)
                .WithMessage("New password must be at least 6 characters")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must be different from old password");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required")
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match");
        }
    }
}
