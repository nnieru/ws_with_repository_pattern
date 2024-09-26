using ws_with_repository_pattern.Application.Dto.Auth;

namespace ws_with_repository_pattern.Application.Contract;

public interface IAuthenticationService
{
    Task  Register(UserRegistrationRequestDto requestDto);
    Task<UserSignInResponseDto> SignIn(UserSignInRequestDto request);
}