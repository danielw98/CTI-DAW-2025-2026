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
    private readonly ITagService _tagService;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxImageBytes = 5 * 1024 * 1024;

    public ArticlesApiController(
        IArticleService articleService,
        ITagService tagService,
        IWebHostEnvironment env)
    {
        _articleService = articleService;
        _tagService = tagService;
        _env = env;
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

        if (dto.TagIds != null && dto.TagIds.Count > 0)
        {
            var tags = await _tagService.GetByIdsAsync(dto.TagIds, cancellationToken);
            foreach (var tag in tags) article.Tags.Add(tag);
        }

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

        // Semantica PUT: ce trimitem in TagIds = exact ce ramane asociat la articol
        article.Tags.Clear();
        if (dto.TagIds != null && dto.TagIds.Count > 0)
        {
            var tags = await _tagService.GetByIdsAsync(dto.TagIds, cancellationToken);
            foreach (var tag in tags) article.Tags.Add(tag);
        }

        await _articleService.UpdateAsync(article, cancellationToken);

        return NoContent();
    }

    // POST: /api/articles/5/image - upload imagine pentru articol (scaffold pre-built Lab12)
    [HttpPost("{id:int}/image")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(
        int id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article == null)
            return NotFound();

        if (!User.CanModifyArticle(article))
            return Forbid(JwtBearerDefaults.AuthenticationScheme);

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Fisierul lipseste sau e gol" });

        if (file.Length > MaxImageBytes)
            return BadRequest(new { message = $"Fisier prea mare (max {MaxImageBytes / 1024 / 1024} MB)" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return BadRequest(new { message = $"Extensie nepermisa. Permise: {string.Join(", ", AllowedImageExtensions)}" });

        var fileName = $"article-{id}-{Guid.NewGuid():N}{ext}";
        var imagesDir = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(imagesDir);
        var savePath = Path.Combine(imagesDir, fileName);

        await using (var stream = System.IO.File.Create(savePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        article.ImagePath = $"/images/{fileName}";
        await _articleService.UpdateAsync(article, cancellationToken);

        return Ok(new { imagePath = article.ImagePath });
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
