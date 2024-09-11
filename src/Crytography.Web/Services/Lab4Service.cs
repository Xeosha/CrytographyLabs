using System.Text;

namespace Crytography.Web.Services
{
    public static class Lab4Service
    {
        private const int BLOCK_SIZE = 16; // 16 бит (2 байта)
        private const int CHAR_SIZE = 8; // 8 бит для одного символа
        private const int KEY_SIZE = 32; // 32 бита (4 байта)
        private const int ROUNDS = 16; // Количество раундов


        private static string XOR(string s1, string s2)
        {
            var result = new StringBuilder();
            for (int i = 0; i < s1.Length; i++)
            {
                result.Append(s1[i] == s2[i] ? '0' : '1');
            }
            return result.ToString();
        }

        private static string FFunction(string halfBlock, string roundKey)
        {
            return XOR(halfBlock, roundKey);
        }

        private static string ShiftKey(string key, int round)
        {
            return key.Substring(round % KEY_SIZE) + key.Substring(0, round % KEY_SIZE);
        }

        public static string Encrypt(string plaintext, string key)
        {
            if (key.Length != KEY_SIZE / CHAR_SIZE)
                throw new ArgumentException($"Key must be {KEY_SIZE / CHAR_SIZE} characters long.");

            var plaintextBinary = ToBinaryString(plaintext);
            var blocks = DivideIntoBlocks(plaintextBinary);

            var result = new StringBuilder();
            foreach (var block in blocks)
            {
                var left = block.Substring(0, BLOCK_SIZE / 2);
                var right = block.Substring(BLOCK_SIZE / 2);

                for (int i = 0; i < ROUNDS; i++)
                {
                    var roundKey = ShiftKey(key, i);
                    var newRight = XOR(left, FFunction(right, roundKey));
                    left = right;
                    right = newRight;
                }

                result.Append(left + right);
            }

            return FromBinaryString(result.ToString());
        }

        public static string Decrypt(string ciphertext, string key)
        {
            if (key.Length != KEY_SIZE / CHAR_SIZE)
                throw new ArgumentException($"Key must be {KEY_SIZE / CHAR_SIZE} characters long.");

            var ciphertextBinary = ToBinaryString(ciphertext);
            var blocks = DivideIntoBlocks(ciphertextBinary);

            var result = new StringBuilder();
            foreach (var block in blocks)
            {
                var left = block.Substring(0, BLOCK_SIZE / 2);
                var right = block.Substring(BLOCK_SIZE / 2);

                for (int i = ROUNDS - 1; i >= 0; i--)
                {
                    var roundKey = ShiftKey(key, i);
                    var newLeft = XOR(right, FFunction(left, roundKey));
                    right = left;
                    left = newLeft;
                }

                result.Append(left + right);
            }

            return FromBinaryString(result.ToString());
        }

        private static string ToBinaryString(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(CHAR_SIZE, '0'));
            }
            return sb.ToString();
        }

        private static string FromBinaryString(string binaryString)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < binaryString.Length; i += CHAR_SIZE)
            {
                var byteString = binaryString.Substring(i, CHAR_SIZE);
                sb.Append((char)Convert.ToInt32(byteString, 2));
            }
            return sb.ToString();
        }

        private static string[] DivideIntoBlocks(string binaryString)
        {
            var blockCount = binaryString.Length / BLOCK_SIZE;
            var blocks = new string[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                blocks[i] = binaryString.Substring(i * BLOCK_SIZE, BLOCK_SIZE);
            }
            return blocks;
        }
    }
}
