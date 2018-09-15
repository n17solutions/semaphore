using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using N17Solutions.Semaphore.Responses.FeatureFlags;

namespace N17Solutions.Semaphore.Domain.Model
{
    public class Feature : ITimestampedEntity
    {
        /// <summary>
        /// The Database identifier of this Feature.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// The Resource Id of this Feature.
        /// </summary>
        public Guid ResourceId { get; set; }
        
        /// <summary>
        /// The Name of this Feature.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// A collection of <see cref="Signal" /> objects associated with this Feature
        /// </summary>
        /// <remarks>Think of a <see cref="Signal" /> as the flag part of feature flag.</remarks>
        public ICollection<Signal> Signals { get; set; } = new List<Signal>();

        /// <inheritdoc cref="ITimestampedEntity.DateCreated" />
        public DateTime DateCreated { get; set; }
        
        /// <inheritdoc cref="ITimestampedEntity.DateLastUpdated" />
        public DateTime DateLastUpdated { get; set; }
    }

    public static class FeatureExpressions
    {
        public static Expression<Func<Feature, FeatureFlagResponse>> ToFeatureFlagResponse =>
            domainModel =>
                new FeatureFlagResponse
                {
                    ResourceId = domainModel.ResourceId,
                    Name = domainModel.Name,
                    Settings = domainModel.Signals.Select(signal => bool.Parse(signal.Value)).ToArray()
                };
    }
}