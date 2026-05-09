namespace Lab12.Middleware;

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
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException        => (StatusCodes.Status404NotFound,            "Resursa nu a fost gasita."),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden,           "Acces interzis."),
            ArgumentException           => (StatusCodes.Status400BadRequest,          "Cerere invalida."),
            _                           => (StatusCodes.Status500InternalServerError, "A aparut o eroare interna.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message
        });
    }
}
