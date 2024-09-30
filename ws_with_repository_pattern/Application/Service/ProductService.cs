using System.Net;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Product;
using ws_with_repository_pattern.Domain.Contract;
using ws_with_repository_pattern.Domain.Entity;
using ws_with_repository_pattern.Infrastructures.Helper;
using ws_with_repository_pattern.Response;

namespace ws_with_repository_pattern.Application.Service;

public class ProductService: IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    public async Task<BaseResponse<IEnumerable<ProductResponseItem>>> GetAllProduct()
    {
        var products = await _productRepository.GetAllProducts();
        var response  = ResponseMapper<Product, ProductResponseItem>.MapToBaseResponse(products, HttpStatusCode.OK, "success");
        return response;
    }

    public async Task<BaseResponse<GetProductByIdResponseDto>> GetProductById(string id)
    {
        var product = await _productRepository.GetProductById(id);
        var response = ResponseMapper<Product, GetProductByIdResponseDto>.MapToBaseResponse(product, HttpStatusCode.OK, "success");
        return response;
    }
}