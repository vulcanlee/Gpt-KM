using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace GptLibrary.Converts
{
    public class ConverterToTextFactory 
    {
        public IFileToText Create(string fileType)
        {
            IFileToText result = null;

            if (fileType.EndsWith(".xlsx"))
            {
                result = new ExcelToText();
            }
            else if (fileType.EndsWith(".docx"))
            {
                result = new WordToText();
            }
            else if (fileType.EndsWith(".pptx"))
            {
                result = new PptToText();
            }
            else if (fileType.EndsWith(".pdf"))
            {
                result = new PdfToText();
            }
            else if (fileType.EndsWith(".html"))
            {
                result = new HtmlToText();
            }
            else if (fileType.EndsWith(".txt"))
            {
                result = new TextToText();
            }
            else if (fileType.EndsWith(".md"))
            {
                result = new MarkdownToText();
            }

            return result;
        }
    }
}