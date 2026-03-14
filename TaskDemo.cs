using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class TaskDemo
    {
        public void Run()
        {
            Console.WriteLine("Задачі");
            // Task - основний клас з управління задачами

            // Task.Run(() => Show("Task Argument")).Wait(); -- дуже погана практика
            //  запускати асинхронну задачу та тут же її очікувати - синхронне виконання
            //  Show("Task Argument") буде простішим та ефективнішим

            Task task = Task.Run(() => Show("Task Argument"));   // правильніше - запустити задачу ...
            Task<String> task3 = Task.Run(() => Concat("Hello", " World"));
            
            Task<String> task2 = Task.Run(Supply);   // Обгортка Task автоматично додається до повернення String
            Console.WriteLine(task2.Result);

            task.Wait();   // ... та перейти до очікування після того, як будуть виконані якісь інші дії

            Console.WriteLine(task3.Result);
            Console.WriteLine("--------------------------------------");
            Task.Run(RunAsync).Wait();   // перехід до асинхронного контексту
        }

        private void Show(String str)
        {
            Console.WriteLine("Show start");
            Task.Delay(1000).Wait();   // при роботі з задачами не бажано звертатись до Thread (Thread.Sleep)
            Console.WriteLine(str);
        }

        private String Supply()
        {
            Task.Delay(1000).Wait();
            return "Supply Resource";
        }

        private String Concat(String str1, String str2)
        {
            Task.Delay(1000).Wait();
            return str1 + str2;
        }

        // syntax sugar
        // для спрощення використання формалізму Task.Run вводиться 
        // спеціальний синтаксис "async", який явно зазначає асинхронну природу 
        // методу та дозволяє запускати його як звичайний метод, одержуючи 
        // в результаті саму задачу. Традиція - вживати суфікс "Async"
        // в іменах таких методів
        private async Task ShowAsync(String str)
        {
            Task.Delay(1000).Wait();   
            Console.WriteLine(str);
        }
        private async Task<String> SupplyAsync()
        {
            Task.Delay(1000).Wait();
            return "Supply Resource";   // хоча метод має повертати Task<String> у return тільки сам String
        }
        private async Task<String> ConcatAsync(String str1, String str2)
        {
            Task.Delay(1000).Wait();
            return str1 + str2;
        }

        private async void VoidMethodAsync()   // async метод може бути void
        {
            Console.WriteLine("VoidMethodAsync start");
            Task.Delay(1000).Wait();
            Console.WriteLine("VoidMethodAsync finish");
        }

        private async Task RunAsync()
        {
            // var task1 = ShowAsync("syntax sugar");          Однаковість інструкцій
            // var task2 = SupplyAsync();                      може сприйматись як однотипність
            // var task3 = ConcatAsync("Hello", " World");     змінних, що не є так

            Task task1 = ShowAsync("syntax sugar");               // Пряме зазначення типів
            Task<String> task2 = SupplyAsync();                   // є більш інформативним
            Task<String> task3 = ConcatAsync("Hello", " World");  // 
            // також можна звернути увагу, що немає потреби у лямбда-виразах

            Console.WriteLine("Before call");

            await task1;  // await - заміна .Wait() та одночасно .Result
            // але вживання await дозволяється тільки в async методах. 
            // для практичної роботи десь має відбуватись перехід з синхронного
            // до асинхронного контексту, оскільки не завжди Main дозволяється
            // декларувати асинхронним

            Console.WriteLine(await task2);
            Console.WriteLine(await task3);

            VoidMethodAsync();   // очікувати async void неможна, це здійснюється автоматично

            Console.WriteLine("After call");
        }
    }
}
/* Задачі.
 * Задачі - об'єкти реалізації асинхронності на рівні мови програмування
 * або платформи (.NET)
 * Задачі покликані виправити недоліки використання потоків
 * - відсутність повернення даних - взаємодія тільки через спільні ресурси
 * - передача тільки одного аргументу, причому без типізації (Object?)
 * - непоширення винятків за межі потоку
 * - пріоритети: звичайні потоки не дають завершуватись програмі, а 
 *    фонові потоки неможна очікувати.
 * - прив'язка до операційної системи - використання некерованих ресурсів.
 * 
 * На перший погляд, задачі приймають на виконання один з двох варіантів:
 * Action - тип даних для опису методів на кшталт
 *             void Method() - без параметрів та повернення
 * Func<Task?> - тип даних для опису методів на кшталт
 *             Task? Method() - без параметрів з поверненням Task?
 * Однак, лямбда-вирази дозволяють перетворити на ці типи довільні 
 * методи шляхом конвертації до виразу без параметрів
 *  () => AnyMethod(Any Arguments)
 * Якщо функція повертає значення, то для нього створюється тип 
 * Task<TResult> - задача, результатом очікуванням якої буде 
 * результат заданого типу. !! Як задачу її можна очікувати, а як
 * задачу з результатом - одержати відповідний результат.
 *  
 * Виконання задач є асинхронним, схожим з фоновими потоками у тому, що 
 * задачі припиняють виконання після зупинки головного потоку (Main),
 * але є можливість їх очікування.
 * 
 * Просте очікування задачі - .Wait() - доступне для усіх задач
 * Очікування результату - .Result - очікування + повернення результату.
 *             
 */
/* Д.З. Користувач вводить число (наприклад, 10)
 * програма запускає відповідну кількість задач (!! Task !!), кожна з яких
 * додає до спільного ресурсу-рядка (string) літеру з відповідним
 * порядком: (10 перших - це abcdefghij)
 * У результаті через конкуренцію порядок літер має виглядати
 * випадковим.
 * Також реалізувати виведення проміжних результатів кожної задачі.
 * Очікуваний вигляд консолі:
 * Enter num: 10
 * Processing letter 5 (e): ce
 * Processing letter 1 (a): cea
 * Processing letter 3 (c): c
 * ....
 * Result: ceaijhbafg
 */