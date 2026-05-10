using Lab12.DTOs;
using Lab12.Mappings;
using Lab12.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab12.Controllers.Api;

[ApiController]
[Route("api/categories")]
public class CategoriesApiController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /api/categories - public, used by Angular dropdown in article forms
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories.ToDtoList());
    }
}
