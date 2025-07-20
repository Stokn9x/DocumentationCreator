using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationCreator.Utils
{
    public static class Chunker
    {
        public static List<string> SplitCode(string code, int maxLength = 3000)
        {
            var lines = code.Split('\n');
            var chunks = new List<string>();
            var currentChunk = new List<string>();
            int currentLength = 0;

            foreach (var line in lines)
            {
                if (currentLength + line.Length >= maxLength)
                {
                    chunks.Add(string.Join('\n', currentChunk));
                    currentChunk.Clear();
                    currentLength = 0;
                }

                currentChunk.Add(line);
                currentLength += line.Length;
            }

            if (currentChunk.Count > 0)
                chunks.Add(string.Join('\n', currentChunk));

            return chunks;
        }
    }
}

