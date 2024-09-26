using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Application.Service;

public class AuthenticationService: IAuthenticationService
{

    private readonly IUserRepository _userRepository;
    
    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task  Register(UserRegistrationRequestDto requestDto)
    {
        try
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.password);
            var user = new User
            {
                email = requestDto.email,
                password_hash = hashedPassword,
                username = requestDto.username,
            };
            await _userRepository.InsertUser(user);
        }
        catch ( Exception e)
        {
            throw e;
        }
        
    }

    public Task<UserSignInResponseDto> SignIn(UserSignInRequestDto request)
    {
        throw new NotImplementedException();
    }
}