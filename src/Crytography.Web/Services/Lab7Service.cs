using System.Collections.Generic;
using System.Text;

namespace Crytography.Web.Services
{
    public class Lab7Service
    {
        private static readonly int MaxPasswordLength = 18; // Максимальная длина пароля
        private static string _KEY = "ACAB";

        public static string HashPassword(string password)
        {
            if (password.Length < 4 || password.Length > MaxPasswordLength)
                throw new ArgumentException($"Пароль должен быть длиной от 4 до {MaxPasswordLength} символов.");

            string hash = "";
            string keyHash = _KEY;

            for (int i = 0; i <= 5; i++)
            {
                var encrypted = Lab4Service.Encrypt(password, Encoding.UTF8.GetBytes(keyHash));
                var firstFourSymbols = encrypted.Substring(0, 4);
                var secondFoutSymbols = encrypted.Substring(4, 4);
                keyHash = secondFoutSymbols;

                if (i == 0)
                    continue;
                hash += firstFourSymbols;
            }

            Console.WriteLine("Захешированный пароль: " + hash);

            // Возвращаем хеш как строку
            return hash;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Хешируем предоставленный пароль
            var hashed = HashPassword(password);

            Console.WriteLine("Проверка пароля: " + hashed);

            // Сравниваем хеши
            return hashed == hashedPassword;
        }
    }
}
