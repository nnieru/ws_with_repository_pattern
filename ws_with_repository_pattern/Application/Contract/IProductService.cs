using ws_with_repository_pattern.Application.Dto.Product;
using ws_with_repository_pattern.Response;

namespace ws_with_repository_pattern.Application.Contract;

public interface IProductService
{
    Task<BaseResponse<IEnumerable<ProductResponseItem>>> GetAllProduct();
    Task<BaseResponse<GetProductByIdResponseDto>> GetProductById(string id);
}