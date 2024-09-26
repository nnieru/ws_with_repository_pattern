using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ws_with_repository_pattern.Domain.Entity;

[Table("UserSession")]
public class UserSession
{
    [Key]
    public string id { get; set; }
    public string userId { get; set; }
    public string access_token { get; set; }
    public int duration { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
    public DateTime updated_at { get; set; } = DateTime.Now;
    public DateTime? deleted_at { get; set; }
}