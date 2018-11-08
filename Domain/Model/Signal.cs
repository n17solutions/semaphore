using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using N17Solutions.Semaphore.Responses.Signals;
using N17Solutions.Semaphore.ServiceContract;
using N17Solutions.Semaphore.ServiceContract.Extensions;
using N17Solutions.Semaphore.ServiceContract.Signals;
using Newtonsoft.Json;

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

        public SignalWriteModel ToWriteModel()
        {
            var isEncrypted = Tags != null && Tags.Contains(Constants.EncryptedTag);
            
            return new SignalWriteModel
            {
                Name = Name,
                Tags = Tags?.Split(',').ToList() ?? new List<string>(),
                Encrypted = isEncrypted,
                Value = ValueResolver.Resolve(Value, ValueType, IsBaseType)
            };
        }

        public void PopulateFromWriteModel(SignalWriteModel writeModel)
        {
            var isBaseType = writeModel.Value.IsBaseType();
            
            Name = writeModel.Name;
            Tags = writeModel.Tags.IsNullOrEmpty() ? null : string.Join(",", writeModel.Tags);
            IsBaseType = isBaseType;
            ValueType = writeModel.Value.GetSignalValueType();
            Value = isBaseType ? writeModel.Value.ToString() : JsonConvert.SerializeObject(writeModel.Value);
        }

        public bool IsEncrypted()
        {
            return Tags?.Contains(Constants.EncryptedTag) ?? false;
        }
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
                    IsBaseType = domainModel.IsBaseType,
                    IsEncrypted = domainModel.Tags != null && domainModel.Tags.Contains(Constants.EncryptedTag)
                };
    }
}