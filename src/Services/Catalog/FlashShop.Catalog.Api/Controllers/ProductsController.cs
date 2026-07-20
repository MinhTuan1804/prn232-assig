using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Catalog.Api.Controllers;

[ApiController]
[Route("api/catalog/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] ProductQueryParams queryParams)
    {
        var result = await _productService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(product));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id },
            ApiResponse<object>.SuccessResponse(product, "Product created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _productService.UpdateAsync(id, request);
        return Ok(ApiResponse<object>.SuccessResponse(product, "Product updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Product deleted successfully."));
    }
}
