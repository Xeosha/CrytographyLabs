namespace Crytography.Web.Services.Lab6Services
{
    public class ArithmeticCoder : ICoder
    {
        private List<char> _alphabet = new();
        private List<decimal> _probabilities = new();
        private int _inputLenght;

        public ArithmeticCoder(int inputLenght)
        {
            _alphabet = new List<char>();
            _probabilities = new List<decimal>();
            _inputLenght = inputLenght;

            // Fill alphabet and probabilities
            foreach (char c in "abcdefghijklmnopqrstuvwxyz ")
            {
                _alphabet.Add(c);
                _probabilities.Add(1.0m / 27); // Approximate equal probabilities
            }
        }

        public string Decode(byte[] input)
        {
            var stringInput = GlobalService.binaryToString(input);
            return DecompressString(stringInput, _alphabet, _probabilities, _inputLenght, 20);
        }

        public byte[] Encode(string input)
        {
            var res = CompressString(input, _alphabet, _probabilities, 20);
            return GlobalService.binaryStringToByteArray(res);
        }


        static string CompressString(string input, List<char> alphabet, List<decimal> probabilities, int blockSize = 16)
        {
            var result = new List<string>();

            for (int i = 0; i < input.Length; i += blockSize)
            {
                var block = input.Substring(i, Math.Min(blockSize, input.Length - i));
                var encodedValue = ArithmeticEncode(block, alphabet, probabilities);
                var binaryData = GlobalService.decimalToBinary(encodedValue);
                var binaryString = GlobalService.binaryToString(binaryData);
                result.Add(binaryString);

                //Console.WriteLine($"Original string: {block} - {block.Length * sizeof(char)} bytes");
                //Console.WriteLine($"Encoded value: {encodedValue}  {binaryString} - {sizeof(decimal)} bytes");
            }

            return string.Join("", result);
        }
        static string DecompressString(string encodedString, List<char> alphabet, List<decimal> probabilities, int originalLength, int blockSize = 16)
        {
            string decodedString = "";
            int size = sizeof(decimal) * 8; // Длина в битах для decimal
            int totalBlocks = (int)Math.Ceiling((double)originalLength / blockSize); // Количество блоков

            for (int i = 0; i < totalBlocks; i++)
            {
                // Рассчитываем начальный индекс и длину блока
                int start = i * size; // Начальный индекс в битах
                int length = Math.Min(size, encodedString.Length - start); // Длина блока в битах

                // Извлекаем блок
                var block = encodedString.Substring(start, length);
                var binaryData = GlobalService.binaryStringToByteArray(block);
                var encodedValue = GlobalService.byteArrayToDecimal(binaryData);
                var decodedBlock = ArithmeticDecode(encodedValue, alphabet, probabilities, Math.Min(blockSize, originalLength - decodedString.Length));
                decodedString += decodedBlock;

                //Console.WriteLine($"Original block: {decodedBlock}");
                //Console.WriteLine($"Decoded value: {encodedValue}  {block}");
            }

            return decodedString;
        }


        static decimal ArithmeticEncode(string input, List<char> alphabet, List<decimal> probabilities, int blockSize = 16)
        {
            decimal low = 0.0m;
            decimal high = 1.0m;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                int index = alphabet.IndexOf(c);
                if (index < 0) throw new ArgumentException($"Character '{c}' not in alphabet."); // Проверка на наличие символа
                decimal range = high - low;
                high = low + range * (GetCumulativeProbability(probabilities, index + 1));
                low = low + range * GetCumulativeProbability(probabilities, index);
            }

            return (low + high) / 2; // Возвращаем среднее значение
        }
        static decimal GetCumulativeProbability(List<decimal> probabilities, int index)
        {
            decimal cumulative = 0.0m;
            for (int i = 0; i < index; i++)
            {
                cumulative += probabilities[i];
            }
            return cumulative;
        }
        static string ArithmeticDecode(decimal encodedValue, List<char> alphabet, List<decimal> probabilities, int length)
        {
            decimal low = 0.0m;
            decimal high = 1.0m;
            string decodedString = "";

            for (int i = 0; i < length; i++)
            {
                decimal range = high - low;

                // Проверка на возможность деления
                if (range == 0) throw new InvalidOperationException("Invalid range during decoding.");

                decimal value = (encodedValue - low) / range;

                for (int j = 0; j < alphabet.Count; j++)
                {
                    decimal cumulativeProb = GetCumulativeProbability(probabilities, j + 1);
                    if (value < cumulativeProb)
                    {
                        decodedString += alphabet[j];
                        high = low + range * cumulativeProb;
                        low = low + range * GetCumulativeProbability(probabilities, j);
                        break;
                    }
                }
            }

            return decodedString;
        }
    }
}
