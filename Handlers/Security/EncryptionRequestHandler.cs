using System;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using N17Solutions.Semaphore.Requests.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class EncryptionRequestHandler : RequestHandler<EncryptionRequest, string>
    {
        protected override string Handle(EncryptionRequest request)
        {
            var publicKey = (RsaKeyParameters) PublicKeyFactory.CreateKey(request.PublicKey);
            
            var rsaParameters = new RSAParameters
            {
                Modulus = publicKey.Modulus.ToByteArrayUnsigned(),
                Exponent = publicKey.Exponent.ToByteArrayUnsigned()
            };

            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            var bytes = Encoding.UTF8.GetBytes(request.ToEncrypt);
            var enc = rsa.Encrypt(bytes, false);
            return Convert.ToBase64String(enc);
        }
    }
}