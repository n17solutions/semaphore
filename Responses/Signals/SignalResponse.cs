using System;

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
        public string Value { get; set; }
    }
}