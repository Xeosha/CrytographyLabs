using System.Text;

namespace Crytography.Web.Services
{
    public static class Lab4Service
    {
        private const int BlockSize = 16; // Размер блока в битах
        private const int KeySize = 32; // Размер ключа в битах

        public static string Encrypt(string text, string key)
        {
            // Проверяем длину ключа
            if (key.Length != KeySize)
            {
                throw new ArgumentException("Ключ должен быть длиной 32 бита (4 байта)");
            }

            // Преобразуем текст и ключ в байты
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Добавляем паддинг к тексту, чтобы длина была кратна размеру блока
            textBytes = PadBytes(textBytes, BlockSize / 8);

            // Делим текст на блоки
            List<byte[]> blocks = SplitBytes(textBytes, BlockSize / 8);

            // Шифруем каждый блок
            List<byte[]> encryptedBlocks = new List<byte[]>();
            foreach (byte[] block in blocks)
            {
                encryptedBlocks.Add(EncryptBlock(block, keyBytes));
            }

            // Соединяем зашифрованные блоки
            return Convert.ToBase64String(encryptedBlocks.SelectMany(b => b).ToArray());
        }

        public static string Decrypt(string cipherText, string key)
        {
            // Проверяем длину ключа
            if (key.Length != KeySize)
            {
                throw new ArgumentException("Ключ должен быть длиной 32 бита (4 байта)");
            }

            // Преобразуем зашифрованный текст и ключ в байты
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Делим зашифрованный текст на блоки
            List<byte[]> blocks = SplitBytes(cipherBytes, BlockSize / 8);

            // Расшифровываем каждый блок
            List<byte[]> decryptedBlocks = new List<byte[]>();
            foreach (byte[] block in blocks)
            {
                decryptedBlocks.Add(DecryptBlock(block, keyBytes));
            }

            byte[] decryptedBytes = decryptedBlocks.SelectMany(b => b).ToArray();

            decryptedBytes = RemovePadding(decryptedBytes);

            // Соединяем расшифрованные блоки
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] RemovePadding(byte[] bytes)
        {
            int paddingLength = bytes[bytes.Length - 1]; // Последний байт хранит длину паддинга
            byte[] result = new byte[bytes.Length - paddingLength];
            Array.Copy(bytes, result, result.Length);
            return result;
        }

        // Шифрование одного блока 
        private static byte[] EncryptBlock(byte[] block, byte[] key)
        {
            // Разделяем блок на две половины
            byte[] left = new byte[BlockSize / 16];
            byte[] right = new byte[BlockSize / 16];
            Array.Copy(block, 0, left, 0, BlockSize / 16);
            Array.Copy(block, BlockSize / 16, right, 0, BlockSize / 16);

            // Выполняем 4 раунда шифрования
            for (int i = 0; i < 4; i++)
            {
                // Ф-функция
                byte[] fResult = FeistelFunction(right, key);

                // XOR левой половины с результатом F-функции
                left = XOR(left, fResult);

                // Меняем половины местами
                byte[] temp = left;
                left = right;
                right = temp;
            }

            // Соединяем половины обратно в один блок
            byte[] result = new byte[BlockSize / 8];
            Array.Copy(left, 0, result, 0, BlockSize / 16);
            Array.Copy(right, 0, result, BlockSize / 16, BlockSize / 16);
            return result;
        }

        // Расшифрование одного блока
        private static byte[] DecryptBlock(byte[] block, byte[] key)
        {
            // Разделяем блок на две половины
            byte[] left = new byte[BlockSize / 16];
            byte[] right = new byte[BlockSize / 16];
            Array.Copy(block, 0, left, 0, BlockSize / 16);
            Array.Copy(block, BlockSize / 16, right, 0, BlockSize / 16);

            // Выполняем 4 раунда расшифрования
            for (int i = 0; i < 4; i++)
            {
                // Меняем половины местами
                byte[] temp = left;
                left = right;
                right = temp;

                // XOR левой половины с результатом F-функции
                byte[] fResult = FeistelFunction(right, key);
                left = XOR(left, fResult);
            }

            // Соединяем половины обратно в один блок
            byte[] result = new byte[BlockSize / 8];
            Array.Copy(left, 0, result, 0, BlockSize / 16);
            Array.Copy(right, 0, result, BlockSize / 16, BlockSize / 16);
            return result;
        }

        // F-функция
        private static byte[] FeistelFunction(byte[] input, byte[] key)
        {
            // Простая реализация F-функции: XOR с ключом
            return XOR(input, key);
        }

        // XOR двух массивов байтов
        private static byte[] XOR(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (byte)(a[i] ^ b[i]);
            }
            return result;
        }

        // Добавляет паддинг к массиву байтов
        private static byte[] PadBytes(byte[] bytes, int blockSize)
        {
            int paddingLength = blockSize - (bytes.Length % blockSize);
            byte[] paddedBytes = new byte[bytes.Length + paddingLength];
            bytes.CopyTo(paddedBytes, 0);
            for (int i = 0; i < paddingLength; i++)
            {
                paddedBytes[bytes.Length + i] = (byte)paddingLength;
            }
            return paddedBytes;
        }

        // Делит массив байтов на блоки
        private static List<byte[]> SplitBytes(byte[] bytes, int blockSize)
        {
            List<byte[]> blocks = new List<byte[]>();
            for (int i = 0; i < bytes.Length; i += blockSize)
            {
                byte[] block = new byte[blockSize];
                Array.Copy(bytes, i, block, 0, blockSize);
                blocks.Add(block);
            }
            return blocks;
        }
    }
}
