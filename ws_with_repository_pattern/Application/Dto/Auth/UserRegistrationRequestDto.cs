using FluentValidation;


namespace ws_with_repository_pattern.Application.Dto.Auth;

public class UserRegistrationRequestDto
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string confirmationPassword { get; set; }
    
    public string role_id { get; set; }
    public bool read { get; set; }
    public bool write { get; set; }
    public bool delete { get; set; }
}

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequestDto>
{
    public UserRegistrationRequestValidator()
    {
        RuleFor(x => x.username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.email).NotEmpty().EmailAddress();
        RuleFor(x => x.password).NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        RuleFor(x => x.confirmationPassword)
            .NotEmpty().WithMessage("Please confirm your password.")
            .Equal(x => x.password).WithMessage("Passwords do not match.");
    }
}