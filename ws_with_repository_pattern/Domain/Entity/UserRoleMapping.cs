namespace ws_with_repository_pattern.Domain.Entity;

public class UserRoleMapping
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string userId { get; set; }
    public Guid roleId { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
    public DateTime updated_at { get; set; } = DateTime.Now;
    public DateTime? deleted_at { get; set; }
}