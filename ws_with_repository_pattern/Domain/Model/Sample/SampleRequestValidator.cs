using FluentValidation;
using ws_with_repository_pattern.Domain.Model.Sample.Dto;

namespace ws_with_repository_pattern.Domain.Model.Sample;

public class SampleRequestValidator: AbstractValidator<SampleRequestDto>
{
    public SampleRequestValidator()
    {
        RuleFor(s => s.Id).NotNull().MaximumLength(16).MinimumLength(5);
        RuleFor(s => s.Name).NotNull().MinimumLength(5);
    }
}