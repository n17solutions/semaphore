using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;
using N17Solutions.Semaphore.Requests.Signals;
using Newtonsoft.Json;

namespace N17Solutions.Semaphore.Handlers.Signals
{
    public class CreateSignalRequestHandler : IRequestHandler<CreateSignalRequest, Guid>
    {
        public const string EncryptedTag = "encrypted";
        public const string SignalAlreadyExistsErrorMessage = "Signal already exists.";
        
        private readonly SemaphoreContext _context;
        private readonly IMediator _mediator;

        public CreateSignalRequestHandler(SemaphoreContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateSignalRequest request, CancellationToken cancellationToken)
        {
            var tags = request.Tags.ToList();
            var value = !(request.Value is string) ? JsonConvert.SerializeObject(request.Value) : request.Value.ToString();
            if (request.Encrypted)
            {
                var publicKey = await _mediator.Send(new GetSettingRequest
                {
                    Name = GenerateKeysRequestHandler.PublicKeySettingName
                }, cancellationToken).ConfigureAwait(false);
                
                value = await _mediator.Send(new EncryptionRequest
                {
                    PublicKey = Convert.FromBase64String(publicKey),
                    ToEncrypt = value
                }, cancellationToken).ConfigureAwait(false);
                
                if (!tags.Contains(EncryptedTag))
                    tags.Add(EncryptedTag);
            }

            var joinedTags = string.Join(",", tags);
            
            // We check here to make sure we're not creating a duplicate
            var duplicate = tags.Any()
                ? await _mediator.Send(new GetSignalByNameAndTagRequest
                {
                    Name = request.Name,
                    Tag = joinedTags
                }, cancellationToken).ConfigureAwait(false)
                : await _mediator.Send(new GetSignalByNameRequest
                {
                    Name = request.Name
                }, cancellationToken).ConfigureAwait(false);
            if (duplicate != null)
                throw new InvalidOperationException(SignalAlreadyExistsErrorMessage);
            
            var signal = new Signal
            {
                ResourceId = RT.Comb.Provider.PostgreSql.Create(),
                Name = request.Name,
                Tags = joinedTags,
                Value = value
            };

            await _context.Signals.AddAsync(signal, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return signal.ResourceId.Value;
        }
    }
}