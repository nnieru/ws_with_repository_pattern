using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Auth;
using ws_with_repository_pattern.Application.Exception;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.Entity;
using ws_with_repository_pattern.Infrastructures.Helper;
using ws_with_repository_pattern.Response;

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
        catch ( System.Exception e)
        {
            throw e;
        }
        
    }

    public async Task<BaseResponse<UserSignInResponseDto>> SignIn(UserSignInRequestDto request)
    {
        var user = await _userRepository.GetUser(request.email);
        
        if (user == null)
        {
            throw new UserNotFoundException("User Not Found");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.password, user.password_hash))
        {
            throw new UnauthorizedAccessException();
        }
        
        var roleMapping = await _userRepository.GetUserRoles(request.email);
        var masterRole = await _userRepository.GetMasterRoles();

        var userRole = (from a in roleMapping
            join b in masterRole on a.roleId equals b.id
            select b.name ).ToList();

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user?.username),
            new Claim(ClaimTypes.Email, user?.email),
        };

        foreach (var role in userRole)
        {
            authClaims.Add(
                new Claim(ClaimTypes.Role, role)
                );
        }

        var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1eWZ#^7A$Uzp3MCzG0l9&2@Rj^qJ!nLt"));

        var token = new JwtSecurityToken(
            issuer: "test-dev",
            audience: "test-dev",
            expires: DateTime.Now.AddHours(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
        );
        
        
        
        var userSiginInResponse = new UserSignInResponseDto
        {
            id = user.id,
            username = user.username,
            email = user.email,
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        };

        return ResponseMapper<UserSignInResponseDto, UserSignInResponseDto>.MapToBaseResponse(userSiginInResponse,
            HttpStatusCode.OK, "success"); 
    }
}