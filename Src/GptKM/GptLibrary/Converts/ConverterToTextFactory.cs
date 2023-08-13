using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GptLibrary.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GptLibrary.Converts
{
    /// <summary>
    /// 建立可用於轉換檔案成為文字內容的物件之工廠方法
    /// </summary>
    public class ConverterToTextFactory
    {
        private readonly ILogger<ConverterToTextFactory> logger;

        public ConverterToTextFactory(ILogger<ConverterToTextFactory> logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// 產生生成文字內容的物件
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public IFileToText Create(ContentTypeEnum contentType)
        {
            IFileToText result = null;

            if (contentType == ContentTypeEnum.EXCEL)
            {
                result = new ExcelToText();
            }
            else if (contentType == ContentTypeEnum.WORD)
            {
                result = new WordToText();
            }
            else if (contentType == ContentTypeEnum.POWERPOINT)
            {
                result = new PptToText();
            }
            else if (contentType == ContentTypeEnum.PDF)
            {
                result = new PdfPigToText(logger);
            }
            else if (contentType == ContentTypeEnum.HTML)
            {
                result = new HtmlToText();
            }
            else if (contentType == ContentTypeEnum.TEXT)
            {
                result = new TextToText();
            }
            else if (contentType == ContentTypeEnum.MARKDOWN)
            {
                result = new MarkdownToText();
            }

            return result;
        }
    }
}