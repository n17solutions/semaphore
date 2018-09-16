using System.IO;
using MediatR;
using N17Solutions.Semaphore.Requests.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class GenerateKeysRequestHandler : RequestHandler<GenerateKeysRequest, byte[]>
    {
        public const string DataFolderName = "/data/semaphore";
        public const string PublicKeyFileName = "public.key";

        protected override byte[] Handle(GenerateKeysRequest request)
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            var randomGenerator = new CryptoApiRandomGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(randomGenerator), request.KeySize));

            var keys = rsaKeyPairGenerator.GenerateKeyPair();
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);

            var publicKey = publicKeyInfo.ToAsn1Object().GetDerEncoded();

            using (var fileStream = File.Create(request.PublicKeyPath))
                fileStream.Write(publicKey, 0, publicKey.Length);

            return privateKeyInfo.ToAsn1Object().GetDerEncoded();
        }
    }
}