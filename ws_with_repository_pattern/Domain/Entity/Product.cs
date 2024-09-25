using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ws_with_repository_pattern.Domain.Entity;

[Table("Product")]
public class Product
{
    [Key]
    public string id { set; get; }
    public string name { set; get; }
    public string description { set; get; }
    public decimal price { set; get; }
    public int stock_quantity { set; get; }
    public DateTime created_at { set; get; }
    public DateTime updated_at { set; get; }
    public DateTime? deleted_at { set; get; }
}