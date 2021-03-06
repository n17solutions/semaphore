using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;
using N17Solutions.Semaphore.Requests.Signals;
using N17Solutions.Semaphore.ServiceContract;
using N17Solutions.Semaphore.ServiceContract.Extensions;
using N17Solutions.Semaphore.ServiceContract.Signals;

namespace N17Solutions.Semaphore.Handlers.Signals
{
    public class PatchSignalRequestHandler : AsyncRequestHandler<PatchSignalRequest>
    {
        public const string SignalNotFoundMessage = "No Signal found with the identifier '{0}'.";
        
        private readonly SemaphoreContext _semaphoreContext;
        private readonly IMediator _mediator;

        public PatchSignalRequestHandler(SemaphoreContext semaphoreContext, IMediator mediator)
        {
            _semaphoreContext = semaphoreContext;
            _mediator = mediator;
        }

        protected override Task Handle(PatchSignalRequest request, CancellationToken cancellationToken)
        {
            return Execute(request, cancellationToken);
        }

        internal async Task Execute(PatchSignalRequest request, CancellationToken cancellationToken)
        {
            var domainModel = await _semaphoreContext.Signals.FirstOrDefaultAsync(signal => signal.ResourceId == request.Id, cancellationToken).ConfigureAwait(false);
            if (domainModel == null)
                throw new InvalidOperationException(string.Format(SignalNotFoundMessage, request.Id));

            var wasEncrypted = domainModel.IsEncrypted();
            if (wasEncrypted && !request.PrivateKey.IsNullOrBlank())
            {
                domainModel.Value = await _mediator.Send(new DecryptionRequest
                {
                    PrivateKey = request.PrivateKey,
                    ToDecrypt = domainModel.Value
                }, cancellationToken).ConfigureAwait(false);
            }
            
            // Because the request needs to be the abstracted SignalWriteModel we have to perform some necessary steps
            // 1) Map the domain model to a write model
            // 2) Use the Microsoft JSON Patch framework to apply the changes requested
            // 3) Populate the domain model with the given changes
            // 4) If requested, encrypt the value
            // 5) Save changes
            
            // 1)
            var writeModel = domainModel.ToWriteModel();
            
            // 2)
            request.Patch.ApplyTo(writeModel);
            if ((writeModel.Tags.IsNullOrEmpty() || !writeModel.Tags.Contains(Constants.EncryptedTag)) && (wasEncrypted || writeModel.Encrypted))
                writeModel.Tags = (writeModel.Tags ?? Enumerable.Empty<string>()).Union(new[] {Constants.EncryptedTag}).ToList();
            
            // 3)
            domainModel.PopulateFromWriteModel(writeModel);
            
            // 4)
            if (request.Patch.Operations.Any(operation => CultureInfo.CurrentCulture.CompareInfo.IndexOf(operation.path, nameof(SignalWriteModel.Value), CompareOptions.OrdinalIgnoreCase) >= 0 ||
                                                          CultureInfo.CurrentCulture.CompareInfo.IndexOf(operation.path, nameof(SignalWriteModel.Tags), CompareOptions.OrdinalIgnoreCase) >= 0))
            {
                if (writeModel.Encrypted || (writeModel.Tags ?? new List<string>()).Contains(Constants.EncryptedTag))
                {
                    var publicKey = await _mediator.Send(new GetSettingRequest
                    {
                        Name = GenerateKeysRequestHandler.PublicKeySettingName
                    }, cancellationToken).ConfigureAwait(false);

                    domainModel.Value = await _mediator.Send(new EncryptionRequest
                    {
                        PublicKey = Convert.FromBase64String(publicKey),
                        ToEncrypt = domainModel.Value
                    }, cancellationToken).ConfigureAwait(false);
                }
            }
            
            // 5)
            await _semaphoreContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}