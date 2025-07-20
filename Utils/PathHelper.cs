using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationCreator.Utils
{
    public static class PathHelper
    {
        public static string GetCategory(string filePath)
        {
            var parts = filePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var knownFolders = new[] { "Services", "Controllers", "Models", "Utils", "Helpers" };

            foreach (var part in parts)
            {
                if (knownFolders.Contains(part, StringComparer.OrdinalIgnoreCase))
                    return part;
            }

            return "Misc";
        }
    }
}

