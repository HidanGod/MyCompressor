using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompressor
{
    internal class MZipCompressor
    {
        public void Compress(string inputFile, string outputFile)
        {
            new Compressor().Compress(inputFile, outputFile, GetTemplates("Templates.txt"));
        }

        public void Decompress(string inputFile, string outputFile)
        {
            new Decompressor().Decompress(inputFile, outputFile, GetTemplates("Templates.txt"));
        }

        private static Template[] GetTemplates(string patch)
        {
            var allTemplatesString = File.ReadAllLines(patch);
            var countTemplates = allTemplatesString.Length;
            var templates = new Template[countTemplates];
            for (int i = 0; i < countTemplates; i++)
                templates[i] = new Template(Encoding.Default.GetBytes(allTemplatesString[i]), i, allTemplatesString[i].Length);
            return templates;
        }

    }
}
