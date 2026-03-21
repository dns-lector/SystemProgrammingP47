using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SystemProgrammingP47
{
    internal class ChainingDemo
    {
        private Stopwatch stopwatch = new();

        public void Run()
        {
            Console.WriteLine("Подовження задач. Нитки (ланцюги) коду. Task Chaining");
            stopwatch.Start();
            // Task.Run(RunAsync).Wait();
            // var chain1 = Task.Run(LoadDbConfig).ContinueWith(t1 => ConnectDb());
            // var chain2 = Task.Run(LoadApiConfig).ContinueWith(t1 => ConnectApi());
            // chain1.Wait();
            // chain2.Wait();
            // Console.WriteLine(Inverse("abc"));
            Task.Run(Generate)
                .ContinueWith(Inverse)
                .ContinueWith(UnSpacefy)
                .ContinueWith(Print)
                .Wait();
        }
        // UnSpacefy - видалити усі пробіли з рядка
        // LettersOnly - видалити усе окрім літер
        // SingleCase - перевести усі літери до одного реєстру
        // створити методи, додати перевантаження для аргумента Task<String> 
        // Запустити дві нитки коду
        // Generate - UnSpacefy - LettersOnly - SingleCase - Print
        // Generate - Inverse - UnSpacefy - LettersOnly - SingleCase - Print
        // порівняти їх результати, зробити висновок про те, чи є фраза паліндромом
        // до звіту з ДЗ додати скріншоти (для різних фраз, що генеруються)

        private String UnSpacefy(String str)
        {
            return str.Replace(" ", "");
        }
        private String UnSpacefy(Task<String> t)
        {
            return UnSpacefy(t.Result);
        }


        private String Generate()
        {
            return "Madam, I'm Adam";
        }

        private String Inverse(String str)
        {
            return new([..str.Reverse()]);
            // return str.Reverse().Aggregate("", (acc, c) => acc + c);
            // return String.Concat(str.Reverse());
            // return String.Join("", str.Reverse());
        }
        private String Inverse(Task<String> t)
        {
            return Inverse(t.Result);
        }
        private void Print(Task<String> t)
        {
            Console.WriteLine(t.Result);
        }


        private async Task RunAsync()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: RunAsync start");
            var chain1 = LoadDbConfigAsync().ContinueWith(t1 => Task.Run(ConnectDbAsync).Wait());
            var chain2 = LoadApiConfigAsync().ContinueWith(t1 => Task.Run(ConnectApiAsync).Wait());
            await chain1;
            await chain2;
        }

        private async Task RunAsyncNonOptimal()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: RunAsync start");
            var t1 = LoadDbConfigAsync();
            var t3 = LoadApiConfigAsync();
            await t1;
            var t2 = ConnectDbAsync();
            await t3;
            var t4 = ConnectApiAsync();
            await t2;
            await t4;
        }

        private void LoadDbConfig()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadDbConfig start");
            Task.Delay(1000).Wait();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadDbConfig finish");
        }

        private void LoadApiConfig()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadApiConfig start");
            Task.Delay(500).Wait();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadApiConfig finish");
        }

        private void ConnectDb()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectDb start");
            Task.Delay(500).Wait();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectDb finish");
        }

        private void ConnectApi()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectApi start");
            Task.Delay(500).Wait();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectApi finish");
        }




        private async Task LoadDbConfigAsync()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadDbConfigAsync start");
            await Task.Delay(1000);
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadDbConfigAsync finish");
        }

        private async Task LoadApiConfigAsync()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadApiConfigAsync start");
            await Task.Delay(500);
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: LoadApiConfigAsync finish");
        }

        private async Task ConnectDbAsync()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectDbAsync start");
            await Task.Delay(500);
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectDbAsync finish");
        }

        private async Task ConnectApiAsync()
        {
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectApiAsync start");
            await Task.Delay(500);
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}: ConnectApiAsync finish");
        }
    }
}
/* Подовження задач. Нитки (ланцюги) коду. Task Chaining
 * Проблема:
 *  деякі задачі потребують попереднього завершення інших задач,
 *  у той же час як з іншими вони можуть виконуватись паралельно.
 * Приклад:
 *  у файлах лежать паролі від ресурсів: окремо від БД, окремо від АРІ
 *  послідовність 1: відкрити файл, зчитати конфігуацію БД (Т1) -- підключитись до БД (Т2)
 *  послідовність 2: відкрити файл, зчитати конфігуацію АРІ (Т3) -- підключитись до АРІ (Т4)
 * Як впорядковувати очікування задач?
 *  Т1, Т3 - запуск
 *  await T1       | як не переставляй порядок очікування
 *  T2             | якась з задач Т2 або Т4 буде чекати "не свою" задачу - Т3 або Т1
 *  await T3       |
 *  T4  -- формально для запуску Т4 здійснюється очікування Т1
 *           await T1
 *  -----1-----|----2----
 *  ==3==      |====4====
 *  
 * Рішення - утворення "ниток" коду, тобто до задач, що виконуються 
 * асинхронно, на етапі оголошення задавати задачі-подовження, які 
 * автоматично будуть запущені після завершення попередньої задачі.
 * -----1-----|----2----
 * ==3==|====4====      
 * 
 * 
 * 
 * Інша галузь використання подовжень - каскадна обробка даних
 * Наприклад,
 *  --читаємо файл--==розархівовуємо==--обробляємо--==заархівовуємо==--зберігаємо--
 * Переваги такого підходу полягають у тому, що кожна ланка обробки
 * набуває універсального характеру і може використовуватись в інших ланках
 * Зазвичай, у таких задачах розрізняють
 * - джерела (Supply)
 * - перетворювачи (Function)
 * - споживачі (Consumer)
 * Supply - Function - Function - ... - Function - Consumer
 */