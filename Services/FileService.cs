using DocumentationCreator.Models;
using DocumentationCreator.Utils;

namespace DocumentationCreator.Services
{
    public class FileService
    {
        public List<CodeFile> LoadCodeFiles(string rootPath)
        {
            var excludedDirs = new[] { "bin", "obj", "Migrations" };

            var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories)
                                 .Where(f =>
                                 {
                                     var directoryPath = Path.GetDirectoryName(f) ?? string.Empty;
                                     return !excludedDirs.Any(excluded =>
                                         directoryPath.Split(Path.DirectorySeparatorChar)
                                                      .Any(part => part.Equals(excluded, StringComparison.OrdinalIgnoreCase)));
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
