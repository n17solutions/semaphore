using MediatR;

namespace N17Solutions.Semaphore.Requests.Security
{
    public class GenerateKeysRequest : IRequest<byte[]>
    {
        public int KeySize { get; set; } = 4096;
    }
}