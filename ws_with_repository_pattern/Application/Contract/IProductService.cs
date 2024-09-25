using ws_with_repository_pattern.Application.Dto.Product;

namespace ws_with_repository_pattern.Application.Contract;

public interface IProductService
{
    Task<List<ProductResponseItem>> GetAllProduct();
    Task<GetProductByIdResponseDto> GetProductById(string id);
}