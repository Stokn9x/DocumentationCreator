using DocumentationCreator.Models;
using DocumentationCreator.Utils;

namespace DocumentationCreator.Services
{
    public class FileService
    {
        public List<CodeFile> LoadCodeFiles(string rootPath)
        {
            var excludedFolders = new[] { "bin", "obj", ".vs", "Migrations" };

            var files = Directory
                .EnumerateFiles(rootPath, "*.cs", SearchOption.AllDirectories)
                .Where(f =>
                {
                    var directory = Path.GetDirectoryName(f);
                    return !excludedFolders.Any(excluded =>
                        directory?.Split(Path.DirectorySeparatorChar).Contains(excluded, StringComparer.OrdinalIgnoreCase) == true);
                })
                .ToList();

            return files.Select(f => new CodeFile
            {
                Path = f,
                Content = File.ReadAllText(f),
                Category = PathHelper.GetCategory(f)
            }).ToList();
        }

    }
}
