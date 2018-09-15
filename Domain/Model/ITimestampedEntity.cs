using System;

namespace N17Solutions.Semaphore.Domain.Model
{
    /// <summary>
    /// Defines the contract of an entity that can track the time it was created and last updated
    /// </summary>
    public interface ITimestampedEntity
    {
        /// <summary>
        /// The <see cref="DateTime" /> this entity was created
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// The <see cref="DateTime" /> this entity was last updated
        /// </summary>
        DateTime DateLastUpdated { get; set; }
    }
}