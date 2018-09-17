using MediatR;

namespace N17Solutions.Semaphore.Requests.Security
{
    public class GenerateKeysRequest : IRequest<byte[]>
    {
        /// <summary>
        /// The size, in bytes of the Public Key
        /// </summary>
        public int KeySize { get; set; } = 4096;
    }
}