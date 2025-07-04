using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblemAsync(context, 500, "Internal Server Error",
                                    "An unexpected error occurred");
        }
    }

    private static Task WriteProblemAsync(HttpContext ctx, int status,
                                          string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status  = status,
            Title   = title,
            Detail  = detail,
            Instance = ctx.Request.Path
        };
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode  = status;
        return ctx.Response.WriteAsJsonAsync(problem);
    }
}
