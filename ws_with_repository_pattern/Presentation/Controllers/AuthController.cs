using Microsoft.AspNetCore.Mvc;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Infrastructures.Helper;

namespace ws_with_repository_pattern.Presentation.Controllers;

[ApiController]
[Route("auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    
    [HttpPost]
    [Route("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] UserRegistrationRequestDto request)
    {
        var validator = ValidatorHelperFactory.New(request, new UserRegistrationRequestValidator());
        var validationResult = validator.Validate();
        if (!validationResult.IsPassed)
        {
            return BadRequest(validationResult.Errors);
        }

        await _authenticationService.Register(request);
        
        return Ok();
    }

    [HttpPost]
    [Route("sign-in")]
    public IActionResult Signin()
    {
        throw new NotImplementedException();
    }
    
}