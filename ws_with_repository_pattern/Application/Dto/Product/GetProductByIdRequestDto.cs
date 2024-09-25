using FluentValidation;

namespace ws_with_repository_pattern.Application.Dto.Product;

public class GetProductByIdRequestDto
{
    public string id { get; set; }
}

public class ProductIdRequestValidator : AbstractValidator<GetProductByIdRequestDto>
{
    public ProductIdRequestValidator()
    {
        RuleFor(x => x.id).NotEmpty().MinimumLength(8);
    }
}