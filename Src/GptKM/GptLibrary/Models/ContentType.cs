namespace GptLibrary.Models
{
    /// <summary>
    /// 定義每種可轉換成為文字內容的檔案類型之副檔案名稱有哪些
    /// </summary>
    public class ContentType
    {
        /// <summary>
        /// 取得指定檔案類型所允許的副檔案名稱
        /// </summary>
        /// <param name="contentTypeEnum"></param>
        /// <returns></returns>
        public static List<string> GetContentType(ContentTypeEnum contentTypeEnum)
        {
            switch (contentTypeEnum)
            {
                case ContentTypeEnum.PDF: return GetPdf();
                case ContentTypeEnum.HTML: return GetHtml();
                case ContentTypeEnum.WORD: return GetWord();
                case ContentTypeEnum.EXCEL: return GetExcel();
                case ContentTypeEnum.POWERPOINT: return GetPowerPoint();
                case ContentTypeEnum.TEXT: return GetText();
                case ContentTypeEnum.MARKDOWN: return GetMarkdown();
                default: return new List<string>();
            }
        }

        #region 取得所有檔案類型的副檔名
        static List<string> GetMarkdown()
        {
            return new List<string>()
            {
                ".md",
            };
        }
        static List<string> GetText()
        {
            return new List<string>()
            {
                ".txt",
            };
        }
        static List<string> GetPowerPoint()
        {
            return new List<string>()
            {
                ".pptx",
            };
        }
        static List<string> GetExcel()
        {
            return new List<string>()
            {
                ".xlsx",
            };
        }
        static List<string> GetWord()
        {
            return new List<string>()
            {
                ".docx",
            };
        }
        static List<string> GetPdf()
        {
            return new List<string>()
            {
                ".pdf",
            };
        }
        static List<string> GetHtml()
        {
            return new List<string>()
            {
                ".html",
            };
        }
        #endregion

        /// <summary>
        /// 取得指定副檔名所屬的檔案列舉值
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static ContentTypeEnum GetContentTypeEnum(string extension)
        {
            var allContentTypeEnum = Enum.GetValues(typeof(ContentTypeEnum)).Cast<ContentTypeEnum>();
            foreach (var value in allContentTypeEnum)
            {
                var extensions = GetContentType(value);
                if (extensions.Contains(extension))
                {
                    return value;
                }
            }
            return ContentTypeEnum.UNKNOWN;
        }
    }
}