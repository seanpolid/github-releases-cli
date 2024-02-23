using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GitHubReleasesCLI.utils
{
    public class SignatureUtils
    {
        public static byte[] Sign(string baseDirectory, byte[] zippedAssets, string keyPath)
        {
            byte[] hash = SHA512.HashData(zippedAssets);

            string privateKeyText = File.ReadAllText(keyPath);
            string signaturePath = $"{baseDirectory}/signature.dat";

            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyText);

            RSAPKCS1SignatureFormatter rsaFormatter = new(rsa);
            rsaFormatter.SetHashAlgorithm(nameof(SHA512));

            return rsaFormatter.CreateSignature(hash);
        }
    }
}
