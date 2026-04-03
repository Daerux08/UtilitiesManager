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
