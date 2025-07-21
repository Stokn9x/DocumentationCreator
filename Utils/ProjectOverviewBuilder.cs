using DocumentationCreator.Services;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationCreator.Utils
{
    public static class ProjectOverviewBuilder
    {
        public static async Task GenerateAsync(string docsRoot, AiService aiService)
        {
            var mdFiles = Directory.GetFiles(docsRoot, "*.md", SearchOption.AllDirectories)
                                   .Where(f => !f.EndsWith("index.md", StringComparison.OrdinalIgnoreCase))
                                   .OrderBy(f => f)
                                   .ToList();

            var summaryLines = new List<string>();
            foreach (var file in mdFiles)
            {
                var relativePath = Path.GetRelativePath(docsRoot, file).Replace("\\", "/");
                var name = Path.GetFileNameWithoutExtension(file);
                summaryLines.Add($"- {name} ({relativePath})");
            }

            var fileList = string.Join("\n", summaryLines);
            var prompt = $$"""
                    Du er en softwarearkitekt.

                    Her er en liste over dokumenterede C#-filer i et projekt:

                    {{fileList}}

                    Ud fra filnavnene og strukturen, skriv:
                    - En beskrivelse af projektets formål
                    - En oversigt over hovedkomponenter og deres ansvar
                    - Eventuelle teknologier/metoder der tydeligt genkendes
                    - Et simpelt “kom i gang”-afsnit, hvis relevant

                    Returnér det som Markdown.
                    """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Du skriver professionel Markdown-baseret dokumentation. Ingen snak."),
                new UserChatMessage(prompt)
            };

            var response = await aiService.AnalyzeCodeChunkAsync(prompt, "Projektoversigt");

            var content = "# 🧠 Projektoversigt\n\n" + response + "\n\n---\n\n📂 [Se den fulde dokumentation](index.md)";

            var overviewPath = Path.Combine(docsRoot, "README.md");
            File.WriteAllText(overviewPath, content);

            Console.WriteLine($"📝 README.md genereret: {overviewPath}");

        }
    }

}
