using System.Text;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;

namespace GptLibrary.Converts
{
    public class PdfToText : IFileToText
    {
        private readonly ILogger<ConverterToTextFactory> logger;

        public PdfToText(ILogger<ConverterToTextFactory> logger)
        {
            this.logger = logger;
        }
        public Task<string> ToTextAsync(string filename)
        {
            var task = Task.Run(() =>
            {
                StringBuilder result = new StringBuilder();

                using (PdfReader pdfReader = new PdfReader(filename))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                    {
                        try
                        {
                            int numberOfPages = pdfDoc.GetNumberOfPages();

                            for (int i = 1; i <= numberOfPages; i++)
                            {
                                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                                string pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                                result.AppendLine(pageContent);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"抽取檔案 {filename} 成為文字發生錯誤");
                        }
                    }
                }

                return result.ToString();
            });
            return task;
        }
    }
}