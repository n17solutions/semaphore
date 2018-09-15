using MediatR;

namespace N17Solutions.Semaphore.Requests.Security
{
    public class DecryptionRequest : IRequest<string>
    {
        /// <summary>
        /// The PrivateKey to use to decrypt the value.
        /// </summary>
        public string PrivateKey { get; set; }
        
        /// <summary>
        /// The value to decrypt.
        /// </summary>
        public string ToDecrypt { get; set; }
    }
}