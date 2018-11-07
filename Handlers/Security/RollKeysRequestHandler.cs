using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;
using N17Solutions.Semaphore.ServiceContract;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class RollKeysRequestHandler : IRequestHandler<RollKeysRequest, string>
    {
        private readonly SemaphoreContext _context;
        private readonly IMediator _mediator;

        public RollKeysRequestHandler(SemaphoreContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<string> Handle(RollKeysRequest request, CancellationToken cancellationToken)
        {
            // Get all encrypted signals
            var encryptedSignals = await _context.Signals.Where(signal => signal.Tags.Contains(Constants.EncryptedTag))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);
            
            // Decrypt the signals
            foreach (var signal in encryptedSignals)
            {
                signal.Value = await _mediator.Send(new DecryptionRequest
                {
                    PrivateKey = request.PrivateKey,
                    ToDecrypt = signal.Value
                }, cancellationToken).ConfigureAwait(false);
            }
            
            // Generate New Keys
            var privateKey = await _mediator.Send(new GenerateKeysRequest(), cancellationToken).ConfigureAwait(false);
            var publicKey = await _mediator.Send(new GetSettingRequest
            {
                Name = GenerateKeysRequestHandler.PublicKeySettingName
            }, cancellationToken).ConfigureAwait(false);
            
            // Re-encrypt the signals
            foreach (var signal in encryptedSignals)
            {
                signal.Value = await _mediator.Send(new EncryptionRequest
                {
                    PublicKey = Convert.FromBase64String(publicKey),
                    ToEncrypt = signal.Value
                }, cancellationToken).ConfigureAwait(false);
            }

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Convert.ToBase64String(privateKey);
        }
    }
}