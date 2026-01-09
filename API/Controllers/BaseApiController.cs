using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected IMediator Mediator
        => field ??= HttpContext.RequestServices.GetService<IMediator>() ??
                     throw new InvalidOperationException(
                         "Mediator service is unavailable");

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        return result switch
        {
            { IsSuccess: false, Code: 404 } => NotFound(),
            { IsSuccess: true, Value: not null } => Ok(result.Value),
            _ => BadRequest(result.Error)
        };
    }
}