using FluentValidation;

namespace ws_with_repository_pattern.Infrastructures.Helper;

public class ValidationResult
{
    public bool IsPassed { get; set; } = false;
    public List<string> Errors { get; set; } = new List<string>();
}

public interface IValidator
{
    ValidationResult Validate();
}

public static class ValidatorHelperFactory {
    public static IValidator New<T>(T m, AbstractValidator<T> v)
    {
        return new ModelValidatorHelper<T>(m, v);
    }
}

public class ModelValidatorHelper<T>: IValidator
{
    private readonly  T _model;
    private readonly AbstractValidator<T> _validator;
    public ModelValidatorHelper(T model, AbstractValidator<T> validator)
    {
        _model = model;
        _validator = validator;
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();
        var validationResult = _validator.Validate(_model);
        result.IsPassed = validationResult.IsValid;
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                result.Errors.Add(error.ErrorMessage);
            }
        }

        return result;
    }
}