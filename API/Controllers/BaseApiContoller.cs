using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiContoller : ControllerBase
    {
        private IMediator mediatR;

        protected IMediator Mediator => mediatR ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSucess && result.Value != null) { return Ok(result.Value); }

            if (result.IsSucess && result.Value == null)
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }
    }
}
