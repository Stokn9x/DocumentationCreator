using DocumentationCreator;
using DocumentationCreator.Models;
using DocumentationCreator.Services;
using DocumentationCreator.Utils;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load(@"C:\Users\CasperErmegaardJense\source\repos\DocumentationCreator\.env");

        var rootPath = args.Length > 0 ? args[0] : @"C:\Users\CasperErmegaardJense\source\repos\FLS-Archive\RunTest\";
        var outputPath = args.Length > 1 ? args[1] : @"C:\Test";

        var fileService = new FileService();
        var markdownBuilder = new MarkdownBuilder();
        var aiService = new AiService();

        var codeFiles = fileService.LoadCodeFiles(rootPath);

        var docService = new DocumentationService(fileService, aiService, markdownBuilder);
        await docService.GenerateDocumentationAsync(rootPath, outputPath);

        ArchitectureDiagramBuilder.Generate(outputPath, codeFiles);

        await ProjectOverviewBuilder.GenerateAsync(outputPath, aiService);

        IndexBuilder.GenerateIndexFile(outputPath);

        Console.WriteLine("✅ Dokumentation genereret.");
    }
}
