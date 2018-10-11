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
using Newtonsoft.Json;

// ReSharper disable InvertIf

namespace N17Solutions.Semaphore.Handlers.Signals
{
    public class GetSignalByNameRequestHandler : IRequestHandler<GetSignalByNameRequest, SignalResponse>
    {
        private readonly SemaphoreContext _context;
        private readonly IMediator _mediator;

        public GetSignalByNameRequestHandler(SemaphoreContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<SignalResponse> Handle(GetSignalByNameRequest request, CancellationToken cancellationToken)
        {
            var result = await _context.Signals
                .Where(signal => string.Equals(signal.Name, request.Name, StringComparison.InvariantCultureIgnoreCase))
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
                
                var valueType = Type.GetType(result.ValueType);
                if (valueType != null)
                    result.Value = result.IsBaseType ? Convert.ChangeType(result.Value, valueType) : JsonConvert.DeserializeObject(result.Value.ToString());                
            }

            return result;
        } 
    }
}