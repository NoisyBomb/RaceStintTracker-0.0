using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RaceStintTracker.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [HttpGet("/error")]
    public IActionResult HandleError()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        return Problem(
            detail: exception?.Message,
            statusCode: 500
        );
    }
}