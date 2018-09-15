using MediatR;
using N17Solutions.Semaphore.Responses.Signals;

namespace N17Solutions.Semaphore.Requests.Signals
{
    public class GetSignalByNameAndTagRequest : IRequest<SignalResponse>
    {
        /// <summary>
        /// The Name of the Signal to retrieve
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The Tag the Signal must be associated to
        /// </summary>
        public string Tag { get; set; }
        
        /// <summary>
        /// If set, will be used to decrypt the value of this Signal.
        /// </summary>
        public string PrivateKey { get; set; }
    }
}