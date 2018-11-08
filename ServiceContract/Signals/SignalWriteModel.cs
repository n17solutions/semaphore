using System.Collections.Generic;
using System.Linq;

namespace N17Solutions.Semaphore.ServiceContract.Signals
{
    public class SignalWriteModel
    {
        /// <summary>
        /// The Name of this Signal.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value to set on this Signal.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// A collection of tags to add to this signal.
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Whether to Encrypt the value
        /// </summary>
        public bool Encrypted { get; set; }
    }
}