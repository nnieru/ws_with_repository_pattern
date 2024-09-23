using Binus.WS.Pattern.Service;
using Microsoft.AspNetCore.Mvc;
using Binus.WS.Pattern.Output;
using ws_with_repository_pattern.Helper;
using ws_with_repository_pattern.Model.Dto;
using ws_with_repository_pattern.Output;

namespace ws_with_repository_pattern.Services;

interface ISampleService
{
    IActionResult Test(SampleRequestDto requestDto);
}

[ApiController]
[Route("sample")]
public class SampleService: BaseService, ISampleService
{
    public SampleService(ILogger<BaseService> logger) : base(logger)
    {
    }

    [HttpPost]
    [Route("test")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SampleOutput), StatusCodes.Status200OK)]
    public IActionResult Test(SampleRequestDto requestDto)
    {
        var validator = ValidatorHelperFactory.New<SampleRequestDto>(requestDto, new SampleRequestValidator());
        var validation = validator.Validate();
        
        if (!validation.IsPassed)
        {
            return StatusCode(400,validation.Errors);
        }
        return StatusCode(200,"oke");
    }
    
}