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

            foreach (var file in codeFiles)
            {
                Console.WriteLine($"📄 Dokumenterer: {file.Path}");

                var markdown = await _aiService.AnalyzeCodeChunkAsync(file.Content, file.Category);

                var finalMarkdown = _markdownBuilder.Build(file, new List<string> { markdown });

                var categoryFolder = Path.Combine(outputFolder, file.Category);
                Directory.CreateDirectory(categoryFolder);

                var outputPath = Path.Combine(categoryFolder, $"{Path.GetFileNameWithoutExtension(file.Path)}.md");
                await File.WriteAllTextAsync(outputPath, finalMarkdown);
            }
        }
    }
}
