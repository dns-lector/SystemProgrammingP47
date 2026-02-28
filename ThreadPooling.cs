using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class ThreadPooling
    {
        private long startTicks;
        private readonly Semaphore semaphore = new(0, 1);


        public void Run()
        {
            startTicks = DateTime.Now.Ticks;
            sum = 100.0;
            cnt = 12;
            for (int i = 1; i <= 12; i += 1)
            {
                ThreadPool.QueueUserWorkItem(LoadPercent, i);   // додає потік та стартує виконання
            }

            // зупинка основного потоку призводить до скасування усіх фонових потоків
            // враховуючи, що у C# немає режиму очікування пулу потоків, на відміну від
            // інших мов, де до очікування додається граничний час, після якого відбувається
            // примусове скасування, аналог може бути створений через додатковий
            // Thread.Sleep в основному потоці
            // Thread.Sleep(10000);
            semaphore.WaitOne();
        }

        // 
        private void ThreadMethod(Object? param)
        {
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Thread {param} Start");
            Thread.Sleep(1000);
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Thread {param} Finish");
        }

        private double sum;
        private readonly Object sumLocker = new();   // об'єкт, створений заради критичної секції
        private int cnt;

        private void LoadPercent(Object? arg)  // arg - int = номер місяця 
        {
            if (arg is int month)
            {
                Console.WriteLine($"Load start month {month}");
                Thread.Sleep(1000);   // імітація тривалості запиту
                double percent = month;
                double k = 1.0 + percent / 100.0;
                double res;
                bool isLast;
                lock (sumLocker)
                {
                    res = sum;
                    res = res * k;
                    sum = res;
                    cnt = cnt - 1;
                    isLast = cnt == 0;  // локально - фіксуємо стан на момент синхронізованої операції
                                        // Console.WriteLine($"Load finish month {month}, sum = {sum}");   // усі звернення до спільного ресурсу синхронізуються
                }
                Console.WriteLine($"Load finish month {month}, sum = {res}");   // локальна змінна (res) дозволяє винести код з блока

                if (isLast)   // (cnt == 0) - неправильно, глобальна змінна напевно змінена іншими потоками
                {
                    Console.WriteLine($"Total: {res}");
                    semaphore.Release();
                }
            }
            else
            {
                Console.WriteLine("arg must be int, not " + (arg?.GetType().Name ?? "NULL"));
            }
        }
    }
}
/* Обмеження потоків
 * 2) Зниження пріоритету
 * Пріоритети потоків:
 * - realtime - найвищий пріоритет, який не рекомендується для прикладних програм
 * - normal - середній пріоритет, як у Main: робота таких потоків не зупиняє процес (навіть якщо Main зупинився)
 * - background - фоновий пріоритет, потоки автоматично знищуються коли завершаються normal-потоки
 * Для потоків фонового пріоритету створюється пул потоків (ThreadPool), який автоматично
 * керує їх виконанням.
 * ThreadPool.QueueUserWorkItem - додавання до пулу потоків нового потоку (worker), який 
 * буде запущений з фоновим пріоритетом.
 * На відміну від Thread, ThreadPool має лише один варіант потокового методу: void method(Object?)
 * Пул потоків зазвичай сам балансує кількість одночасних робіт і не рекомендується 
 * втручатись у його налаштування.
 */
/* Д.З. Виконати попереднє ДЗ
 * а) обмеживши загальну кількість потоків - не більше 3 одночасно
 * б) за допомогою пулу потоків
 * 
 * Попереднє ДЗ: Користувач вводить число (наприклад, 10)
 * програма запускає відповідну кількість потоків, кожен з яких
 * додає до спільного ресурсу-рядка (string) літеру з відповідним
 * порядком: (10 перших - це abcdefghij)
 * У результаті через конкуренцію порядок літер має виглядати
 * випадковим.
 * Також реалізувати виведення проміжних результатів кожного потоку.
 * Очікуваний вигляд консолі:
 * Enter num: 10
 * Processing letter 5 (e): ce
 * Processing letter 1 (a): cea
 * Processing letter 3 (c): c
 * ....
 * Result: ceaijhbafg
 */