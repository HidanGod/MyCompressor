using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            var comressor = new MZipCompressor();

            Stopwatch sw = new Stopwatch();
            sw.Start();
           // File.Delete("Compress.log");
            comressor.Compress(@"example1.log", @"Compress.log");
            // comressor.Decompress("Compress.log", "DeCompress.log");
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds).ToString());
            Console.Read();
        }
    }
}
