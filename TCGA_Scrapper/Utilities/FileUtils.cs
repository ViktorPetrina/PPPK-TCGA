using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCGA_Scrapper.Utilities
{
    public static class FileUtils
    {
        public static void DecompressFile(string source, string destination)
        {
                using (FileStream compressedStream = new FileStream(source, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
                using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(outputStream);
                }
        }
    }
}
