using MediatR;
using N17Solutions.Semaphore.Responses.FeatureFlags;

namespace N17Solutions.Semaphore.Requests.FeatureFlags
{
    public class GetFeatureFlagByNameAndTagRequest : IRequest<FeatureFlagResponse>
    {
        /// <summary>
        /// The Name of the Feature Flag.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The Tag to search for within the feature flag.
        /// </summary>
        public string Tag { get; set; }
    }
}