using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class CancellingPractice
    {
        public void Run()
        {
            Console.Write("Введіть ім'я файлу для шифрування (default demo.txt): ");
            String filename = Console.ReadLine()!;
            if(filename.Length == 0)
            {
                filename = "demo.txt";
            }
            if (File.Exists(filename))
            {
                Console.Write("Пароль шифрування: ");
                String password = Console.ReadLine()!;
                CancellationTokenSource source = new();
                new Thread(Encrypt).Start(
                    new EncryptData(filename, password, source.Token)
                );
                Console.WriteLine("Для скасування натисність будь-яку кнопку");
                Console.ReadKey();
                source.Cancel();
            }
            else
            {
                Console.WriteLine("Файл не знайдено");
            }
        }

        private void Encrypt(Object? arg)
        {
            StringBuilder sb = new();
            try
            {
                EncryptData data = (EncryptData)arg!;
                String text = File.ReadAllText(data.Filename);
                for(int i = 0; i <  text.Length; i++) 
                {
                    char w = text[i];
                    int wi = (int)w;
                    char p = data.Password[i % data.Password.Length];
                    int pi = (int)p;
                    int ci = wi ^ pi;
                    char c = (char) ci;
                    Console.WriteLine("{4}({0}) {5}({1}) {3}({2})", wi, pi, ci, c, w, p);
                    Thread.Sleep(200);   // імітація тривалого процесу
                    sb.Append(c);

                    // перевірка скасування
                    data.Token.ThrowIfCancellationRequested();
                }

                Console.WriteLine("Результат шифрування: {0}", sb.ToString());
                File.WriteAllText("demo.enc", sb.ToString());
            }
            catch
            {
                // завершальні дії - очищаємо напрацьовані дані
                sb.Clear();
                Console.WriteLine("Шифрування скасоване");
            }
        }
    }

    record EncryptData(String Filename, String Password, CancellationToken Token);

}
/* Практика зі скасування потоків.
 * Задача: забезпечити шифрування файлу з можливістю зупинки 
 * роботи за активністю користувача
 * 
 * Шифр XOR як різновид шифрів Віженера - шифр з циклічним
 * накладанням паролю
 * Шифрування:
 * Вихідний текст секретного документу
 * ПарольПарольПарольПарольПарольПарол
 * ------------------------------------XOR
 * a;iha;isne09ub'ija;woihdg[0b9jh'aie
 * 
 * Розшифрування:
 * a;iha;isne09ub'ija;woihdg[0b9jh'aie
 * ПарольПарольПарольПарольПарольПарол
 * ------------------------------------XOR
 * Вихідний текст секретного документу
 * 
 * Базується на математичних властивостях 
 * x XOR x = 0
 * y XOR 0 = y
 * с = w XOR x
 * c XOR x = (w XOR x) XOR x = w XOR (x XOR x) = w XOR 0 = w
 */
/* Д.З. Реалізувати режим розшифрування файлів
 * з можливістю зупинки роботи з боку користувача програми.
 * Алгоритм описаний у практиці.
 * При старті програми реалізувати меню вибору:
 * 1 - Шифрування
 * 2 - Розшифрування,
 * Далі введення імені файлу та паролю.
 * Додати скріншоти різних режимів роботи (правильний пароль, 
 * неправильний пароль, зупинка роботи)
 */ 