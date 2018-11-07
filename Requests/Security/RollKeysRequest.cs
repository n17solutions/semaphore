using MediatR;

namespace N17Solutions.Semaphore.Requests.Security
{
    public class RollKeysRequest : IRequest<string>
    {
        /// <summary>
        /// The PrivateKey to use to decrypt the value.
        /// </summary>
        public string PrivateKey { get; set; }
    }
}