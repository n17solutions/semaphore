using System;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using N17Solutions.Semaphore.Requests.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class DecryptionRequestHandler : RequestHandler<DecryptionRequest, string>
    {
        protected override string Handle(DecryptionRequest request)
        {
            try
            {
                var key = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(Convert.FromBase64String(request.PrivateKey));
                var rsaParameters2 = DotNetUtilities.ToRSAParameters(key);
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParameters2);

                var dec = rsa.Decrypt(Convert.FromBase64String(request.ToDecrypt), false);
                return Encoding.UTF8.GetString(dec);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while decrypting the value. This is most likely due to the wrong private key being used. See InnerException for more details.", ex);
            }
        }
    }
}