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

            // Thread thread = new(ThreadAction1);    // метод, який буде виконуватись потоком
            // !! створення об'єкту не стартує виконання методу
            // thread.Start();   // запуск потоку в асинхронному режимі.
            // 

            try
            {
                new Thread(ThreadAction2).Start(10);    // для передачі даних до методу потоку
                new Thread(ThreadAction2).Start("A");   // аргумент подається до Start(...)
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Threading Demo End");
        }

        private void ThreadAction2(Object? arg)
        {
            if (arg is int)
            {
                Console.WriteLine($"Threading Action2 Begin with arg={arg}");
                Thread.Sleep(1000);
                Console.WriteLine($"Threading Action2 End with arg={arg}");
            }
            else
            {
                throw new ArgumentException("Only int arg");
            }
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
 * - він нічого не повертає (void)
 * - не має параметрів або має один параметр типу object? (у системі - адреса збереження даних)
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
 *                            
 *                            
 * 
 * Про винятки:
                                                        блоку catch вже просто не існує
 * -Main-=ThreadingDemo=try{}={catch}=[End]                |
 *                          \                              |
 *                       thread.Start()                    |
 *                            \__cw(Start)_______________throw
 *                            <---------<------------------|       
 *                            
 * Винятки інснують у межах одного потоку, блок catch з одного потоку не
 * ловить винятки другого потоку, навіть якщо він його стартує у своєму блоці try
 */


/* Виконання функцій чи методів
 * 
 * int fact(int n) { if n < 2 return 1 else return n * facn(n-1) }
 * 
 * fact(3) {
 *   return n * fact(2) {
 *        return n * fact(1) { return 1 }
 */

/* Оголосити клас Point, який має у собі координати (х,у) та назву ("А") точки
 * Створити потоковий метод, що може приймати точку та виводити її у стилі
 * Точка "А" (10;20)
 * Якщо передані дані не є точкою, то він виводить відповідне повідомлення (без винятків)
 * ! передані неправильні дані - очікується Point
 * Реалізувати запуск програми з передачею правильних та неправильних аргументів,
 * прикласти скріншот до звіту з ДЗ
 */