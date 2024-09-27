namespace ws_with_repository_pattern.Application.Dto.Auth;

public class UserSignInRequestDto
{
    public string email { get; set; }
    public string password { get; set; }
}