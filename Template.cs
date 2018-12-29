using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompressor
{
    public class Template
    {
        public byte[] TemplateInBytes { get; set; }
        public int IdTemplate { get; set; }
        public int Count { get; set; }

        public Template(byte[] templateInBytes, int idTemplate, int count)
        {
            TemplateInBytes = templateInBytes;
            IdTemplate = idTemplate;
            Count = count;
        }

    }
}
