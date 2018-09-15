using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace N17Solutions.Semaphore.Requests.FeatureFlags
{
    public class CreateFeatureFlagRequest : IRequest<Guid>
    {
        /// <summary>
        /// The Name of the Feature Flag to be created.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A collection of Tags to associate with this Feature Flag.
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        
        /// <summary>
        /// The initial setting of this Feature Flag.
        /// </summary>
        public bool Setting { get; set; } = false;
    }
}