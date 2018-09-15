using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Requests.Signals;

namespace N17Solutions.Semaphore.API.Controllers
{
    [Route("[Controller]")]
    public class SignalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SignalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name, string tag)
        {
            var privateKey = Request.Headers["private-key"];
            if (!string.IsNullOrEmpty(privateKey))
            {
                if (!System.IO.File.Exists(GenerateKeysRequestHandler.PublicKeyFileName))
                    return StatusCode(500, "No Public Key found.");
            }
            
            var result = await _mediator.Send(new GetSignalByNameAndTagRequest
            {
                Name = name,
                Tag = tag,
                PrivateKey = privateKey
            });
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateSignalRequest request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}