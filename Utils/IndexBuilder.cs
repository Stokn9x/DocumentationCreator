namespace DocumentationCreator.Utils
{
    public class IndexBuilder
    {

        public async Task GenerateIndexFile(string docsRoot)
        {
            var indexLines = new List<string>
                {
                    "# 📚 Kode Wiki",
                    "",
                    "Herunder finder du automatisk genereret dokumentation struktureret efter mapper.",
                    ""
                };

            BuildIndexRecursive(docsRoot, indexLines, docsRoot, 0);

            var indexPath = Path.Combine(docsRoot, "index.md");
            File.WriteAllText(indexPath, string.Join("\n", indexLines));

            Console.WriteLine($"📄 index.md genereret: {indexPath}");
        }

        private static void BuildIndexRecursive(string currentDir, List<string> lines, string rootDir, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 2);
            var relativeDir = Path.GetRelativePath(rootDir, currentDir);
            var dirName = Path.GetFileName(currentDir);

            if (indentLevel == 0)
                lines.Add($"## 📂 {dirName}");
            else
                lines.Add($"{indent}- **{dirName}**");

            var mdFiles = Directory.GetFiles(currentDir, "*.md")
                                   .OrderBy(f => Path.GetFileName(f));

            foreach (var file in mdFiles)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var relativePath = Path.GetRelativePath(rootDir, file).Replace("\\", "/");
                lines.Add($"{indent}  - [{name}]({relativePath})");
            }

            foreach (var subDir in Directory.GetDirectories(currentDir))
            {
                if (Path.GetFileName(subDir).Equals(".git", StringComparison.OrdinalIgnoreCase))
                    continue;

                BuildIndexRecursive(subDir, lines, rootDir, indentLevel + 1);
            }

            if (mdFiles.Any() || Directory.GetDirectories(currentDir).Any())
                lines.Add("");
        }


    }
}
