using System;
using Newtonsoft.Json;

namespace N17Solutions.Semaphore.Responses.Signals
{
    public class SignalResponse
    {
        /// <summary>
        /// The Resource Id of the Signal.
        /// </summary>
        public Guid? ResourceId { get; set; }
        
        /// <summary>
        /// The Name of the Signal.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The Value of the Signal.
        /// </summary>
        public object Value { get; set; }
        
        /// <summary>
        /// The type of the Signal's value
        /// </summary>
        public string ValueType { get; set; }
        
        /// <summary>
        /// Denotes whether the Signal Value is of a base type.
        /// </summary>
        [JsonIgnore]
        public bool IsBaseType { get; set; }
    }
}