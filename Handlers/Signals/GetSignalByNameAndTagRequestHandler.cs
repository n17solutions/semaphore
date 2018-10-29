using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Signals;
using N17Solutions.Semaphore.Responses.Signals;
using N17Solutions.Semaphore.ServiceContract.Signals;
using Newtonsoft.Json;

// ReSharper disable InvertIf

namespace N17Solutions.Semaphore.Handlers.Signals
{
    public class GetSignalByNameAndTagRequestHandler : IRequestHandler<GetSignalByNameAndTagRequest, SignalResponse>
    {
        private readonly SemaphoreContext _context;
        private readonly IMediator _mediator;

        public GetSignalByNameAndTagRequestHandler(SemaphoreContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<SignalResponse> Handle(GetSignalByNameAndTagRequest request, CancellationToken cancellationToken)
        {
            var result = await _context.Signals
                .Where(signal => string.Equals(signal.Name, request.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                 signal.Tags.Contains(request.Tag))
                .Select(SignalExpressions.ToSignalResponse)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
           
            if (result != null)
            {
                if (!string.IsNullOrEmpty(request.PrivateKey) && result.IsEncrypted)
                {
                    result.Value = await _mediator.Send(new DecryptionRequest
                    {
                        PrivateKey = request.PrivateKey,
                        ToDecrypt = result.Value.ToString()
                    }, cancellationToken).ConfigureAwait(false);
                }
                else if (result.IsEncrypted)
                    return result;
                
                result.Value = ValueResolver.Resolve(result.Value, result.ValueType, result.IsBaseType);
            }

            return result;
        }
    }
}