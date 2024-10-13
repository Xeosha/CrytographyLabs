using System.Text;

namespace Crytography.Web.Services.Lab6Services
{
    public class Lz77Coder : ICoder
    {
        public byte[] Encode(byte[] input)
        {
            var lz77Compressed = Compress(input);

            StringBuilder compressedString = new StringBuilder();
            foreach (var (offset, length, nextChar) in lz77Compressed)
            {
                compressedString.Append($"({offset}, {length}, '{nextChar}') ");
            }
            Console.WriteLine($"LZ77 Compressed data: {compressedString}");

            using (var memoryStream = new MemoryStream())
            {
                foreach (var (offset, length, nextChar) in lz77Compressed)
                {
                    memoryStream.WriteByte((byte)offset);
                    memoryStream.WriteByte((byte)length);
                    memoryStream.WriteByte(nextChar);
                }
                return memoryStream.ToArray();
            }
        }

        public byte[] Decode(byte[] input)
        {
            var lz77Compressed = new List<Tuple<int, int, byte>>();

            for (int i = 0; i < input.Length; i += 3)
            {
                int offset = input[i];
                int length = input[i + 1];
                byte nextChar = input[i + 2];
                lz77Compressed.Add(new Tuple<int, int, byte>(offset, length, nextChar));
            }

            var lz77DeCompressed = Decompress(lz77Compressed);
            return lz77DeCompressed;
        }

        public static int CalculateLZ77CompressedSize(List<Tuple<int, int, byte>> compressed)
        {
            return compressed.Count * (8 + 8 + 8) / 8; // смещение, длина совпадения, следующий символ в битах ./8 - байты
        }

        public static List<Tuple<int, int, byte>> Compress(byte[] input)
        {
            List<Tuple<int, int, byte>> output = new List<Tuple<int, int, byte>>();
            int searchBufferSize = 255; // Размер буфера поиска
            int lookaheadBufferSize = 255; // Размер буфера просмотра

            int inputLength = input.Length;
            int i = 0;

            while (i < inputLength)
            {
                int matchLength = 0;
                int matchPosition = -1;

                // Поиск совпадений в буфере поиска
                for (int j = Math.Max(0, i - searchBufferSize); j < i; j++)
                {
                    int length = 0;

                    // Сравнение подстрок
                    while (length < lookaheadBufferSize && (i + length) < inputLength && input[j + length] == input[i + length])
                    {
                        length++;
                    }

                    if (length > matchLength)
                    {
                        matchLength = length;
                        matchPosition = j;
                    }
                }

                if (matchLength > 0)
                {
                    // Добавление ссылки на совпадение
                    byte nextByte = (i + matchLength < inputLength) ? input[i + matchLength] : (byte)0;
                    output.Add(new Tuple<int, int, byte>(i - matchPosition, matchLength, nextByte));
                    i += matchLength + 1; // Перемещаемся к следующему символу после совпадения
                }
                else
                {
                    // Если совпадений нет, добавляем символ как литерал
                    output.Add(new Tuple<int, int, byte>(0, 0, input[i]));
                    i++;
                }
            }

            return output;
        }
        public static byte[] Decompress(List<Tuple<int, int, byte>> compressed)
        {
            List<byte> output = new List<byte>();

            foreach (var (offset, length, nextByte) in compressed)
            {
                // Восстановление данных
                int start = output.Count - offset;

                // Восстановление повторяющейся последовательности
                for (int i = 0; i < length; i++)
                {
                    output.Add(output[start + i]);
                }

                // Добавление следующего байта
                output.Add(nextByte);
            }

            return output.ToArray();
        }
    }
}
