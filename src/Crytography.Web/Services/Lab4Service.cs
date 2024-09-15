using System.Text;

namespace Crytography.Web.Services
{
    public static class Lab4Service
    {
        private static readonly int BlockSize = 2; // 2 байта (16 бит)
        private static readonly int KeySize = 4; // 4 байта (32 бита)
        private static readonly int Rounds = 8; // Количество раундов в сети Фейштеля

        public static string Encrypt(string plaintext, byte[] key)
        {
            if (key.Length != KeySize)
                throw new ArgumentException($"Ключ должен быть {KeySize} байта длиной.");

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // Разбиваем текст на блоки по 2 байта
            int blocksCount = (int)Math.Ceiling((double)plaintextBytes.Length / BlockSize);
            byte[] ciphertext = new byte[blocksCount * BlockSize];

            for (int i = 0; i < blocksCount; i++)
            {
                byte[] block = new byte[BlockSize];
                Array.Copy(plaintextBytes, i * BlockSize, block, 0, Math.Min(BlockSize, plaintextBytes.Length - i * BlockSize));

                // Заполняем блок до 2 байт, если не хватает
                if (block.Length < BlockSize)
                {
                    Array.Resize(ref block, BlockSize);
                }   

                // Шифруем блок
                byte[] encryptedBlock = FeistelNetwork(block, key, true);
                Array.Copy(encryptedBlock, 0, ciphertext, i * BlockSize, BlockSize);
            }

            return Convert.ToBase64String(ciphertext);
        }

        // Дешифрование текста
        public static string Decrypt(string ciphertext, byte[] key)
        {
            if (key.Length != KeySize)
                throw new ArgumentException($"Ключ должен быть {KeySize} байта длиной.");

            byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);

            int blocksCount = ciphertextBytes.Length / BlockSize;
            byte[] plaintextBytes = new byte[blocksCount * BlockSize];

            for (int i = 0; i < blocksCount; i++)
            {
                byte[] block = new byte[BlockSize];
                Array.Copy(ciphertextBytes, i * BlockSize, block, 0, BlockSize);

                // Дешифруем блок
                byte[] decryptedBlock = FeistelNetwork(block, key, false);
                Array.Copy(decryptedBlock, 0, plaintextBytes, i * BlockSize, BlockSize);
            }

            return Encoding.UTF8.GetString(plaintextBytes).TrimEnd('\0'); // Убираем лишние null байты
        }

        // Функция сети Фейштеля (F-функция)
        private static byte[] FeistelFunction(byte[] halfBlock, byte[] roundKey)
        {
            byte[] result = new byte[halfBlock.Length];
            for (int i = 0; i < halfBlock.Length; i++)
            {
                result[i] = (byte)(halfBlock[i] ^ roundKey[i % roundKey.Length]);
            }
            return result;
        }

        // Реализация сети Фейштеля
        private static byte[] FeistelNetwork(byte[] block, byte[] key, bool encrypt)
        {
            byte[] left = new byte[BlockSize / 2];
            byte[] right = new byte[BlockSize / 2];

            // Разделяем блок на левую и правую части
            Array.Copy(block, 0, left, 0, BlockSize / 2);
            Array.Copy(block, BlockSize / 2, right, 0, BlockSize / 2);

            // Создание раундовых ключей
            byte[][] roundKeys = GenerateRoundKeys(key);

            if (!encrypt)
            {
                Array.Reverse(roundKeys); // Для дешифрования ключи используются в обратном порядке
            }

            // Раунды Фейштеля
            for (int round = 0; round < Rounds; round++)
            {
                byte[] temp = right;
                right = XOR(left, FeistelFunction(right, roundKeys[round]));
                left = temp;
            }

            // Соединяем левую и правую части
            byte[] result = new byte[BlockSize];
            Array.Copy(right, 0, result, 0, BlockSize / 2);
            Array.Copy(left, 0, result, BlockSize / 2, BlockSize / 2);

            return result;
        }

        // Генерация раундовых ключей
        private static byte[][] GenerateRoundKeys(byte[] key)
        {
            byte[][] roundKeys = new byte[Rounds][];
            for (int i = 0; i < Rounds; i++)
            {
                roundKeys[i] = new byte[BlockSize / 2];
                Array.Copy(key, (i % key.Length), roundKeys[i], 0, BlockSize / 2);
            }
            return roundKeys;
        }

        // Операция XOR для двух массивов
        private static byte[] XOR(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }
            return result;
        }
    }
}
