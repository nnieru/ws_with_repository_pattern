using Binus.WS.Pattern.Service;
using Microsoft.AspNetCore.Mvc;
using Binus.WS.Pattern.Output;
using ws_with_repository_pattern.Common;
using ws_with_repository_pattern.Helper;
using ws_with_repository_pattern.Model.Dto;
using ws_with_repository_pattern.Output;
using ws_with_repository_pattern.Repository;

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
        // create handler
        var repo = new SampleRepository();
        var validator = ValidatorHelperFactory.New(requestDto, new SampleRequestValidator());
        var factory = SampleFactory.New(repo, validator);
        
        var result = factory.Handle(requestDto);
        if (result.ErrorMessages.Count > 0)
        {
            return StatusCode(400, result);
        }
        
        return StatusCode(200, result);
    }
    
}