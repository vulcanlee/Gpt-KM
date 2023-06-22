namespace GptLibrary.Models
{
    public class ContentType
    {
        public static List<string> GetContentType(ContentTypeEnum contentTypeEnum)
        {
            switch (contentTypeEnum)
            {
                case ContentTypeEnum.PDF: return GetPdf();
                case ContentTypeEnum.HTML: return GetHtml();
                default: return new List<string>();
            }
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
        public class ContentTypeItem
        {
            public string Extension { get; set; } = string.Empty;
        }
    }
}