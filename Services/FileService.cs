using DocumentationCreator.Models;
using DocumentationCreator.Utils;

namespace DocumentationCreator.Services
{
    public class FileService
    {
        public List<CodeFile> LoadCodeFiles(string rootPath, List<string> exclusions)
        {
            var defaultExcludedDirs = new[] { "bin", "obj", "Migrations" };

            var allExcludedDirs = defaultExcludedDirs
                .Concat(exclusions ?? Enumerable.Empty<string>())
                .Select(d => d.ToLowerInvariant())
                .ToHashSet();

            var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories)
                                 .Where(f =>
                                 {
                                     var directoryPath = Path.GetDirectoryName(f) ?? string.Empty;
                                     var pathParts = directoryPath.Split(Path.DirectorySeparatorChar)
                                                                  .Select(p => p.ToLowerInvariant());
                                     return !pathParts.Any(part => allExcludedDirs.Contains(part));
                                 })
                                 .ToList();

            return files.Select(f => new CodeFile
            {
                Path = f,
                Content = File.ReadAllText(f),
                Category = PathHelper.GetCategory(f, rootPath)
            }).ToList();
        }

    }
}
