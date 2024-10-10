    using System.Numerics;
using System.Security.Cryptography;

namespace Crytography.Web.Services
{
    public static class GlobalService
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static byte[] decimalToBinary(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            return bits.SelectMany(b => BitConverter.GetBytes(b)).ToArray();
        }

        public static string binaryToString(byte[] binaryData)
        {
            return string.Join("", binaryData.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        public static byte[] binaryStringToByteArray(string binaryString)
        {
            int numOfBytes = binaryString.Length / 8;
            byte[] byteArray = new byte[numOfBytes];

            for (int i = 0; i < numOfBytes; i++)
            {
                byteArray[i] = Convert.ToByte(binaryString.Substring(8 * i, 8), 2);
            }

            return byteArray;
        }

        public static decimal byteArrayToDecimal(byte[] byteArray)
        {
            int[] bits = new int[4]; // decimal состоит из 4 int
            for (int i = 0; i < byteArray.Length && i < 16; i++)
            {
                bits[i / 4] |= (byteArray[i] << ((i % 4) * 8)); // Объединяем байты в int
            }
            return new decimal(bits);
        }

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
