using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SystemProgrammingP47
{
    internal class ProcessDemo
    {
        private Process? notepadProcess;

        public void Run()
        {
            Console.WriteLine("Process Demo");
            Console.WriteLine(
                File.ReadAllText(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "demo.txt"
            )));
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.WriteLine("\n1 - Show processes");
                Console.WriteLine("2 - Start notepad");
                Console.WriteLine("3 - Close notepad");
                Console.WriteLine("4 - Edit demo.txt");
                Console.WriteLine("0 - Exit");
                keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.KeyChar)
                {
                    case '1': ShowProcesses(); break;
                    case '2': StartNotepad(); break;
                    case '3': CloseNotepad(); break;
                    case '0': break;
                    default: Console.WriteLine("Wrong choice"); break;
                }
            }
            while(keyInfo.KeyChar != '0');

        }
        private void CloseNotepad()
        {
            if (notepadProcess == null || notepadProcess.HasExited)
            {
                Console.WriteLine("Does not active");
            }
            else
            {
                try { notepadProcess.Kill(); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        private void StartNotepad()
        {
            if (notepadProcess == null || notepadProcess.HasExited)
            {
                try {
                    // notepadProcess = Process.Start("notepad.exe"); 
                    notepadProcess = Process.Start(
                        new ProcessStartInfo(
                            "notepad.exe",
                            Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "demo.txt"
                            )
                        )
                    );
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            else
            {
                Console.WriteLine("Already started");
            }
        }

        private void ShowProcesses()
        {
            Process[] processes = Process.GetProcesses();
            Dictionary<String, int> taskManager = [];

            foreach (Process process in processes)
            {
                String name;
                try
                {
                    name = process.ProcessName;
                }
                catch
                {
                    name = "Restrited";
                }

                if (taskManager.ContainsKey(name))
                {
                    taskManager[name] += 1;
                }
                else
                {
                    taskManager[name] = 1;
                }
            }
            foreach (var pair in taskManager.OrderByDescending(p => p.Value).ThenBy(p => p.Key))
            {
                Console.WriteLine($"{pair.Key} ({pair.Value})");
            }
        }
    }
}
/* Процеси - системні об'єкти, що відповідають за виконання програми.
 * Їх відображає диспетчер завдань.
 * За роботу з процесами відповідає клас Process з System.Diagnostics
 * Process.GetProcesses() - перелік усіх зареєстрованих у системі процесів
 */
/* Д.З. Додати до меню Процесів пункти:
 * - відкрити сайт itstep.org (у браузері)
 * ****
 * - відкрити у блокноті перелік процесів (результат ShowProcesses())
 * [реалізувати запис даних у файл та передати його до блокноту
 *  !! генерувати нові файли на кожен запит, не стирати попередні]
 */
