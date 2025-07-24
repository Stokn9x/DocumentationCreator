using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentationCreator.Enums;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace DocumentationCreator.Menu
{
    public static class Beautifier
    {
        public static void CoolWrite(string color, string text)
        {
            AnsiConsole.MarkupLine($"[{color}] {text} [/]");
        }

        public static void CoolPanel(string panelHeader, string panelText)
        {
            Console.WriteLine("\n \n");
            var panel = new Panel(panelText);
            panel.Header(panelHeader);
            panel.BorderColor(Color.Red3);
            AnsiConsole.Write(panel);
        }

        public static void CoolMenuHeader()
        {
            AnsiConsole.Write(
                new FigletText("Documentation")
                    .Centered()
                    .Color(Color.Cyan1));
            AnsiConsole.Write(
                new FigletText("Creator")
                    .Centered()
                    .Color(Color.Magenta1));
        }

        public static void CoolCenterLine(string color)
        {
            var rule = new Rule();
            rule.Style = Style.Parse(color);
            rule.Centered();
            AnsiConsole.Write(rule);
        }

        public static string CoolMenu(string title, string choice1, string choice2)//Method overload for 2 choices
        {
            AnsiConsole.Write(
                new FigletText("Documentation")
                    .Centered()
                    .Color(Color.Cyan1));
            AnsiConsole.Write(
                new FigletText("Creator")
                    .Centered()
                    .Color(Color.Magenta1));

            string playerChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"{title}\n ---------------------------")
                .PageSize(3)
                .AddChoices(choice1, choice2));
            return playerChoice;
        }

        public static string CoolMenu(string title, string choice1, string choice2, string choice3)//Method overload for 3 choices
        {
            string playerChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"{title}\n ---------------------------")
                .PageSize(4)
                .AddChoices(choice1, choice2, choice3));
            return playerChoice;
        }

        public static string CoolMenu(string title, string choice1, string choice2, string choice3, string choice4)//Method overload for 4 choices
        {
            string playerChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"{title}\n ---------------------------")
                .PageSize(5)
                .AddChoices(choice1, choice2, choice3, choice4));
            return playerChoice;
        }

        public static string CoolMenuLanguage(string title)
        {
            AnsiConsole.Write(
                new FigletText("Documentation")
                    .Centered()
                    .Color(Color.Cyan1));
            AnsiConsole.Write(
                new FigletText("Creator")
                    .Centered()
                    .Color(Color.Magenta1));

            var languages = Enum.GetNames(typeof(SupportedLanguage)).ToList();

            string playerChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"{title}\n---------------------------")
                .PageSize(languages.Count + 1)
                .AddChoices(languages));

            return playerChoice;
        }

        public static void ShowLoadingAnimation()
        {
            var tasksWithSpeed = new List<(string Message, int MinSpeed, int MaxSpeed, string Color, string Emoji)>
            {
                ("Spinning up the AI", 100, 200, "aqua", "🤖"),
                ("Analyzing your codebase", 80, 120, "yellow1", "🧠"),
                ("Linking documentation nodes", 50, 80, "springgreen3", "🔗"),
                ("Compiling Markdown magic", 70, 100, "magenta", "✨"),
                ("Mapping architecture", 60, 90, "deepskyblue1", "🏗️"),
                ("Generating ER diagram", 90, 130, "orange1", "🗃️"),
                ("Organizing categories", 60, 90, "orchid", "🗂️"),
                ("Finalizing everything...", 50, 70, "lime", "📝")
            };

            AnsiConsole.Write(
                new FigletText("Documentation")
                    .Centered()
                    .Color(Color.Cyan1));
            AnsiConsole.Write(
                new FigletText("Creator")
                    .Centered()
                    .Color(Color.Magenta1));

            AnsiConsole.MarkupLine("[bold yellow]Sit tight while we work our magic![/]");

            AnsiConsole.Progress()
                .AutoClear(true)
                .HideCompleted(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(Spinner.Known.Star)
                })
                .Start(ctx =>
                {
                    var progressTasks = new List<ProgressTask>();

                    foreach (var (message, _, _, color, emoji) in tasksWithSpeed)
                    {
                        progressTasks.Add(ctx.AddTask($"[{color}]{emoji} {message}[/]", new ProgressTaskSettings { MaxValue = 100 }));
                    }

                    var random = new Random();
                    var allCompleted = false;

                    while (!allCompleted)
                    {
                        allCompleted = true;

                        for (int i = 0; i < progressTasks.Count; i++)
                        {
                            var (_, minSpeed, maxSpeed, _, _) = tasksWithSpeed[i];
                            var task = progressTasks[i];

                            if (!task.IsFinished)
                            {
                                task.Increment(random.Next(2, 5));
                                Thread.Sleep(random.Next(minSpeed, maxSpeed));
                                allCompleted = false;
                            }
                        }
                    }
                });

            AnsiConsole.MarkupLine("[bold green]All done! Your documentation is ready.[/]");
        }


    }
}
