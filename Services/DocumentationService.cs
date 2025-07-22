using DocumentationCreator.Models;
using DocumentationCreator.Utils;

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

        public async Task GenerateDocumentationAsync(string rootPath, string outputFolder)
        {
            var codeFiles = _fileService.LoadCodeFiles(rootPath);
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
                    bool isRootCategory = string.IsNullOrWhiteSpace(group.Key) || !group.Key.Contains('/');

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

                    if (!isRootCategory)
                    {
                        var classNames = group.Select(f => Path.GetFileNameWithoutExtension(f.Path)).ToList();
                        var category = group.Key;
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

                        var overviewFolder = Path.Combine(outputFolder, category);
                        Directory.CreateDirectory(overviewFolder);

                        var categoryName = category.Split('/').Last();
                        var overviewPath = Path.Combine(overviewFolder, $"{categoryName}.md");

                        await File.WriteAllTextAsync(overviewPath, overviewMarkdown);
                    }
                }
            }
        }

        private bool ShouldGroup(string category)
        {
            var topLevel = category.Split('/').FirstOrDefault()?.ToLowerInvariant();
            return topLevel is "models" or "dto" or "DTO" or "enums";
        }
    }
}
