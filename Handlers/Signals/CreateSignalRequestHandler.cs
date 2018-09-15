using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Signals;

namespace N17Solutions.Semaphore.Handlers.Signals
{
    public class CreateSignalRequestHandler : IRequestHandler<CreateSignalRequest, Guid>
    {
        private readonly SemaphoreContext _context;
        private readonly IMediator _mediator;

        public CreateSignalRequestHandler(SemaphoreContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateSignalRequest request, CancellationToken cancellationToken)
        {
            var value = request.Value;
            if (request.Encrypted)
            {
                var publicKey = File.ReadAllBytes(GenerateKeysRequestHandler.PublicKeyFileName);
                value = await _mediator.Send(new EncryptionRequest
                {
                    PublicKey = publicKey,
                    ToEncrypt = value
                }, cancellationToken).ConfigureAwait(false);
            }
            
            var signal = new Signal
            {
                ResourceId = RT.Comb.Provider.PostgreSql.Create(),
                Name = request.Name,
                Tags = string.Join(",", request.Tags),
                Value = value
            };

            await _context.Signals.AddAsync(signal, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return signal.ResourceId.Value;
        }
    }
}