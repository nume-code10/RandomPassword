using System;
using System.Linq;
using System.Security.Cryptography;

public class PasswordGenerator
{
    // Символы, которые можно использовать в пароле
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    public static string GeneratePassword(int length)
    {
        // Проверяем, что длина находится в допустимом диапазоне
        if (length < 8 || length > 16)
        {
            throw new ArgumentException("Длина пароля должна быть от 8 до 16 символов", nameof(length));
        }

        // Объединяем все возможные символы
        string allChars = Lowercase + Uppercase + Digits + SpecialChars;

        // Используем криптографически безопасный генератор случайных чисел
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                // Преобразуем байт в индекс в диапазоне allChars
                chars[i] = allChars[randomBytes[i] % allChars.Length];
            }

            return new string(chars);
        }
    }

    // Метод с гарантией, что пароль содержит хотя бы по одному символу каждого типа
    public static string GenerateStrongPassword(int length)
    {
        if (length < 8 || length > 16)
        {
            throw new ArgumentException("Длина пароля должна быть от 8 до 16 символов", nameof(length));
        }

        using (var rng = RandomNumberGenerator.Create())
        {
            var chars = new char[length];
            string allChars = Lowercase + Uppercase + Digits + SpecialChars;

            // Генерируем остальные символы
            byte[] randomBytes = new byte[length - 4];
            rng.GetBytes(randomBytes);

            // Гарантируем наличие символов каждого типа
            chars[0] = GetRandomCharFromSet(Lowercase, rng);
            chars[1] = GetRandomCharFromSet(Uppercase, rng);
            chars[2] = GetRandomCharFromSet(Digits, rng);
            chars[3] = GetRandomCharFromSet(SpecialChars, rng);

            // Заполняем оставшиеся позиции
            for (int i = 4; i < length; i++)
            {
                chars[i] = allChars[randomBytes[i - 4] % allChars.Length];
            }

            // Перемешиваем символы для большей случайности
            return new string(ShuffleArray(chars, rng));
        }
    }

    private static char GetRandomCharFromSet(string charSet, RandomNumberGenerator rng)
    {
        byte[] randomByte = new byte[1];
        rng.GetBytes(randomByte);
        return charSet[randomByte[0] % charSet.Length];
    }

    private static char[] ShuffleArray(char[] array, RandomNumberGenerator rng)
    {
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            byte[] randomByte = new byte[1];
            rng.GetBytes(randomByte);
            int j = randomByte[0] % (i + 1);

            // Меняем местами
            char temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }
}

// Пример использования
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Генератор паролей ===");
        Console.WriteLine("Выберите вариант:");
        Console.WriteLine("1. Простой пароль");
        Console.WriteLine("2. Надежный пароль (содержит символы всех типов)");
        Console.Write("Ваш выбор (1 или 2): ");

        string choice = Console.ReadLine();

        Console.Write("Введите длину пароля (8-16): ");
        if (int.TryParse(Console.ReadLine(), out int length))
        {
            try
            {
                string password;

                if (choice == "2")
                {
                    password = PasswordGenerator.GenerateStrongPassword(length);
                    Console.WriteLine($"\nСгенерированный пароль: {password}");
                    Console.WriteLine("Содержит строчные буквы, заглавные буквы, цифры и специальные символы");
                }
                else
                {
                    password = PasswordGenerator.GeneratePassword(length);
                    Console.WriteLine($"\nСгенерированный пароль: {password}");
                }

                // Дополнительная информация
                Console.WriteLine($"Длина: {password.Length} символов");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ввод длины!");
        }

        // Пример генерации нескольких паролей
        Console.WriteLine("\n=== Примеры паролей ===");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"{i + 1}. {PasswordGenerator.GeneratePassword(12)}");
        }
    }
}
