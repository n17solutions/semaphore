using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Requests.FeatureFlags;
using N17Solutions.Semaphore.Responses.FeatureFlags;

namespace N17Solutions.Semaphore.Handlers.FeatureFlags
{
    public class GetFeatureFlagByNameAndTagRequestHandler : IRequestHandler<GetFeatureFlagByNameAndTagRequest, FeatureFlagResponse>
    {
        private readonly SemaphoreContext _context;

        public GetFeatureFlagByNameAndTagRequestHandler(SemaphoreContext context)
        {
            _context = context;
        }

        public async Task<FeatureFlagResponse> Handle(GetFeatureFlagByNameAndTagRequest request, CancellationToken cancellationToken)
        {
            var result = await _context.Features
                .Where(feature => string.Equals(feature.Name, request.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                  feature.Signals.Select(signal => signal.Tags).Any(tag => tag.Contains(request.Tag)))
                .Select(FeatureExpressions.ToFeatureFlagResponse)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}