using MediatR;

namespace N17Solutions.Semaphore.Requests.Security
{
    public class EncryptionRequest : IRequest<string>
    {
        /// <summary>
        /// The public key
        /// </summary>
        public byte[] PublicKey { get; set; }
        
        /// <summary>
        /// The string to encrypt
        /// </summary>
        public string ToEncrypt { get; set; }
    }
}