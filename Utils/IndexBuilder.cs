namespace DocumentationCreator.Utils
{
    public static class IndexBuilder
    {
        public static void GenerateIndexFile(string docsRoot)
        {
            var indexLines = new List<string>
            {
                "# 📚 Kode Wiki",
                "",
                "Herunder finder du automatisk genereret dokumentation fordelt på kategorier.",
                ""
            };

            foreach (var categoryDir in Directory.GetDirectories(docsRoot))
            {
                var categoryName = Path.GetFileName(categoryDir);
                indexLines.Add($"## 📂 {categoryName}");

                var files = Directory.GetFiles(categoryDir, "*.md");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var relativePath = $"{categoryName}/{Path.GetFileName(file)}";
                    indexLines.Add($"- [{fileName}]({relativePath})");
                }

                indexLines.Add(""); // tom linje mellem kategorier
            }

            var indexPath = Path.Combine(docsRoot, "index.md");
            File.WriteAllText(indexPath, string.Join("\n", indexLines));

            Console.WriteLine($"📄 index.md genereret: {indexPath}");
        }
    }
}
