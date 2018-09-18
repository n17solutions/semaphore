using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace N17Solutions.Semaphore.Requests.Signals
{
    public class CreateSignalRequest : IRequest<Guid>
    {
        /// <summary>
        /// The Name of this Signal.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value to set on this Signal.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A collection of tags to add to this signal.
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Whether to Encrypt the value
        /// </summary>
        public bool Encrypted { get; set; }
    }
}