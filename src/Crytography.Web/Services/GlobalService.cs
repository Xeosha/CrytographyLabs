using System.Numerics;
using System.Security.Cryptography;

namespace Crytography.Web.Services
{
    public static class GlobalService
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public static BigInteger GeneratePrime(int bits)
        {
            BigInteger prime;

            do
            {
                prime = GenerateRandomBigInteger(bits);
            }
            while (!IsProbablyPrime(prime));

            return prime;
        }

        private static BigInteger GenerateRandomBigInteger(int minBits)
        {
            byte[] bytes = new byte[minBits / 8 + 1];
            BigInteger number;

            do
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= 0x7F;

                number = new BigInteger(bytes);
            }
            while (number < BigInteger.Pow(2, minBits - 1));

            return number;
        }


        private static bool IsProbablyPrime(BigInteger number)
        {
            if (number < 2) return false;
            if (number == 2 || number == 3) return true;
            if (number % 2 == 0) return false;

            for (long i = 2; i < number; i++)
                if (number % i == 0)
                    return false;

            return true;
        }
    }
}
