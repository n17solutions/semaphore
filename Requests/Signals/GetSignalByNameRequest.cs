using MediatR;
using N17Solutions.Semaphore.Responses.Signals;

namespace N17Solutions.Semaphore.Requests.Signals
{
    public class GetSignalByNameRequest : GetSignalRequest, IRequest<SignalResponse>
    {
        /// <summary>
        /// The Name of the Signal to retrieve
        /// </summary>
        public string Name { get; set; }
    }
}