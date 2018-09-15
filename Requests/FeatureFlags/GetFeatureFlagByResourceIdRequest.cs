using System;
using MediatR;
using N17Solutions.Semaphore.Responses.FeatureFlags;

namespace N17Solutions.Semaphore.Requests.FeatureFlags
{
    public class GetFeatureFlagByResourceIdRequest : IRequest<FeatureFlagResponse>
    {
        /// <summary>
        /// The Resource Id of this Feature Flag.
        /// </summary>
        public Guid ResourceId { get; set; }
    }
}