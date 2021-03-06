using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Requests.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace N17Solutions.Semaphore.Handlers.Security
{
    public class GenerateKeysRequestHandler : IRequestHandler<GenerateKeysRequest, byte[]>
    {
        public const string PublicKeySettingName = "PublicKey";

        private readonly SemaphoreContext _context;

        public GenerateKeysRequestHandler(SemaphoreContext context)
        {
            _context = context;
        }

        public async Task<byte[]> Handle(GenerateKeysRequest request, CancellationToken cancellationToken)
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            var randomGenerator = new CryptoApiRandomGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(randomGenerator), request.KeySize));

            var keys = rsaKeyPairGenerator.GenerateKeyPair();
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);

            var publicKey = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            var publicKeyValue = Convert.ToBase64String(publicKey);

            await UpdateSetting(PublicKeySettingName, publicKeyValue, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return privateKeyInfo.ToAsn1Object().GetDerEncoded();
        }

        private async Task UpdateSetting(string settingName, string settingValue, CancellationToken cancellationToken)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Name.Equals(settingName, StringComparison.InvariantCultureIgnoreCase), cancellationToken)
                .ConfigureAwait(false);
            if (setting == null)
            {
                setting = new Setting
                {
                    Name = settingName,
                    Value = settingValue
                };
                await _context.Settings.AddAsync(setting, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                setting.Value = settingValue;
            }
        }
    }
}