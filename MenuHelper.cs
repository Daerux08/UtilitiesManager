using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UtilitiesManager
{
    public static class MenuHelper
    {
        public static int ShowArrowMenu(string title, List<string> options, int selectedIndex = 0)
        {
            // Check if console input is redirected
            if (Console.IsInputRedirected)
            {
                return ShowNumberedMenu(title, options, selectedIndex);
            }

            ConsoleKeyInfo key;
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine($"=== {title} ===");
                Console.WriteLine();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.WriteLine($"> {options[i]}");
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Use ↑↓ arrows to navigate, ENTER to select, Q to quit");

                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : options.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = selectedIndex < options.Count - 1 ? selectedIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        running = false;
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        return -1; // User cancelled
                }
            }

            return selectedIndex;
        }

        private static int ShowNumberedMenu(string title, List<string> options, int selectedIndex = 0)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();
            Console.WriteLine("Console input is redirected. Please enter a number:");

            for (int i = 0; i < options.Count; i++)
            {
                var marker = i == selectedIndex ? "> " : "  ";
                Console.WriteLine($"{marker}{i + 1}. {options[i]}");
            }

            Console.WriteLine();
            Console.Write("Enter choice (or Q to quit): ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "q" || input == "quit")
                return -1;

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= options.Count)
            {
                return choice - 1;
            }

            return selectedIndex;
        }

        public static string GetUserInput(string prompt, string defaultValue = "")
        {
            Console.Clear();
            Console.WriteLine(prompt);
            if (!string.IsNullOrEmpty(defaultValue))
            {
                Console.WriteLine($"Default: {defaultValue}");
                Console.Write("Enter value (or press ENTER for default): ");
            }
            else
            {
                Console.Write("Enter value: ");
            }

            string input = Console.ReadLine()?.Trim() ?? "";
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        public static bool GetConfirmation(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.Write("Continue? (y/N): ");
            var response = Console.ReadLine()?.ToLower().Trim();
            return response == "y" || response == "yes";
        }

        public static void ShowMessage(string title, string message, bool waitForKey = true)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();
            Console.WriteLine(message);
            
            if (waitForKey)
            {
                Console.WriteLine();
                if (!Console.IsInputRedirected)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
                else
                {
                    Console.WriteLine("Press ENTER to continue...");
                    Console.ReadLine();
                }
            }
        }

        public static void ShowError(string title, string error)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();
            Console.WriteLine($"ERROR: {error}");
            Console.ResetColor();
            Console.WriteLine();
            
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
            }
        }

        public static int ShowQuickSelectMenu(string title, Dictionary<string, int> options, int selectedIndex = 0)
        {
            var optionList = new List<string>(options.Keys);
            return ShowArrowMenu(title, optionList, selectedIndex);
        }

        public static string ShowTextInputMenu(string title, string currentValue)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();
            Console.WriteLine($"Current value: {currentValue}");
            Console.WriteLine();
            Console.Write("Enter new value: ");
            
            var input = Console.ReadLine()?.Trim() ?? "";
            return input;
        }

        public static void DisplayTable(string title, List<string> headers, List<List<string>> rows)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();

            // Calculate column widths
            var columnWidths = new int[headers.Count];
            for (int i = 0; i < headers.Count; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var row in rows)
                {
                    if (i < row.Count && row[i].Length > columnWidths[i])
                    {
                        columnWidths[i] = row[i].Length;
                    }
                }
                columnWidths[i] += 2; // Add padding
            }

            // Display headers
            for (int i = 0; i < headers.Count; i++)
            {
                Console.Write(string.Format("{0,-" + columnWidths[i] + "}", headers[i]));
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', columnWidths.Sum()));

            // Display rows
            foreach (var row in rows)
            {
                for (int i = 0; i < Math.Min(headers.Count, row.Count); i++)
                {
                    Console.Write(string.Format("{0,-" + columnWidths[i] + "}", row[i]));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void DisplayKeyValueList(string title, Dictionary<string, string> items)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();

            int maxKeyLength = items.Keys.Max(k => k.Length) + 2;

            foreach (var item in items)
            {
                Console.WriteLine(string.Format("{0,-" + maxKeyLength + "}: {1}", item.Key, item.Value));
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void DisplayScrollableList(string title, List<string> items, int pageSize = 20)
        {
            int currentIndex = 0;
            ConsoleKeyInfo key;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== {title} ===");
                Console.WriteLine($"Showing {Math.Min(pageSize, items.Count - currentIndex)} of {items.Count} items");
                Console.WriteLine("Use ↑↓ to scroll, ENTER to select, Q to quit");
                Console.WriteLine();

                for (int i = 0; i < Math.Min(pageSize, items.Count - currentIndex); i++)
                {
                    var item = items[currentIndex + i];
                    Console.WriteLine(string.Format("{0:D3}. {1}", currentIndex + i + 1, item));
                }

                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentIndex > 0)
                            currentIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentIndex < items.Count - pageSize)
                            currentIndex++;
                        break;
                    case ConsoleKey.Enter:
                        if (currentIndex < items.Count)
                            return;
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
    }
}
