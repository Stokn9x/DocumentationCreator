using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentationCreator.Models;

namespace DocumentationCreator.Builders
{
    public class ErDiagramBuilder
    {
        public async Task Generate(string outputPath, List<CodeFile> codeFiles, string ormClassName)
        {
            var dbFile = codeFiles.FirstOrDefault(f => f.Content.Contains($"class {ormClassName}"));
            if (dbFile == null)
            {
                Console.WriteLine($"❌ Kunne ikke finde klassen {ormClassName}");
                return;
            }

            // Indlæs entity-filer
            var entities = codeFiles
                .Where(f => f != dbFile && f.Content.Contains("class "))
                .ToDictionary(f => GetClassName(f.Content), f => f.Content);

            var mermaidDiagram = MermaidDiagramBuilder.GenerateErDiagramFromDbContext(dbFile.Content, entities);

            var markdown = $$"""
                # 🗃️ ER-Diagram

                Dette diagram viser relationerne mellem entiteter i databasen.

                ```mermaid
                erDiagram
                {{mermaidDiagram}}
                ```
                """;

            var path = Path.Combine(outputPath, "ErDiagram.md");
            File.WriteAllText(path, markdown);
            Console.WriteLine($"📊 ER-diagram genereret: {path}");
        }

        private static string GetClassName(string content)
        {
            var match = Regex.Match(content, @"class\s+(\w+)");
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
