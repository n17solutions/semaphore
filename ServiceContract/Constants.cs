using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("N17Solutions.Semaphore.Handlers")]
[assembly:InternalsVisibleTo("N17Solutions.Semaphore.Handlers.Tests")]
[assembly:InternalsVisibleTo("N17Solutions.Semaphore.Domain")]
[assembly:InternalsVisibleTo("N17Solutions.Semaphore.Domain.Tests")]

namespace N17Solutions.Semaphore.ServiceContract
{
    internal class Constants
    {
        public const string EncryptedTag = "encrypted";
    }
}