using ws_with_repository_pattern.Domain.Common;
using ws_with_repository_pattern.Domain.Model.Sample.Dto;
using ws_with_repository_pattern.Infrastructures.Presistance.Repository;
using ws_with_repository_pattern.Infrastructures.Presistance.UnitOfWorks;

namespace ws_with_repository_pattern.Application.Handler;

public interface ISampleHandler
{
    Result<SampleResponseDto> Handle(SampleRequestDto request);
}

public class SampleHandler: ISampleHandler
{
    private readonly SampleUnitOfWork _unitOfWork;
    private readonly IValidator _validator;

    public SampleHandler(SampleUnitOfWork unitOfWork, IValidator validator)
    {
        _unitOfWork = unitOfWork;
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

        var result = _unitOfWork.SampleRepositoryImp.GetAllSample();

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