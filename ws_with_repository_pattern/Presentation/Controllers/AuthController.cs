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

        await _authenticationService.Register(request);
        
        return Ok();
    }

    [HttpPost]
    [Route("sign-in")]
    public async Task<IActionResult> Signin([FromBody] UserSignInRequestDto requestDto)
    {
       
        var result = await _authenticationService.SignIn(requestDto);
        return Ok(result);
        
       
    }
}