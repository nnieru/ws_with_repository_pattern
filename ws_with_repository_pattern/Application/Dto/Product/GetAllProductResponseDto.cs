namespace ws_with_repository_pattern.Application.Dto.Product;

public class GetAllProductResponseDto
{
    public List<ProductResponseItem> products { get; set; }
}

public  class ProductResponseItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public decimal price { get; set; }
    public int stock { get; set; }
}

