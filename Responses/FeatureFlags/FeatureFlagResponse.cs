using System;
using System.Linq;

namespace N17Solutions.Semaphore.Responses.FeatureFlags
{
    public class FeatureFlagResponse
    {
        /// <summary>
        /// The Resource Id of this Feature Flag.
        /// </summary>
        public Guid ResourceId { get; set; }
        
        /// <summary>
        /// The Name of this Feature Flag.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The collection of Settings associated with this Feature Flag.
        /// </summary>
        public bool[] Settings { get; set; }

        /// <summary>
        /// The Setting associated with this Feature Flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">If multiple Settings are found.</exception>
        public bool Setting
        {
            get
            {
                if (Settings.Length > 1)
                    throw new InvalidOperationException("Sequence contains more than one element.");

                return Settings.Single();
            }
        }
    }
}