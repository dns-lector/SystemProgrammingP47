using System;
using System.Collections.Generic;
using System.Text;

namespace SystemProgrammingP47
{
    internal class Exam
    {
        const String SearchDirName = "files";

        public void Run()
        {
            Console.WriteLine("Екзамен: текстовий пошук у файлах");
            Console.WriteLine("Для пошуку доступно N файлів: ");
            String searchPath = Path.Combine(Directory.GetCurrentDirectory(), SearchDirName);
            String[] fileNames = Directory.GetFiles(searchPath);
            foreach (String fileName in fileNames)
            {
                Console.WriteLine(Path.GetFileName(fileName));
            }
            Console.WriteLine("--------------------------------");
            Console.Write("Введіть фрагмент для пошуку: ");
            String fragment = Console.ReadLine()!;
            // ! - null-checker, аналог if (fragment == null) throw new NullReferenceException();
            Thread[] works = [..
                fileNames
                .Select(filename => new Thread(() => SearchInFile(filename, fragment)))
            ];
            foreach (Thread work in works)
            {
                work.Start();
            }          
            Console.WriteLine("Run finish. Waiting for works to finish");
            foreach (Thread work in works)
            {
                work.Join();
            }
            Console.WriteLine("All works are finished");
        }

        private void SearchInFile(String filename, String fragment)
        {
            String shortName = Path.GetFileName(filename);
            Console.WriteLine($"{shortName} start");
            String fileContent = File.ReadAllText(filename);
            List<int> positions = [];
            int index = -1;
            while(true)
            {
                index = fileContent.IndexOf(fragment, index + 1);
                if(index > 0)
                {
                    positions.Add(index);
                }
                else {  break; }
            }
            if (positions.Count > 0)
            {
                String ending = positions.Count == 1 ? "ї" : "ях";
                Console.WriteLine($"{shortName}: знайдено у позиці{ending}: {String.Join(',', positions)}");
            }
            else
            {
                Console.WriteLine($"{shortName}: не знайдено");
            }
        }
    }
}
/* Реалізувати екзаменаційне завдання 
 * за допомогою багатозадачності (саме через задачі)
 */