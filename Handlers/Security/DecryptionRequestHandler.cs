using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N17Solutions.Semaphore.Encryption;
using N17Solutions.Semaphore.Requests.Security;
using Newtonsoft.Json;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class DecryptionRequestHandler : IRequestHandler<DecryptionRequest, string>
    {
        private readonly DataEncrypter _dataEncrypter;
        
        public DecryptionRequestHandler(DataEncrypter dataEncrypter)
        {
            _dataEncrypter = dataEncrypter;
        }

        public async Task<string> Handle(DecryptionRequest request, CancellationToken cancellationToken)
        {
            var dataBlock = JsonConvert.DeserializeObject<EncryptedDataBlock>(request.ToDecrypt);
            var result = await _dataEncrypter.DecryptDataBlock(Convert.FromBase64String(request.PrivateKey), dataBlock);
            return result;
        }
    }
}