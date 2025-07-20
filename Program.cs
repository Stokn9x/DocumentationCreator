using DocumentationCreator;
using DocumentationCreator.Services;
using DocumentationCreator.Utils;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load(@"C:\Users\Casper\source\repos\DocumentationCreator\.env");

        var rootPath = args.Length > 0 ? args[0] : @"C:\Users\Casper\source\repos\ERP-archival\RunTest\";
        var outputPath = args.Length > 1 ? args[1] : @"C:\Users\Casper\Desktop\Fix";

        var fileService = new FileService();
        var markdownBuilder = new MarkdownBuilder();
        var aiService = new AiService();

        var docService = new DocumentationService(fileService, aiService, markdownBuilder);
        await docService.GenerateDocumentationAsync(rootPath, outputPath);
        IndexBuilder.GenerateIndexFile(outputPath);

        Console.WriteLine("✅ Dokumentation genereret.");
    }
}
