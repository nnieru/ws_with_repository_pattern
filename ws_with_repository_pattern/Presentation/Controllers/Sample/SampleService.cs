using Binus.WS.Pattern.Service;
using Microsoft.AspNetCore.Mvc;
using ws_with_repository_pattern.Domain.Common;
using ws_with_repository_pattern.Domain.Model.Sample;
using ws_with_repository_pattern.Domain.Model.Sample.Dto;
using ws_with_repository_pattern.Domain.Output;
using ws_with_repository_pattern.Infrastructures.Presistance.Repository;
using ws_with_repository_pattern.Infrastructures.Presistance.UnitOfWorks;

namespace ws_with_repository_pattern.Presentation.Controllers.Sample;

interface ISampleService
{
    IActionResult Test(SampleRequestDto requestDto);
}

[ApiController]
[Route("sample")]
public class SampleService: BaseService, ISampleService
{
    private readonly SampleUnitOfWork _unitOfWork;

  
    public SampleService(ILogger<BaseService> logger, SampleUnitOfWork unitOfWork
   ) : base(logger)
    {
        _unitOfWork = unitOfWork;

    }

    [HttpPost]
    [Route("test")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SampleOutput), StatusCodes.Status200OK)]
    public IActionResult Test(SampleRequestDto requestDto)
    {
        var validator = ValidatorHelperFactory.New(requestDto, new SampleRequestValidator());
        // create handler
        var factory = SampleFactory.New(_unitOfWork, validator);

        try
        {
            var result = factory.Handle(requestDto);
        
            if (result.ErrorMessages.Count > 0)
            {
                return StatusCode(400, result);
            }
        
            return StatusCode(200, result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
       
    }
}