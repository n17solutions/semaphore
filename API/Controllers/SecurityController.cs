using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using N17Solutions.Semaphore.Requests.Security;

namespace N17Solutions.Semaphore.API.Controllers
{
    [Route("[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SecurityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GenerateKeysRequest());
            return Ok(result);
        }
    }
}