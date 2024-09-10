
using System.Text;

namespace Crytography.Services
{
    public class LabOneService
    {
        private static readonly IEnumerable<char> _alphabat = Enumerable.Range(0x0410, 32)
                                            .Select(i => (char)i);
        public static async Task<string> EncryptAsync(string inputText, string key)
            => await Task.Run(() => Encrypt(inputText, key));

        public static string Encrypt(string inputText, string key)
        {
            inputText = inputText.Replace(" ", "").ToUpper();
            key = key.ToUpper();

            if (!IsValidText(inputText))
                return "Неверный текст";

            var playfairTable = CreateTable(key);

            var bigrams = SplitIntoBigrams(inputText);

            return EncryptBigrams(bigrams, playfairTable);
        }

        private static char[,] CreateTable(string key)
        {
            key = new string(key.Distinct().ToArray());

            var filteredAlphabet = _alphabat.Where(c => !key.Contains(c));
            var combinedChars = key.Concat(filteredAlphabet).ToArray();

            char[,] playfairTable = new char[4, 8];
            int count = 0;

            Console.WriteLine("Вывод таблицы");
            for (int i = 0; i < playfairTable.GetLength(0); i++)
            {
                for (int j = 0; j < playfairTable.GetLength(1); j++)
                {
                    playfairTable[i, j] = combinedChars[count++];
                }
            }

            return playfairTable;
        }

        private static bool IsValidText(string text)
        {
            for (int i = 0; i < text.Length - 1; i += 2)
            {
                if (text[i] == text[i + 1])
                    return false;
            }

            return text.Length % 2 == 0;
        }

        private static string[] SplitIntoBigrams(string text)
        {
            string[] bigrams = new string[text.Length / 2];
            for (int i = 0; i < text.Length; i += 2)
            {
                bigrams[i / 2] = text.Substring(i, 2);
            }
            return bigrams;
        }

        private static string EncryptBigrams(string[] bigrams, char[,] playfairTable)
        {
            string encryptedText = "";

            var rows = playfairTable.GetLength(0);
            var cols = playfairTable.GetLength(1);

            foreach (var bigram in bigrams)
            {
                var firstChar = bigram[0];
                var secondChar = bigram[1];

                var (firstRow, firstCol) = FindPosition(firstChar, playfairTable);
                var (secondRow, secondCol) = FindPosition(secondChar, playfairTable);

                if (firstRow == secondRow)
                {
                    encryptedText += playfairTable[firstRow, (firstCol + 1) % cols];
                    encryptedText += playfairTable[secondRow, (secondCol + 1) % cols];
                }
                else if (firstCol == secondCol)
                {
                    encryptedText += playfairTable[(firstRow + 1) % rows, firstCol];
                    encryptedText += playfairTable[(secondRow + 1) % rows, secondCol];
                }
                else
                {
                    encryptedText += playfairTable[firstRow, secondCol];
                    encryptedText += playfairTable[secondRow, firstCol];
                }

                encryptedText += " ";
            }

            return "Зашифрованная строка:\n" + encryptedText + "\n" + "Матрица:\n" + GetTextMatrix(playfairTable);
        }

        private static (int row, int col) FindPosition(char c, char[,] playfairTable)
        {
            for (int row = 0; row < playfairTable.GetLength(0); row++)
            {
                for (int col = 0; col < playfairTable.GetLength(1); col++)
                {
                    if (playfairTable[row, col] == c)
                    {
                        return (row, col);
                    }
                }
            }

            throw new Exception($"Буквы {c} нет в таблице.");
        }

        private static string GetTextMatrix(char[,] matrix)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    sb.Append(matrix[i, j]);
                    sb.Append(' '); // Добавляем пробел между символами
                }
                sb.AppendLine(); // Переход на следующую строку после каждой строки матрицы
            }

            return sb.ToString();
        }
    }

}
