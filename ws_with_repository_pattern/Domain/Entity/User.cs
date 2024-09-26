using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ws_with_repository_pattern.Domain.Entity;

[Table("User")]
public class User
{
    [Key] public string id { get; set; } = Guid.NewGuid().ToString();
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
    public DateTime updated_at { get; set; } = DateTime.Now;
    public DateTime? deleted_at { get; set; }
}