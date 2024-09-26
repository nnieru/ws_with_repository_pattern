using System.ComponentModel.DataAnnotations.Schema;

namespace ws_with_repository_pattern.Domain.Entity;

[Table("MasterRole")]
public class MasterRole
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public DateTime created_at { get; set; } = DateTime.Now;
    public DateTime updated_at { get; set; } = DateTime.Now;
    public DateTime? deleted_at { get; set; }
}