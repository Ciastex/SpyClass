using System;
using System.IO;
using HtmlAgilityPack;

namespace SpyClass.Rendering.HtmlRendering
{
    public class DocFile
    {
        public string FileName { get; }
        public HtmlDocument Document { get; }

        public DocFile(string fileName)
        {
            FileName = fileName;
            Document = new HtmlDocument();
        }

        public void WriteToDisk(string outDir)
        {
            Document.Save(
                Path.Combine(
                    outDir,
                    "types", 
                    FileName
                )
            );
        }
    }
}