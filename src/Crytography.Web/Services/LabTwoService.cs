using Crytography.Web.Models;
using Crytography.Web.Services;
using System.Numerics;

namespace Cryptography.Web.Services
{
    public static class LabTwoService
    {
        private static readonly short cap = 16;

        public static Lab2Model GenerateKeys()
        {
            var P = GlobalService.GeneratePrime(cap);
            var Q = GlobalService.GeneratePrime(cap);

            var N = BigInteger.Multiply(P, Q);
            var phi = BigInteger.Multiply(P - 1, Q - 1);

            BigInteger E;
            do
            {
                E = GlobalService.GeneratePrime(cap);
            } while (E >= phi && GCD(E, phi) != 1);

            var (D, Y) = ExtendedGCD(E, phi);

            if (D < 0)
                D += phi;


            return new Lab2Model(P, Q, N, E, D, Y);
        }

        private static (BigInteger x, BigInteger y) ExtendedGCD(BigInteger a, BigInteger b)
        {
            if (a == 0)
                return (0, 1);

            var (x1, y1) = ExtendedGCD(b % a, a);

            var x = y1 - (b / a) * x1;
            var y = x1;

            return (x, y);
        }

        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }


        public static string Encrypt(string plaintext, BigInteger E, BigInteger N)
        {
            var encryptedChars = new List<string>();

            foreach (var character in plaintext)
            {
                // Преобразуем символ в BigInteger
                var charBigInt = new BigInteger(character);

                // Шифруем BigInteger
                var encryptedCharBigInt = BigInteger.ModPow(charBigInt, E, N);

                // Сохраняем зашифрованный BigInteger как строку десятичного числа
                encryptedChars.Add(encryptedCharBigInt.ToString());
                //encryptedChars.Add(Convert.ToBase64String(encryptedCharBigInt.ToByteArray()));
            }

            // Соединяем зашифрованные значения через специальный разделитель (например, пробел)
            return string.Join(" ", encryptedChars);
        }

        public static string Decrypt(string ciphertext, BigInteger D, BigInteger N)
        {
            var encryptedChars = ciphertext.Split(' ');
            var decryptedChars = new List<char>();

            foreach (var encryptedChar in encryptedChars)
            {
                // Преобразуем строку в BigInteger
                var encryptedCharBigInt = BigInteger.Parse(encryptedChar);

                //var encryptedCharBytes = Convert.FromBase64String(encryptedChar);
                //var encryptedCharBigInt = new BigInteger(encryptedCharBytes);

                // Расшифровываем BigInteger
                var decryptedCharBigInt = BigInteger.ModPow(encryptedCharBigInt, D, N);

                // Преобразуем расшифрованный BigInteger обратно в символ
                var decryptedChar = (char)decryptedCharBigInt;

                // Добавляем расшифрованный символ в список
                decryptedChars.Add(decryptedChar);
            }

            // Преобразуем список символов обратно в строку
            return new string(decryptedChars.ToArray());
        }

    }
}
