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
    public async Task<IActionResult> Signin([FromBody] UserSignInRequestDto requestDto)
    {
        try
        {
            var result = await _authenticationService.SignIn(requestDto);
            return Ok(result);
        }
        catch (Exception e)
        {
            if (e.Message == "Unauthorized")
            {
                return Unauthorized(e);
            }

            if (e.Message == "User Not Found")
            {
                return NotFound(e);
            }

            return StatusCode(500, e.Message);
        }
    }
}