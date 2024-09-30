using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Response;

namespace ws_with_repository_pattern.Application.Contract;

public interface IAuthenticationService
{
    Task  Register(UserRegistrationRequestDto requestDto);
    Task<BaseResponse<UserSignInResponseDto>>SignIn(UserSignInRequestDto request);

    Task UpdateUserAccess(UpdateUserAccessRequestDto request);
}