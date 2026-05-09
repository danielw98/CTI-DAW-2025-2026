using System.Security.Claims;
using Lab12.Authorization;
using Lab12.DTOs;
using Lab12.Mappings;
using Lab12.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab12.Controllers.Api;

[ApiController]
[Route("api/articles")]
public class ArticlesApiController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesApiController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    // GET: /api/articles
    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ArticleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var articles = await _articleService.GetAllAsync(cancellationToken);
        return Ok(articles.ToDtoList());
    }

    // GET: /api/articles/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article == null)
            return NotFound();

        return Ok(article.ToDto());
    }

    // POST: /api/articles
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ArticleDto>> Create(
        CreateArticleDto dto,
        CancellationToken cancellationToken)
    {
        var article = dto.ToEntity();
        article.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _articleService.AddAsync(article, cancellationToken);

        var created = await _articleService.GetByIdAsync(article.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, created!.ToDto());
    }

    // PUT: /api/articles/5
    [HttpPut("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        UpdateArticleDto dto,
        CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article == null)
            return NotFound();

        if (!User.CanModifyArticle(article))
            return Forbid(JwtBearerDefaults.AuthenticationScheme);

        dto.ApplyTo(article);
        await _articleService.UpdateAsync(article, cancellationToken);

        return NoContent();
    }

    // DELETE: /api/articles/5
    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article == null)
            return NotFound();

        if (!User.CanModifyArticle(article))
            return Forbid(JwtBearerDefaults.AuthenticationScheme);

        await _articleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
