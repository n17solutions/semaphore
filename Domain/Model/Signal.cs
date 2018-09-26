using System;
using System.Linq.Expressions;
using N17Solutions.Semaphore.Responses.Signals;

namespace N17Solutions.Semaphore.Domain.Model
{
    public class Signal : ITimestampedEntity
    {
        /// <summary>
        /// The Database identifier of this Signal
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// The Resource Id of this Signal
        /// </summary>
        public Guid? ResourceId { get; set; }
        
        /// <summary>
        /// The name of this Signal
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value of this Signal
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// The type of the Signal's value
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Denotes whether the Value is of a base type
        /// </summary>
        public bool IsBaseType { get; set; }

        /// <summary>
        /// The tags associated with this Signal
        /// </summary>
        public string Tags { get; set; }
        
        /// <inheritdoc cref="ITimestampedEntity.DateCreated" />
        public DateTime DateCreated { get; set; }
        
        /// <inheritdoc cref="ITimestampedEntity.DateLastUpdated" />
        public DateTime DateLastUpdated { get; set; }
    }
    
    public static class SignalExpressions
    {
        public static Expression<Func<Signal, SignalResponse>> ToSignalResponse =>
            domainModel =>
                new SignalResponse
                {
                    ResourceId = domainModel.ResourceId,
                    Name = domainModel.Name,
                    Value = domainModel.Value,
                    ValueType = domainModel.ValueType,
                    IsBaseType = domainModel.IsBaseType
                };
    }
}