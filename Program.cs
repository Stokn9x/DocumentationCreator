using DocumentationCreator;
using DocumentationCreator.Builders;
using DocumentationCreator.Enums;
using DocumentationCreator.Menu;
using DocumentationCreator.Models;
using DocumentationCreator.Services;
using DocumentationCreator.Utils;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

class Program
{
    static async Task Main(string[] args)
    {

        Env.Load(@"C:\Users\CasperErmegaardJense\source\repos\DocumentationCreator\.env");

        var rootPath = args.Length > 0 ? args[0] : @"C:\Users\CasperErmegaardJense\source\repos\FLS-Archive\RunTest\";
        var outputPath = args.Length > 1 ? args[1] : @"C:\Test";

        bool state = true;
        string dbConfigName = string.Empty;
        List<string> exclusions = new List<string>();

        Beautifier.ShowLoadingAnimation();
        
        while (state)
        {
            Console.Clear();
            string userChoice = Beautifier.CoolMenu("Er du klar til at dokumentere", "Ja", "Niks");

            if (userChoice == "Ja")
            {
                Console.Clear();
                userChoice = Beautifier.CoolMenuLanguage("Hvad skal der laves markdown på");

                if (userChoice == SupportedLanguage.CSharp.ToString())
                {
                    Console.Clear();
                    userChoice = Beautifier.CoolMenu("Har du en database config", "Ja", "Nej");

                    if (userChoice == "Ja")
                    {
                        dbConfigName = AnsiConsole.Ask<string>("Hvad er navnet på din [green]DbContext[/] klasse?")
                            ?? throw new InvalidOperationException("DbContext-navn må ikke være tomt.");

                        bool exclusionState = true;

                        Console.Clear();
                        string userExclusion = Beautifier.CoolMenu("Vil du ekskludere mapper fra dokumentationen?", "Ja", "Nej");

                        if (userExclusion == "Ja")
                        {
                            exclusions = MenuHelper.GetExclusionFolders();

                            Console.Clear();
                            var serviceProvider = ServiceConfigurator.ConfigureServices();
                            var runner = serviceProvider.GetRequiredService<Run>();
                            await runner.Start(rootPath, outputPath, dbConfigName, exclusions);
                        }
                        else
                        {
                            Console.Clear();
                            var serviceProvider = ServiceConfigurator.ConfigureServices();
                            var runner = serviceProvider.GetRequiredService<Run>();
                            await runner.Start(rootPath, outputPath, dbConfigName, exclusions);
                        }

                    }
                    else if (userChoice == "Nej")
                    {
                        Console.Clear();
                        string userExclusion = Beautifier.CoolMenu("Vil du ekskludere mapper fra dokumentationen?", "Ja", "Nej");

                        if (userExclusion == "Ja")
                        {
                            exclusions = MenuHelper.GetExclusionFolders();

                            Console.Clear();
                            var serviceProvider = ServiceConfigurator.ConfigureServices();
                            var runner = serviceProvider.GetRequiredService<Run>();
                            await runner.Start(rootPath, outputPath, dbConfigName, exclusions);
                        }
                        else
                        {
                            Console.Clear();
                            var serviceProvider = ServiceConfigurator.ConfigureServices();
                            var runner = serviceProvider.GetRequiredService<Run>();
                            await runner.Start(rootPath, outputPath, dbConfigName, exclusions);
                        }
                    }
                }
                else if (userChoice == SupportedLanguage.Python.ToString())
                {
                    Beautifier.CoolWrite("red", "Python understøttes ikke endnu. Prøv C#.");
                    continue;
                }
            }
            else
            {
                Console.WriteLine("Afslutter programmet...");
                state = false;
                continue;
            }
        }

        Beautifier.CoolWrite("purple", "✅ Dokumentation genereret.");
    }
}

