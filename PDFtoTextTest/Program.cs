using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace PDFtoTextTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToinput = "C:\\Users\\Kir\\Documents\\Visual Studio 2013\\Projects\\SWStool\\PDFtoTextTest\\bin\\Debug\\Orlov.pdf";
            string pathTooutput = "C:\\Users\\Kir\\Documents\\Visual Studio 2013\\Projects\\SWStool\\PDFtoTextTest\\bin\\Debug\\Orlov.txt";
            PDDocument doc = null;
            try
            {
                doc = PDDocument.load(pathToinput);
                PDFTextStripper stripper = new PDFTextStripper();
                System.IO.StreamWriter file = new System.IO.StreamWriter(pathTooutput);
                file.WriteLine(stripper.getText(doc));
                return;
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
        }
    }
}
