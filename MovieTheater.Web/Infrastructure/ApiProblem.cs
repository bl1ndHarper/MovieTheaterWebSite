using Microsoft.AspNetCore.Mvc;

namespace MovieTheater.Web.Infrastructure;

public static class ApiProblem
{
    public static IActionResult Bad(string title, string detail)
        => Problem(400, title, detail);

    public static IActionResult Unauthorized(string title, string detail = "")
        => Problem(401, title, detail);

    public static IActionResult Forbidden(string title, string detail = "")
        => Problem(403, title, detail);

    public static IActionResult NotFound(string title, string detail)
        => Problem(404, title, detail);

    public static IActionResult Conflict(string title, string detail)
        => Problem(409, title, detail);

    public static IActionResult Internal(string detail = "Unexpected error")
        => Problem(500, "Internal Server Error", detail);

    private static IActionResult Problem(int status, string title, string detail) =>
        new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = detail
        }) { StatusCode = status };
}
