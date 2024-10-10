
using System.Text;
using Crytography.Web.Services;

namespace Cryptography.Web.Services
{
    public class Lab6Service
    {
        public static string CompressString(string input, List<char> alphabet, List<decimal> probabilities, int blockSize = 16)
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
        public static string DecompressString(string encodedString, List<char> alphabet, List<decimal> probabilities, int originalLength, int blockSize = 16)
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



        private static decimal ArithmeticEncode(string input, List<char> alphabet, List<decimal> probabilities, int blockSize = 16)
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
        private static decimal GetCumulativeProbability(List<decimal> probabilities, int index)
        {
            decimal cumulative = 0.0m;
            for (int i = 0; i < index; i++)
            {
                cumulative += probabilities[i];
            }
            return cumulative;
        }
        private static string ArithmeticDecode(decimal encodedValue, List<char> alphabet, List<decimal> probabilities, int length)
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
        private const int BufferSize = 256; // размер буфера
        public static int CalculateLZ77CompressedSize(List<Tuple<int, int, char>> compressed)
        {
            return compressed.Count * (8 + 8 + 8) / 8; // смещение, длина совпадения, следующий символ в битах ./8 - байты
        }
        public static List<Tuple<int, int, char>> Compress(string input)
        {
            var result = new List<Tuple<int, int, char>>();
            int i = 0;

            while (i < input.Length)
            {
                int matchLength = 0;
                int matchPosition = 0;

                // Поиск наибольшего совпадения
                for (int j = Math.Max(0, i - BufferSize); j < i; j++)
                {
                    int length = 0;
                    while (length < BufferSize && i + length < input.Length && input[j + length] == input[i + length])
                    {
                        length++;
                    }

                    if (length > matchLength)
                    {
                        matchLength = length;
                        matchPosition = j;
                    }
                }

                // Если совпадения найдены, добавляем их
                if (matchLength > 0)
                {
                    result.Add(new Tuple<int, int, char>(i - matchPosition, matchLength, i + matchLength < input.Length ? input[i + matchLength] : '\0'));
                    i += matchLength + 1; // переходим к следующему символу после совпадения
                }
                else
                {
                    result.Add(new Tuple<int, int, char>(0, 0, input[i]));
                    i++;
                }
            }

            return result;
        }
        public static string Decompress(List<Tuple<int, int, char>> compressed)
        {
            var output = new StringBuilder();

            foreach (var (offset, length, nextChar) in compressed)
            {
                int start = output.Length - offset;
                for (int i = 0; i < length; i++)
                {
                    output.Append(output[start + i]);
                }
                if (nextChar != '\0')
                {
                    output.Append(nextChar);
                }
            }

            return output.ToString();
        }
    }

}
