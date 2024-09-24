using ws_with_repository_pattern.Common;
using ws_with_repository_pattern.Helper;
using ws_with_repository_pattern.Model.Dto;
using ws_with_repository_pattern.Repository;

namespace ws_with_repository_pattern.Handler;

public interface ISampleHandler
{
    Result<SampleResponseDto> Handle(SampleRequestDto request);
}

public class SampleHandler: ISampleHandler
{
    private readonly ISampleRepository _repository;
    private readonly IValidator _validator;

    public SampleHandler(ISampleRepository repository, IValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }
    
    public Result<SampleResponseDto> Handle(SampleRequestDto request)
    {
        var validation = _validator.Validate();
        
        if (!validation.IsPassed)
        {
            return new Result<SampleResponseDto>
            {
                ErrorMessages = validation.Errors
            };
        }

        var result = _repository.GetAllSample();

        if (result != null)
        {
            return new Result<SampleResponseDto>()
            {
                Data = new SampleResponseDto
                {
                    name = request.Name,
                    message = $"hello, {request.Name}"
                }
            };
        }

        return null;
    }
}