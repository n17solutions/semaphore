using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Requests.FeatureFlags;

namespace N17Solutions.Semaphore.Handlers.FeatureFlags
{
    public class CreateFeatureFlagRequestHandler : IRequestHandler<CreateFeatureFlagRequest, Guid>
    {
        private readonly SemaphoreContext _context;

        public CreateFeatureFlagRequestHandler(SemaphoreContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateFeatureFlagRequest request, CancellationToken cancellationToken)
        {
            var feature = new Feature
            {
                ResourceId = RT.Comb.Provider.PostgreSql.Create(),
                Name = request.Name,
                Signals = new List<Signal>
                {
                    new Signal
                    {
                        Value = request.Setting.ToString(),
                        Tags = string.Join(",", request.Tags)
                    }
                }
            };

            await _context.Features.AddAsync(feature, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return feature.ResourceId;
        }
    }
}