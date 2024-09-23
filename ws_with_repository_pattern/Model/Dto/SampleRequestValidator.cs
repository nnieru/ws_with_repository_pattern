using FluentValidation;

namespace ws_with_repository_pattern.Model.Dto;

public class SampleRequestValidator: AbstractValidator<SampleRequestDto>
{
    public SampleRequestValidator()
    {
        RuleFor(s => s.Id).NotNull().MaximumLength(16).MinimumLength(5);
        RuleFor(s => s.Name).NotNull().MinimumLength(5);
    }
}