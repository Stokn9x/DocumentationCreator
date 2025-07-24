using DocumentationCreator.Builders;
using DocumentationCreator.Menu;
using DocumentationCreator.Models;

namespace DocumentationCreator.Services
{
    public class DocumentationService
    {
        private readonly FileService _fileService;
        private readonly AiService _aiService;
        private readonly MarkdownBuilder _markdownBuilder;

        public DocumentationService(FileService fileService, AiService aiService, MarkdownBuilder markdownBuilder)
        {
            _fileService = fileService;
            _aiService = aiService;
            _markdownBuilder = markdownBuilder;
        }

        public async Task GenerateDocumentationAsync(string rootPath, string outputFolder, List<CodeFile> codeFiles)
        {
            Beautifier.CoolMenuHeader();
            Console.WriteLine();
            Console.WriteLine();
            Beautifier.CoolCenterLine("Green");
            Console.WriteLine();
            Console.WriteLine();


            var grouped = codeFiles.GroupBy(f => f.Category);

            foreach (var group in grouped)
            {
                if (ShouldGroup(group.Key))
                {
                    Console.WriteLine($"📦 Samler dokumentation for: {group.Key}");

                    var allCode = string.Join("\n\n", group.Select(f => f.Content));
                    var markdown = await _aiService.AnalyzeCodeChunkAsync(allCode, group.Key);

                    var dummyFile = group.First();
                    var finalMarkdown = _markdownBuilder.Build(dummyFile, new List<string> { markdown });

                    var categoryPath = Path.Combine(outputFolder, group.Key.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(Path.GetDirectoryName(categoryPath)!);

                    var outputPath = categoryPath + ".md";
                    await File.WriteAllTextAsync(outputPath, finalMarkdown);
                }
                else
                {
                    bool isRootCategory = string.IsNullOrWhiteSpace(group.Key);

                    foreach (var file in group)
                    {
                        Console.WriteLine($"📄 Dokumenterer: {file.Path}");

                        var markdown = await _aiService.AnalyzeCodeChunkAsync(file.Content, file.Category);
                        var finalMarkdown = _markdownBuilder.Build(file, new List<string> { markdown });

                        var categoryFolder = Path.Combine(outputFolder, file.Category);
                        Directory.CreateDirectory(categoryFolder);

                        var outputPath = Path.Combine(categoryFolder, $"{Path.GetFileNameWithoutExtension(file.Path)}.md");
                        await File.WriteAllTextAsync(outputPath, finalMarkdown);
                    }

                    if (!isRootCategory && !ShouldGroup(group.Key))
                    {
                        var classNames = group.Select(f => Path.GetFileNameWithoutExtension(f.Path)).ToList();
                        var category = group.Key;

                        // ➕ Mermaid klasse-diagram
                        var diagram = MermaidDiagramBuilder.GenerateClassDiagram(group);

                        var overviewPrompt = $$"""
                            Du er en teknisk dokumentationsskribent. Her er en oversigt over klasser i kategorien "{{category}}".

                            Skriv en introduktion i Markdown der:
                            - Forklarer overordnet hvad denne kategori handler om.
                            - Opsummerer kort hvilke typer klasser den indeholder.
                            - Indeholder en liste med links til hver enkelt klasse i denne kategori (brug `[Navn](Navn.md)` format).

                            Klasser:
                            {{string.Join("\n", classNames)}}
                            """;

                        var overviewMarkdown = await _aiService.AnalyzeCodeChunkAsync(overviewPrompt, category);
                        var fullOverview = diagram + "\n\n" + overviewMarkdown;

                        var overviewFolder = Path.Combine(outputFolder, category);
                        Directory.CreateDirectory(overviewFolder);

                        var categoryName = category.Split('/').Last();
                        var overviewPath = Path.Combine(overviewFolder, $"{categoryName}.md");

                        await File.WriteAllTextAsync(overviewPath, fullOverview);
                    }
                }
            }
        }

        private bool ShouldGroup(string category)
        {
            var topLevel = category.Split('/').FirstOrDefault()?.ToLowerInvariant();
            return topLevel is "models" or "dto" or "enums";
        }
    }
}
