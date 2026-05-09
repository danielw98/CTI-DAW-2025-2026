namespace Lab10.Middleware;

// TODO Lab 10 (Ex. 2): Implementati middleware-ul de logging
// - In InvokeAsync masurati durata, logati method, path, status code si ms
// - Folositi _logger.LogInformation cu structured logging ({Method}, {Path}, ...)
// - Inregistrati-l in Program.cs cu app.UseMiddleware<LoggingMiddleware>();
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: masurati timpul cu Stopwatch sau DateTime.UtcNow,
        //       logati inainte si dupa await _next(context).
        await _next(context);
    }
}
