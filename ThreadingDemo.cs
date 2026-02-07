using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class ThreadingDemo
    {
        public void Run()
        {
            Console.WriteLine("Threading Demo");
            Thread thread = new(ThreadAction1);    // метод, який буде виконуватись потоком
            // !! створення об'єкту не стартує виконання методу
            thread.Start();   // запуск потоку в асинхронному режимі.
            Thread.Sleep(1);
            Console.WriteLine("Threading Demo End");
        }

        private void ThreadAction2()
        {
            Console.WriteLine("Threading Action2 Begin");
            Thread.Sleep(1000);
            Console.WriteLine("Threading Action2 End");
        }

        private void ThreadAction1()
        {
            Console.WriteLine("Threading Action1 Begin");
            Thread.Sleep(1000);
            Console.WriteLine("Threading Action1 End");
        }
    }
}
/* У програмуванні існує два поняття потоків:
 * thread - потоки коду - предмет розгляду
 * stream - потоки даних
 * 
 * Потоки беруть на виконання код, адреса якого передається операційній системі (ОС)
 * У C# може використовуватись посилання (reference) на метод. До методу висуваються
 * обмеження:
 * - він нічого не повертає
 * 
 * 
 * 
 * 
 * 
 * 
 * [початок Main]                               [кінець Main]
 * |      [початок Demo]       [кінець Demo]     |
 * |       |                              |      |
 * --Main--==ThreadingDemo===Sleep(1)===cw=--cw--
 *                          \
 *                       thread.Start()  
 *                            \__cw(Start)____ThreadAction1_____cw(End)_
 */

/* Виконання функцій чи методів
 * 
 * int fact(int n) { if n < 2 return 1 else return n * facn(n-1) }
 * 
 * fact(3) {
 *   return n * fact(2) {
 *        return n * fact(1) { return 1 }
 */
