using System.Security.Claims;
using Lab10.DTOs;
using Lab10.Mappings;
using Lab10.Models;
using Lab10.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab10.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ArticlesApiController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesApiController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    // GET: /api/articlesapi
    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ArticleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var articles = await _articleService.GetAllAsync(cancellationToken);
        return Ok(articles.ToDtoList());
    }

    // GET: /api/articlesapi/5
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

    // POST: /api/articlesapi
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

    // PUT: /api/articlesapi/5
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

        if (!IsOwnerOrAdmin(article))
            return Forbid(JwtBearerDefaults.AuthenticationScheme);

        dto.ApplyTo(article);
        await _articleService.UpdateAsync(article, cancellationToken);

        return NoContent();
    }

    // DELETE: /api/articlesapi/5
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

        if (!IsOwnerOrAdmin(article))
            return Forbid(JwtBearerDefaults.AuthenticationScheme);

        await _articleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private bool IsOwnerOrAdmin(Article article)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return article.AuthorId == userId || User.IsInRole("Admin");
    }
}
