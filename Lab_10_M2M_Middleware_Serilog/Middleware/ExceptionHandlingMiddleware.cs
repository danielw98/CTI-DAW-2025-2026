namespace Lab10.Middleware;

// TODO Lab 10 (Ex. 2): Implementati middleware-ul global de exception handling
// - In InvokeAsync prindeti exceptiile si raspundeti cu JSON + status code corespunzator
//   (ex. KeyNotFoundException -> 404, UnauthorizedAccessException -> 403, default -> 500)
// - Inregistrati-l INAINTE de LoggingMiddleware in Program.cs (trebuie sa prinda tot)
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: try { await _next(context); } catch (Exception ex) { ... }
        await _next(context);
    }
}
