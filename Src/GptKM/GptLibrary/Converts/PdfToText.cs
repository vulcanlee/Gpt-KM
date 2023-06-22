using System.Text;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;

namespace GptLibrary.Converts
{
    public class PdfToText : IFileToText
    {
        public string ToText(string filename)
        {
            StringBuilder result = new StringBuilder();

            using (PdfReader pdfReader = new PdfReader(filename))
            {
                using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                {
                    int numberOfPages = pdfDoc.GetNumberOfPages();

                    for (int i = 1; i <= numberOfPages; i++)
                    {
                        try
                        {
                            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                            string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                            result.AppendLine(pageContent);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}