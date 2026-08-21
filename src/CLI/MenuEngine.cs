using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public class GoBackException : Exception
    {
        public GoBackException() : base("Go back to the previous menu") { }
    }

    public static class MenuEngine
    {
        public static bool YesNoPrompt(string prompt, string YesMSG, string NoMSG)
        {
            bool answer = AnsiConsole.Confirm(prompt);
            if (answer)
                AnsiConsole.MarkupLine("[green]" + YesMSG + "[/]");
            else
                AnsiConsole.MarkupLine("[red]" + NoMSG + "[/]");
            return answer;
        }

        public static void ShowError(string title, string message)
        {
            var panel = new Panel(message)
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader($"[bold red]{title}[/]")
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        public static void ShowMessage(string title, string message, bool clearFirst = true)
        {
            if (clearFirst) AnsiConsole.Clear();
            var panel = new Panel(message)
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green),
                Header = new PanelHeader($"[bold green]{title}[/]")
            };
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        public static string GetUserInput(string prompt, string defaultValue = "")
        {
            return AnsiConsole.Ask<string>(prompt + ":") ?? defaultValue;
        }

        public static int ShowQuickSelectMenu(string title, Dictionary<string, int> options)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(options.Keys));

            return options.Keys.ToList().IndexOf(choice);
        }

        public static int ShowArrowMenu(string title, List<string> options, int selectedIndex = 0)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(options));

            return options.IndexOf(choice);
        }

        public static async Task DisplayMenu(List<(string, Func<Task>)> menu)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select an option:")
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(menu.Select(x => x.Item1).ToArray()));

            try
            {
                await menu.First(x => x.Item1 == choice).Item2();
            }
            catch (GoBackException)
            {
                return;
            }
        }

        public static async Task DisplayMenuAsync(List<(string, Func<Task>)> menu)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select an option:")
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(menu.Select(x => x.Item1).ToArray()));

            try
            {
                await menu.First(x => x.Item1 == choice).Item2();
            }
            catch (GoBackException)
            {
                return;
            }
        }

        public static void DisplayMenu(List<(string, Action)> menu)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select an option:")
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(menu.Select(x => x.Item1).ToArray()));

            try
            {
                menu.First(x => x.Item1 == choice).Item2();
            }
            catch (GoBackException)
            {
                return;
            }
        }

        public static string TextInput(string prompt)
        {
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                .PromptStyle(new Style(foreground: Color.Green1, decoration: Decoration.Bold)));
            return input;
        }

        public static void ErrorMessage(string message)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]");
        }

        public static void GeneralMessage(string message)
        {
            AnsiConsole.MarkupLine($"[bold yellow]{message}[/]");
        }

        public static void EscapeKey()
        {
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    throw new GoBackException();
                }
            }
        }
    }
}
