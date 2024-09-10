using Crytography.Web.Models;
using System.Numerics;
using System.Text;

namespace Crytography.Web.Services
{
    public class LabThreeService
    {
        private static readonly short CAP = 20;

        public static Lab3Model GenerateKeys()
        {
            var N = GlobalService.GeneratePrime(CAP / 2);
            var Q = GlobalService.GeneratePrime(CAP / 2);
            var X = GlobalService.GeneratePrime(CAP);
            var Y = GlobalService.GeneratePrime(CAP);

            var A = BigInteger.ModPow(Q, X, N);
            var B = BigInteger.ModPow(Q, Y, N);

            var kx = BigInteger.ModPow(B, X, N);
            var ky = BigInteger.ModPow(A, Y, N);

            return new Lab3Model(N, X, Q, Y, kx, ky, A, B);
        }


        public static string Encrypt(string plaintext, BigInteger kx, BigInteger N)
        {
            var sb = new StringBuilder();
            foreach (var c in plaintext)
            {
                var charValue = (BigInteger)c;
                var encryptedValue = BigInteger.ModPow(charValue, kx, N); // возводим в степень
                sb.Append(encryptedValue.ToString() + " ");
            }
            return sb.ToString().Trim();
        }

        public static string Decrypt(string ciphertext, BigInteger ky, BigInteger N)
        {
            var sb = new StringBuilder();
            var values = ciphertext.Split(' ');
            foreach (var value in values)
            {
                if (BigInteger.TryParse(value, out var encryptedValue))
                {
                    var decryptedChar = FindDiscreteLog(encryptedValue, ky, N); // типо логарифм
                    sb.Append(decryptedChar);
                }
            }
            return sb.ToString();
        }

        private static char FindDiscreteLog(BigInteger encryptedValue, BigInteger ky, BigInteger N)
        {
            for (int i = 0; i < 256; i++) // Assuming ASCII characters
            {
                var testValue = (BigInteger)i;
                var testEncryptedValue = BigInteger.ModPow(testValue, ky, N);
                if (testEncryptedValue == encryptedValue)
                {
                    return (char)i;
                }
            }
            throw new Exception("Discrete logarithm not found.");
        }


    }
}
