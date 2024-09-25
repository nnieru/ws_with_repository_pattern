using Microsoft.EntityFrameworkCore;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.DbContext;
using ws_with_repository_pattern.Domain.Entity;

namespace ws_with_repository_pattern.Domain.Repository;

public class ProductRepository: IProductRepository
{
    private readonly KazutoDbContext _productContext;

    public ProductRepository(KazutoDbContext productContext)
    {
        _productContext = productContext;
    }
        
    public async Task<List<Product>> GetAllProducts()
    {
        return await _productContext.Set<Product>().ToListAsync();
    }

    public async Task<Product?> GetProductById(string id)
    {
        return await _productContext.Set<Product>().FirstOrDefaultAsync(x => x.id == id);
    }
}