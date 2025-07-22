using DocumentationCreator.Services;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentationCreator.Utils
{
    public static class ProjectOverviewBuilder
    {
        public static async Task GenerateAsync(string docsRoot, AiService aiService)
        {
            // Find alle markdown-filer undtagen index og README
            var mdFiles = Directory.GetFiles(docsRoot, "*.md", SearchOption.AllDirectories)
                                   .Where(f =>
                                       !f.EndsWith("index.md", StringComparison.OrdinalIgnoreCase) &&
                                       !f.EndsWith("README.md", StringComparison.OrdinalIgnoreCase))
                                   .OrderBy(f => f)
                                   .ToList();

            // Find kun kategori-oversigter (f.eks. Services.md, Utils.md, osv.)
            var categoryOverviews = mdFiles.Where(f =>
            {
                var fileName = Path.GetFileName(f);
                return mdFiles.All(other =>
                    other == f || !other.StartsWith(f.Replace(".md", Path.DirectorySeparatorChar + ""), StringComparison.OrdinalIgnoreCase));
            }).ToList();

            var summaryLines = new List<string>();
            foreach (var file in categoryOverviews)
            {
                var relativePath = Path.GetRelativePath(docsRoot, file).Replace("\\", "/");
                var name = Path.GetFileNameWithoutExtension(file);
                summaryLines.Add($"- [{name}]({relativePath})");
            }

            var fileList = string.Join("\n", summaryLines);

            var prompt = $$"""
                Du er en softwarearkitekt.

                Her er en liste over dokumentationssektioner for et C#-projekt:

                {{fileList}}

                Skriv en **oversigtsside i Markdown** der:
                - Forklarer projektets formål
                - Giver et hurtigt overblik over dets hovedområder
                - Præsenterer de vigtigste komponenter og kategorier
                - Linker til hver kategori med en punktopstilling

                Returnér kun Markdown.
                """;

            var response = await aiService.AnalyzeCodeChunkAsync(prompt, "Projektoversigt");

            var content = "# 🧠 Projektoversigt\n\n" + response;

            var overviewPath = Path.Combine(docsRoot, "README.md");
            File.WriteAllText(overviewPath, content);

            Console.WriteLine($"📝 README.md genereret: {overviewPath}");
        }
    }
}
