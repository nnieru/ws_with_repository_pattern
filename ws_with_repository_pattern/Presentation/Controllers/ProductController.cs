using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Product;
using ws_with_repository_pattern.Infrastructures.Helper;

namespace ws_with_repository_pattern.Presentation.Controllers;

[ApiController]
[Authorize(Roles = "administrator, General")]
[Route("product")]
public class ProductController: ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService service) 
    {
        _productService = service;
    }

    [HttpGet]
    [Authorize]
    [Route("products")]
    [Authorize("read")]
    [Produces("application/json")]
    public  async Task<IActionResult> GetAllProducts()
    {
        
        var result = await _productService.GetAllProduct();
        return StatusCode(200, result);
        
      
    }
    
    [HttpPost]
    [Route("productById")]
    [Authorize("read")]
    [Produces("application/json")]
    public  async Task<IActionResult> GetProductById(GetProductByIdRequestDto request) 
    {
        var result = await _productService.GetProductById(request.id);
        return StatusCode(200, result);
        
    }
    
}