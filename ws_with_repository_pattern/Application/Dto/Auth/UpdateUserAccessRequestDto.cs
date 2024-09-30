namespace ws_with_repository_pattern.Application.Dto.Auth;

public class UpdateUserAccessRequestDto
{
    public string userId { get; set; }
    public string roleId { get; set; }
    public bool read { get; set; }
    public bool write { get; set; }
    public bool delete { get; set; }
}