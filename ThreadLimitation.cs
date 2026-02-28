using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class ThreadLimitation
    {
        private readonly Semaphore semaphore =     // Семафор - об'єкт синхронізації, що дозволяє одначасну
            new(initialCount:0, maximumCount:5);   // роботу кількох потоків, кількість яких задається maximumCount
                                                   // initialCount - початкова кількість "вільних місць"
        private long startTicks;

        public void Run()
        {
            startTicks = DateTime.Now.Ticks;
            Thread[] threads = [..Enumerable.Range(1, 20).Select(_ => new Thread(ThreadMethod))];
            for(int i = 1; i <= 20; i += 1)
            {
                threads[i-1].Start(i);
            }
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Loop finish - threads ready. Pause before start");
            Thread.Sleep(1000);
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Semaphore released");
            semaphore.Release(5);
            // очікування потоків - для того щоб Main не завершувався раніше за потоки
            // Join() - виклик очікування, який треба подати для найдовшого з потоків, а якщо
            // це не відомо, то для кожного (якщо потік вже завершений, то виклик ігнорується)
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private void ThreadMethod(Object? param)
        {
            semaphore.WaitOne();
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Thread {param} Start");
            Thread.Sleep(1000);
            Console.WriteLine($"{(DateTime.Now.Ticks - startTicks) / 1e7:F1} Thread {param} Finish");
            semaphore.Release();
        }
    }
}
/* Обмеження потоків
 * 1) Обмеження за кількістю
 * Багатопоточність дозволяє пришвидшувати виконання програми
 * за рахунок задіяння додаткових фізичних виконавців (процесорів)
 * але коли потоків забагато, система не може їх обслуговувати
 * одночасно, перемикаючись між певною кількістю, що відповідає
 * архітектурі (кількості процесорів). За наявності дуже великої
 * кількості потоків переваги нівелюються постійним їх перемиканням,
 * через що час роботи може навіть збільшитись у порівнянні з
 * синхронним виконанням.
 * Семафор - інструмент обмеження кількості одночасно працюючих
 * потоків. Не замінює критичну секцію у контексті звернення до
 * спільних ресурсів. В реальних задачах комбінуються:
 * семафор обмежує одночасну кількість
 * критична секція (lock) обмежує конкуренцію зі спільним ресурсом
 * 
 */
