using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompressor
{
    internal class Compressor
    {

        private Template[] Templates { get; set; }

        public async void Compress(string inputFile, string outputFile, Template[] templates)
        {
            Templates = templates.OrderBy(x => x.Count).ToArray();
            byte[] fileInBytes =  await RemoveTemplatePhrasesAsync(File.ReadAllBytes(inputFile));
            //byte[] fileInBytes = RemoveTemplatePhrases(File.ReadAllBytes(inputFile));
            var TempFileName = $"{Guid.NewGuid()}.log";

            using (var inputStream = new FileStream(TempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await inputStream.WriteAsync(fileInBytes, 0 , fileInBytes.Length);
                inputStream.Position = 0;
                using (var outputStream = File.OpenWrite(outputFile))
                using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal, true))
                    inputStream.CopyTo(gzipStream);
            }

            File.Delete(TempFileName);
        }

        public async Task<byte[]> RemoveTemplatePhrasesAsync(byte[] fileBytes)
        {
            return await Task.Run(() => RemoveTemplatePhrases(fileBytes));
        }

        public byte[] RemoveTemplatePhrases(byte[] fileBytes)
        {
            var array = new List<byte>();
            var startPossitionFloatingWindow = 0;
            while (startPossitionFloatingWindow < fileBytes.Length)
            {
                var template = GetTemplate(startPossitionFloatingWindow, fileBytes);
                var byteForWrite = GetByteForWrite(template, startPossitionFloatingWindow, fileBytes);
                array.AddRange(byteForWrite);
                startPossitionFloatingWindow += template.Count;
            }

            return array.ToArray();
        }

        private List<byte> GetByteForWrite(Template template, int startPossition, byte[] fileBytes)
        {
            if (template.IdTemplate == -1) return new List<byte> {fileBytes[startPossition]};
            var byteForWrite = Encoding.Default.GetBytes(template.IdTemplate.ToString()).ToList();
            byteForWrite.Insert(0, 255);
            byteForWrite.Add(255);

            return byteForWrite;
        }

        private Template GetTemplate(int stratPosition, byte[] fileBytes)
        {
            foreach (var template in Templates)
                if (Compare(stratPosition, template, fileBytes))
                    return template;

            return new Template(null, -1, 1);
        }

        private bool Compare(int stratPosition, Template template, byte[] fileBytes)
        {
            if (template.TemplateInBytes.Length + template.Count >= fileBytes.Length) return false;
            for (int i = 0; i < template.TemplateInBytes.Length; i++)
                if (fileBytes[i + stratPosition] != template.TemplateInBytes[i])
                    return false;

            return true;
        }
    }
}
