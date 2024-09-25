using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Product;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.Entity;
using ws_with_repository_pattern.Infrastructures.Helper;

namespace ws_with_repository_pattern.Application.Service;

public class ProductService: IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<List<ProductResponseItem>> GetAllProduct()
    {
        var products = await _productRepository.GetAllProducts();
        var response  = ResponseMapper<Product, ProductResponseItem>.Map(products);
        return response.ToList();
    }

    public async Task<GetProductByIdResponseDto> GetProductById(string id)
    {
        var product = await _productRepository.GetProductById(id);
        var response = ResponseMapper<Product, GetProductByIdResponseDto>.Map(product);
        return response;
    }
}