using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompressor
{
    internal class Decompressor
    {
        private Template[] Templates { get; set; }

        public async void Decompress(string inputFile, string outputFile, Template[] templates)
        {
            Templates = templates.OrderBy(x => x.Count).ToArray();

            var TempFileName = $"{Guid.NewGuid()}.log";
            DecompressGZip(inputFile, TempFileName);
            byte[] fileInBytes = await AddTemplatePhrasesAsync(File.ReadAllBytes(TempFileName));
            File.Delete(TempFileName);
            using (var outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                await outputStream.WriteAsync(fileInBytes,  0, fileInBytes.Length);

        }

        private async Task<byte[]> AddTemplatePhrasesAsync(byte[] fileBytes)
        {
            return await Task.Run(() => AddTemplatePhrases(fileBytes));
        }

        public byte[] AddTemplatePhrases(byte[] fileBytes)
        {
            var array = new List<byte>();
            var startPossitionFloatingWindow = 0;
            while (startPossitionFloatingWindow < fileBytes.Length)
            {
                var template = SearchTemplate(startPossitionFloatingWindow, fileBytes);
                var byteForWrite = GetByteForWrite(template, startPossitionFloatingWindow, fileBytes);
                array.AddRange(byteForWrite);
                startPossitionFloatingWindow += GetOffset(template);
            }

            return array.ToArray();
        }

        private Template SearchTemplate(int startPossitionFloatingWindow, byte[] fileBytes)
        {
            if (fileBytes[startPossitionFloatingWindow] != 255) return new Template(null, -1, 1);
            int templateId = GetTemplateId(startPossitionFloatingWindow + 1, fileBytes);
            return Templates.FirstOrDefault(t => t.IdTemplate == templateId);
        }

        private int GetTemplateId(int startPossitionFloatingWindow, byte[] fileBytes)
        {
            var templateIdByte = new List<byte>();
            var i = 0;
            while (fileBytes[startPossitionFloatingWindow + i] != 255)
            {
                templateIdByte.Add(fileBytes[startPossitionFloatingWindow + i]);
                i++;
            }
            return int.Parse(Encoding.UTF8.GetString(templateIdByte.ToArray(), 0, templateIdByte.Count));
        }

        private byte[] GetByteForWrite(Template template, int startPossitionFloatingWindow, byte[] fileBytes)
        {
            if (template.IdTemplate == -1) return new byte[] { fileBytes[startPossitionFloatingWindow] };
            return template.TemplateInBytes;
        }

        private int GetOffset(Template template)
        {
            if (template.IdTemplate == -1) return 1;
            return 3;

        }

        private void DecompressGZip(string inputFile, string outputFile)
        {
            using (var inputStream = File.OpenRead(inputFile))
            using (var outputStream = File.OpenWrite(outputFile))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
                gzipStream.CopyTo(outputStream);
        }
    }
}
