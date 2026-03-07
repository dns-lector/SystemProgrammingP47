using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SystemProgrammingP47
{
    internal class CancellingDemo
    {
        private readonly Stopwatch stopwatch = new();

        public void Run()
        {
            Console.WriteLine("Скасування потоків та задач");
            CancellationTokenSource source = new();   // джерело токенів - "центр управління" ними
            stopwatch.Start();
            var t = new Thread(ThreadFunc);
            t.Start(source.Token);   // до потоку передаємо токен від джерела

            Console.WriteLine("Press a key to stop Thread");
            Console.ReadKey(true);
            // t.Abort();  -- стара схема не підтримується і призводить до винятку при виклику
            source.Cancel();   // подаємо команду скасування усіх токенів даного джерела
            // (само по собі скасування токенів ні до чого не призводить,
            //  лише до того факту, що токен став скасованим. Перевіряти це має потокова функція)
        }

        private void ThreadFunc(Object? arg)
        {
            try
            {
                CancellationToken token = (CancellationToken)arg!;
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc start");
                // з метою реалізації контрольованого скасування роботу необхідно 
                // поділити на транзакції - елементарні блоки, між якими може припускатись
                // скасування загальної роботи.
                for(int i = 0; i < 100; i += 1)
                {
                    Thread.Sleep(100);
                    // завершення транзакції - місце дозволеного скасування
                    // Перевірямо чи не перейшов токен до скасованого стану
                    token.ThrowIfCancellationRequested();   // перевірка + виняток якщо скасований
                }                
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc stop");
            }
            catch
            {
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc cancelled");
            }
        }

        private void ThreadFuncOld(Object? arg)
        {
            // Функція організована у старий спосіб, її зупинити (скасувати) можна 
            // лише зовнішнім винятком
            try
            {
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc start");
                Thread.Sleep(10000);
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc stop");
            }
            catch
            {
                Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000} ThreadFunc cancelled");
            }
        }
    }
}
/* Скасування потоків та задач
 * Є дві схеми скасування потоків:
 * - стара (новими .NET не підтримується) - використання системних команд 
 *    зупинки потоків - Abort()
 * - нова - з використанням спеціальних токенів скасування
 * 
 * == стара схема ==
 * Main {                       /--> func() {  відповідно до того, що зупинка відбувається через виняток
 *  t = new Thread(func);------/      try {    весь робочий код прийнято вміщувати до try-catch блоку
 *  ...
 *  ...                        /----> throw AbortException -- принцип зупинки - поява винятку в ф-ції потоку
 *  t.Abort() ----------------/       } catch() { завершальні дії }       
 * }                                 }  оскільки момент появи винятку непередбачуваний, задача блоку catch
 *                                       полягає у тому, щоб фіналізувати роботу та звільнити ресурси.
 *                                       
 * == нова схема ==
 * мета: покращити безпекові показники - той факт, що потік може зупинитись від іншого виконавця
 * а також непередбачуваність моменту завершення є недоліками схеми.
 * Main {                                  func(token) {
 * source -- токен можна скасувати тільки через джерело, відповідно тільки власник джерела може це зробити                                    
 * t = new Thread(func, source.Token);        
 * ....
 * source.Cancel() ----------------------->  виняток не створюється, тільки токен переходить до 
 *                                            скасованого стану. Перевіряти це має код func
 *                   Відповідно, є дві традиції реалізації: 
 * - на базі старої схеми - організовувати try-catch і при скасуванні токену створювати 
 *     виняток (! але контрольовано - у ті моменти, коли це можна дозволяти, між транзакціями)
 * - без винятків - з періодичною перевіркою токена на скасованість    
 * У будь-якому разі скасування роботи здійснюється не автоматично, а з дозволу коду ф-ції
 */