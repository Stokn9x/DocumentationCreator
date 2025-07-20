using DocumentationCreator.Models;

namespace DocumentationCreator
{
    public class MarkdownBuilder
    {
        public string Build(CodeFile file, List<string> sections)
        {
            var md = new List<string>
            {
                $"# 📄 {file.FileName}",
                $"_Kategori_: **{file.Category}**",
                "",
                "---",
                ""
            };

            md.AddRange(sections);
            return string.Join("\n\n", md);
        }
    }
}
