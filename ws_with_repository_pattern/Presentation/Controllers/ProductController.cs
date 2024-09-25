﻿using Binus.WS.Pattern.Service;
using Microsoft.AspNetCore.Mvc;
using ws_with_repository_pattern.Application.Contract;
using ws_with_repository_pattern.Application.Dto.Product;
using ws_with_repository_pattern.Infrastructures.Helper;

namespace ws_with_repository_pattern.Presentation.Controllers;

[ApiController]
[Route("product")]
public class ProductController: BaseService
{
    private readonly IProductService _productService;
    public ProductController(ILogger<BaseService> logger, IProductService service) : base(logger)
    {
        _productService = service;
    }

    [HttpGet]
    [Route("products")]
    [Produces("application/json")]
    // [ProducesResponseType(typeof(SampleOutput), StatusCodes.Status200OK)]
    public  async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var result = await _productService.GetAllProduct();
            return StatusCode(200, result);
        }
        catch ( Exception ex )
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("productById")]
    [Produces("application/json")]
    // [ProducesResponseType(typeof(SampleOutput), StatusCodes.Status200OK)]
    public  async Task<IActionResult> GetProductById(GetProductByIdRequestDto request) 
    {
        try{ 
            var validator = ValidatorHelperFactory.New(request, new ProductIdRequestValidator());
            var validationResult = validator.Validate();
            if (!validationResult.IsPassed)
            {
                return StatusCode(400, validationResult.Errors);
            }
            
            var result = await _productService.GetProductById(request.id);
            return StatusCode(200, result);
        }
        catch ( Exception ex )
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    
}