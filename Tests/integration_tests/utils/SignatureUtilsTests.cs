using GitHubReleasesCLI.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tests.integration_tests.utils
{
    public class SignatureUtilsTests
    {
        private static readonly string BASE_DIRECTORY = Directory.GetCurrentDirectory();
        private static readonly string PRIVATE_KEY_PATH = $"{BASE_DIRECTORY}/private.pem";

        public void Dispose()
        {
            File.Delete(PRIVATE_KEY_PATH);
        }

        [Fact]
        public void Sign_ValidPEM_Success()
        {
            // Arrange
            using RSA rsa = RSA.Create();
            RSAParameters parameters = rsa.ExportParameters(false);

            string privateKey = rsa.ExportRSAPrivateKeyPem();
            File.WriteAllText(PRIVATE_KEY_PATH, privateKey);

            byte[] data = Encoding.UTF8.GetBytes("Test");
            
            // Act
            byte[] signature = SignatureUtils.Sign(BASE_DIRECTORY, data, PRIVATE_KEY_PATH);

            // Assert
            bool valid = isValid(data, signature, parameters);
            Assert.True(valid);
        }

        public bool isValid(byte[] data, byte[] signature, RSAParameters parameters)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportParameters(parameters);

            RSAPKCS1SignatureDeformatter rsaDeformatter = new(rsa);
            rsaDeformatter.SetHashAlgorithm(nameof(SHA512));

            byte[] hash = SHA512.HashData(data);

            return rsaDeformatter.VerifySignature(hash, signature);
        }

        [Fact]
        public void Sign_InvalidPEM_Exception()
        {
            // Arrange
            using RSA rsa = RSA.Create();
            RSAParameters parameters = rsa.ExportParameters(false);

            File.WriteAllText(PRIVATE_KEY_PATH, "invalid text");

            byte[] data = Encoding.UTF8.GetBytes("Test");

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
            {
                SignatureUtils.Sign(BASE_DIRECTORY, data, PRIVATE_KEY_PATH);
            });
        }
    }
}
