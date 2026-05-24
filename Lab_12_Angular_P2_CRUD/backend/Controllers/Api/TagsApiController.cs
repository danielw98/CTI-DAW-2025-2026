using Lab12.DTOs;
using Lab12.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab12.Controllers.Api;

[ApiController]
[Route("api/tags")]
public class TagsApiController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsApiController(ITagService tagService)
    {
        _tagService = tagService;
    }

    // GET: /api/tags - public, used by Angular tag selector in article form
    [HttpGet]
    [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TagDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetAllAsync(cancellationToken);
        return Ok(tags.Select(t => new TagDto(t.Id, t.Name)).ToList());
    }
}
