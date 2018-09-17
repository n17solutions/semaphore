using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;

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
            var publicKey = await _mediator.Send(new GetSettingRequest
            {
                Name = GenerateKeysRequestHandler.PublicKeySettingName
            });

            if (!force && publicKey != null)
                return Ok("Public Key exists already.");

            var result = await _mediator.Send(new GenerateKeysRequest());
            return Ok(result);
        }
    }
}