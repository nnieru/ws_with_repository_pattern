namespace ws_with_repository_pattern.Application.Dto.Auth;

public class UserSignInResponseDto
{
    public string id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string access_token { get; set; }
    public DateTime expiration { get; set; }
}