using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using N17Solutions.Semaphore.Handlers.Security;
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
            var dataDirectoryExists = Directory.Exists(GenerateKeysRequestHandler.DataFolderName);
            var publicKeyPath = dataDirectoryExists
                ? Path.Combine(GenerateKeysRequestHandler.DataFolderName, GenerateKeysRequestHandler.PublicKeyFileName)
                : GenerateKeysRequestHandler.PublicKeyFileName;

            if (System.IO.File.Exists(publicKeyPath))
                return Ok("Public Key exists already.");

            var result = await _mediator.Send(new GenerateKeysRequest
            {
                PublicKeyPath = publicKeyPath
            });
            return Ok(result);
        }
    }
}