using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Domain.Contract;

public interface IProductRepository
{
    public Task<List<Product>> GetAllProducts();
    public Task<Product?> GetProductById(string id);
}