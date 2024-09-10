using Crytography.Web.Models;
using System.Numerics;

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

        public static string Encrypt(string message, BigInteger sharedKey)
        {
            var encryptedChars = new List<string>();

            foreach (var character in message)
            {
                var charBigInt = new BigInteger(character);
                var encryptedCharBigInt = BigInteger.ModPow(charBigInt, sharedKey, sharedKey);
                encryptedChars.Add(encryptedCharBigInt.ToString());
            }

            return string.Join(" ", encryptedChars);
        }

        // Метод расшифрования с использованием общего секрета
        public static string Decrypt(string ciphertext, BigInteger sharedKey)
        {
            var encryptedChars = ciphertext.Split(' ');
            var decryptedChars = new List<char>();

            foreach (var encryptedChar in encryptedChars)
            {
                var encryptedCharBigInt = BigInteger.Parse(encryptedChar);
                var decryptedCharBigInt = BigInteger.ModPow(encryptedCharBigInt, sharedKey, sharedKey);
                var decryptedChar = (char)decryptedCharBigInt;
                decryptedChars.Add(decryptedChar);
            }

            return new string(decryptedChars.ToArray());
        }




    }
}
