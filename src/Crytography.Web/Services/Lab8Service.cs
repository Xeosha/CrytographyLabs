using System.Text;

namespace Crytography.Web.Services
{
    public class Lab8Service
    {
        private const int BlockSize = 32; // Размер блока в битах

        // Метод для генерации гаммы

        private static uint GenerateGamma()
        {
            Random random = new Random();
            uint gamma = 0;

            for (int i = 0; i < BlockSize; i++)
            {
                gamma <<= 1; // Сдвигаем влево
                gamma |= (uint)(random.Next(0, 2)); // Добавляем случайный бит (0 или 1)
            }

            return gamma;
        }

        public static uint[] GenerateListGamma(int length)
        {
            var gammaBlocks = new uint[length];

            for (int i = 0; i < length; i++)
            {
                var gamma = GenerateGamma(); // Генерация гаммы для каждого блока
                gammaBlocks[i] = gamma;
            }

            return gammaBlocks;
        }


        // Метод для шифрования сообщения
        public static uint Encrypt(uint plaintext, uint gamma)
        {
            return plaintext ^ gamma; // Операция XOR
        }

        public static string Encrypt(string plaintext, uint[] gammaLst)
        {
            var plaintextBlocks = ConvertToBlocks(Encoding.UTF8.GetBytes(plaintext));

            var ciphertextBlocks = new uint[plaintextBlocks.Length];


            for (int i = 0; i < plaintextBlocks.Length; i++)
            {
                ciphertextBlocks[i] = Encrypt(plaintextBlocks[i], gammaLst[i]);
                Console.WriteLine($"Блок {i}: гамма = {Convert.ToString(gammaLst[i], 2).PadLeft(BlockSize, '0')} | шифротекст = {Convert.ToString(ciphertextBlocks[i], 2).PadLeft(BlockSize, '0')}");
            }


            byte[] ciphertextBytes = ConvertBlocksToBytes(ciphertextBlocks);

            return Convert.ToBase64String(ciphertextBytes);
        }

        public static string Decrypt(string ciphertext, uint[] gammaLst)
        {
            var ciphertextBytes = Convert.FromBase64String(ciphertext);
            var ciphertextBlocks = ConvertToBlocks(ciphertextBytes);

            // Проверка на совпадение длины гаммы с длиной блоков шифротекста
            if (gammaLst.Length < ciphertextBlocks.Length)
            {
                throw new ArgumentException("Недостаточно гаммы для расшифрования.");
            }

            // Расшифровка блоков
            List<byte> decryptedBytes = new List<byte>();
            for (int i = 0; i < ciphertextBlocks.Length; i++)
            {
                uint gamma = gammaLst[i];
                uint decryptedBlock = Encrypt(ciphertextBlocks[i], gamma);

                // Заполнение расшифрованного массива байтов
                for (int j = 0; j < 4; j++)
                {
                    decryptedBytes.Add((byte)((decryptedBlock >> (24 - j * 8)) & 0xFF));
                }
            }

            // Удаление лишних нулей (если они есть)
            while (decryptedBytes.Count > 0 && decryptedBytes[^1] == 0)
            {
                decryptedBytes.RemoveAt(decryptedBytes.Count - 1);
            }

            // Преобразование расшифрованного массива байтов в строку
            return ConvertBytesToString(decryptedBytes.ToArray());
        }

        // Метод для преобразования строки в массив 32-битных блоков
        public static uint[] ConvertToBlocks(byte[] messageBytes)
        {
            int blockCount = (messageBytes.Length + 3) / 4; // Количество блоков по 4 байта
            uint[] blocks = new uint[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                // Получаем 4 байта для блока, дополнительно заполняем нулями
                uint block = 0;
                for (int j = 0; j < 4; j++)
                {
                    if (i * 4 + j < messageBytes.Length)
                    {
                        block |= (uint)(messageBytes[i * 4 + j] << (24 - j * 8));
                    }
                }
                blocks[i] = block;
            }

            return blocks;
        }

        // Метод для преобразования массива блоков в массив байтов
        public static byte[] ConvertBlocksToBytes(uint[] blocks)
        {
            byte[] bytes = new byte[blocks.Length * 4];
            for (int i = 0; i < blocks.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bytes[i * 4 + j] = (byte)((blocks[i] >> (24 - j * 8)) & 0xFF);
                }
            }
            return bytes;
        }

        // Метод для преобразования массива байтов в строку
        public static string ConvertBytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
