using DocumentationCreator.Models;

namespace DocumentationCreator.Utils;

public static class ArchitectureDiagramBuilder
{
    public static void Generate(string docsRoot, List<CodeFile> codeFiles)
    {
        var excludedRoots = new[] { "models", "dto", "enums" };

        var relevant = codeFiles
            .Where(f => !excludedRoots.Contains(f.Category?.Split('/').FirstOrDefault()?.ToLowerInvariant() ?? ""))
            .ToList();

        var diagram = MermaidDiagramBuilder.GenerateClassDiagram(relevant);

        var markdown = $$"""
        # 🏗️ Systemarkitektur

        Dette diagram viser et overblik over systemets arkitektur, med fokus på controllere, services, repositories, middleware og værktøjer.

        {{diagram}}

        """;

        var outputPath = Path.Combine(docsRoot, "Architecture.md");
        File.WriteAllText(outputPath, markdown);

        Console.WriteLine($"🏗️ Architecture.md genereret: {outputPath}");
    }
}
