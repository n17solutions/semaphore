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

        /// <summary>
        /// Requests a new Security key pair
        /// </summary>
        /// <param name="force">If true, will force the creation of a new key pair. In doing this, any previous encrypted values will no longer be able to be decrypted</param>
        /// <returns>The private key for the client.</returns>
        [HttpGet]
        public async Task<IActionResult> Get(bool force = false)
        {
            var dataDirectoryExists = Directory.Exists(GenerateKeysRequestHandler.DataFolderName);
            var publicKeyPath = dataDirectoryExists
                ? Path.Combine(GenerateKeysRequestHandler.DataFolderName, GenerateKeysRequestHandler.PublicKeyFileName)
                : GenerateKeysRequestHandler.PublicKeyFileName;

            if (!force && System.IO.File.Exists(publicKeyPath))
                return Ok("Public Key exists already.");

            var result = await _mediator.Send(new GenerateKeysRequest
            {
                PublicKeyPath = publicKeyPath
            });
            return Ok(result);
        }
    }
}