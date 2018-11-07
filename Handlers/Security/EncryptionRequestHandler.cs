using MediatR;
using N17Solutions.Semaphore.Encryption;
using N17Solutions.Semaphore.Requests.Security;
using Newtonsoft.Json;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class EncryptionRequestHandler : RequestHandler<EncryptionRequest, string>
    {
        private readonly DataEncrypter _dataEncrypter;
        
        public EncryptionRequestHandler(DataEncrypter dataEncrypter)
        {
            _dataEncrypter = dataEncrypter;
        }

        protected override string Handle(EncryptionRequest request)
        {
            var result = _dataEncrypter.EncryptData(request.PublicKey, request.ToEncrypt);
            return JsonConvert.SerializeObject(result);
        }
    }
}