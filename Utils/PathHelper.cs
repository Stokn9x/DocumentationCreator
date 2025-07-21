using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationCreator.Utils
{
    public static class PathHelper
    {
        public static string GetCategory(string filePath, string rootPath)
        {
            var relativePath = Path.GetRelativePath(rootPath, filePath);
            var dirPath = Path.GetDirectoryName(relativePath);

            return dirPath?.Replace(Path.DirectorySeparatorChar, '/') ?? "Misc";
        }
    }

}

