using Binus.WS.Pattern.Service;
using Microsoft.AspNetCore.Mvc;
using Binus.WS.Pattern.Output;
using FluentValidation;
using ws_with_repository_pattern.Common;
using ws_with_repository_pattern.DbContext;
using ws_with_repository_pattern.Helper;
using ws_with_repository_pattern.Model.Dto;
using ws_with_repository_pattern.Output;
using ws_with_repository_pattern.Repository;
using IValidator = ws_with_repository_pattern.Common.IValidator;

namespace ws_with_repository_pattern.Services;

interface ISampleService
{
    IActionResult Test(SampleRequestDto requestDto);
}

[ApiController]
[Route("sample")]
public class SampleService: BaseService, ISampleService
{
    private readonly ISampleRepository _repository;

  
    public SampleService(ILogger<BaseService> logger,    ISampleRepository repository
   ) : base(logger)
    {
        _repository = repository;

    }

    [HttpPost]
    [Route("test")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SampleOutput), StatusCodes.Status200OK)]
    public IActionResult Test(SampleRequestDto requestDto)
    {
        var validator = ValidatorHelperFactory.New(requestDto, new SampleRequestValidator());
        // create handler
        var factory = SampleFactory.New(_repository, validator);

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