using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace N17Solutions.Semaphore.Encryption
{
    public class DataEncrypter
    {
        public EncryptedDataBlock EncryptData(byte[] publicKey, string dataToEncrypt)
        {
            var encryptedData = SymmetricallyEncrypt(dataToEncrypt);
            var hash = GenerateHash(encryptedData.encryptedValue);
            var digitalSignature = AsymmetricallyEncrypt(publicKey, hash);

            return new EncryptedDataBlock
            {
                EncryptedData = encryptedData.encryptedValue,
                DigitalSignature = digitalSignature,
                AesKey = Convert.ToBase64String(encryptedData.aesKey),
                InitialisationVector = Convert.ToBase64String(encryptedData.aesIv)
            };
        }

        public async Task<string> DecryptDataBlock(byte[] privateKey, EncryptedDataBlock dataBlock)
        {
            ValidateDigitalSignature(privateKey, dataBlock);

            return await SymmetricallyDecrypt(Convert.FromBase64String(dataBlock.AesKey), Convert.FromBase64String(dataBlock.InitialisationVector),
                dataBlock.EncryptedData).ConfigureAwait(false);
        }

        public string OldFashionedDecrypter(string privateKey, string toDecrypt)
        {
            try
            {
                var key = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
                var rsaParameters2 = DotNetUtilities.ToRSAParameters(key);
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParameters2);

                var dec = rsa.Decrypt(Convert.FromBase64String(toDecrypt), false);
                return Encoding.UTF8.GetString(dec);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while decrypting the value. This is most likely due to the wrong private key being used. See InnerException for more details.", ex);
            }
        }

        private static void ValidateDigitalSignature(byte[] privateKey, EncryptedDataBlock dataBlock)
        {
            try
            {
                var decryptedDigitalSignature = AsymmetricallyDecrypt(privateKey, dataBlock.DigitalSignature);
                var hash = GenerateHash(dataBlock.EncryptedData);

                if (string.Compare(decryptedDigitalSignature, hash, StringComparison.OrdinalIgnoreCase) != 0)
                    throw new InvalidOperationException("The computed digital signature for the data block does not match the original digital signature.");
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("There was a problem decrypting the data block. Potential data corruption or packet tampering has occurred.", ex);
            }
        }

        private static string GenerateHash(string toHash)
        {
            var sha512 = new SHA512Managed();
            return Convert.ToBase64String(sha512.ComputeHash(Encoding.UTF8.GetBytes(toHash)));
        }

        private static string AsymmetricallyEncrypt(byte[] publicKey, string toEncrypt)
        {
            var key = (RsaKeyParameters) PublicKeyFactory.CreateKey(publicKey);
            var rsaParameters = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.Exponent.ToByteArrayUnsigned()
            };
            
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            var encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(toEncrypt), false);
            return Convert.ToBase64String(encrypted);
        }

        private static string AsymmetricallyDecrypt(byte[] privateKey, string toDecrypt)
        {
            try
            {
                var key = (RsaPrivateCrtKeyParameters) PrivateKeyFactory.CreateKey(privateKey);
                var rsaParameters2 = DotNetUtilities.ToRSAParameters(key);
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParameters2);

                var decrypted = rsa.Decrypt(Convert.FromBase64String(toDecrypt), false);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while decrypting the value. This is most likely due to the wrong private key being used. See InnerException for more details.", ex);
            }
        }

        private (string encryptedValue, byte[] aesKey, byte[] aesIv) SymmetricallyEncrypt(string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt))
                throw new ArgumentNullException(nameof(toEncrypt));

            byte[] encrypted, aesKey, aesIv;

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    throw new ApplicationException("Creating an instance of AES failed.");

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                        swEncrypt.Write(toEncrypt);

                    encrypted = msEncrypt.ToArray();
                }

                aesKey = aesAlg.Key;
                aesIv = aesAlg.IV;
            }

            return (Convert.ToBase64String(encrypted), aesKey, aesIv);
        }

        private async Task<string> SymmetricallyDecrypt(byte[] key, byte[] iv, string toDecrypt)
        {
            if (string.IsNullOrEmpty(toDecrypt))
                throw new ArgumentNullException(nameof(toDecrypt));

            string plainText;
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null)
                    throw new ApplicationException("Creating an instance of AES failed.");

                aesAlg.Key = key;
                aesAlg.IV = iv;
                
                var cipherText = Convert.FromBase64String(toDecrypt);
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                using (var msDecrypt = new MemoryStream(cipherText))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                        plainText = await srDecrypt.ReadToEndAsync().ConfigureAwait(false);
            }

            return plainText;
        }
    }
}