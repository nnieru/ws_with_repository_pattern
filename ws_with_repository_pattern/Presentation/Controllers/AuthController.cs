using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
    [Route("update-access")]
    [Authorize(Roles = "administrator")]
    [Authorize("write")]
    public async Task<IActionResult> ChangeAccess([FromBody] UpdateUserAccessRequestDto requestDto)
    {
        await _authenticationService.UpdateUserAccess(requestDto);
        return Ok();
    }

    [HttpGet]
    [Route("my-roles")]
    [Authorize(Roles = "administrator")]
    [Authorize("read")]
    public async Task<IActionResult> GetMyRoles()
    {
        throw new NotImplementedException();
    }
}