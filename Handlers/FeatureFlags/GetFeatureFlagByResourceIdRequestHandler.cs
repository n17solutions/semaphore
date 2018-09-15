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
    public class GetFeatureFlagByResourceIdRequestHandler : IRequestHandler<GetFeatureFlagByResourceIdRequest, FeatureFlagResponse>
    {
        private readonly SemaphoreContext _context;

        public GetFeatureFlagByResourceIdRequestHandler(SemaphoreContext context)
        {
            _context = context;
        }

        public async Task<FeatureFlagResponse> Handle(GetFeatureFlagByResourceIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _context.Features
                .Where(feature => feature.ResourceId == request.ResourceId)
                .Select(FeatureExpressions.ToFeatureFlagResponse)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}