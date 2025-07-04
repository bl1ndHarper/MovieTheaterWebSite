using Microsoft.AspNetCore.Mvc;

namespace MovieTheater.Web.Models;

public class ApiProblem
{
    public int Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ApiProblem(int status, string error, string message)
    {
        Status = status;
        Error = error;
        Message = message;
    }

    public static ObjectResult AsResult(int status, string error, string message)
    {
        var problem = new ApiProblem(status, error, message);
        return new ObjectResult(problem) { StatusCode = status };
    }
}
