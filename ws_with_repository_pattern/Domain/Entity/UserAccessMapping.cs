using System.ComponentModel.DataAnnotations.Schema;

namespace ws_with_repository_pattern.Domain.Entity;

[Table("UserAccessMapping")]
public class UserAccessMapping
{
    public Guid id { get; set; } = Guid.NewGuid();
    public string user_id { get; set; }
    public Guid role_id {get; set; }
    public bool read { get; set; } = false;
    public bool write { get; set; } = false;
    public bool delete { get; set; } = false;
    public DateTime created_at { get; set; } = DateTime.Now;
    public DateTime updated_at { get; set; } = DateTime.Now;
    public DateTime? deleted_at { get; set; }
}