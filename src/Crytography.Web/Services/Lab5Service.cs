namespace Crytography.Web.Services
{
    public static class Lab5Service
    {
        public static bool IsBinaryString(string input)
        {
            return input.All(c => c == '0' || c == '1');
        }

        public static int CalculateParityBit(string binaryString)
        {
            int count = binaryString.Count(c => c == '1');
            return count % 2 == 0 ? 0 : 1; // Возвращаем 0 если четное количество, иначе 1
        }
    }
}
