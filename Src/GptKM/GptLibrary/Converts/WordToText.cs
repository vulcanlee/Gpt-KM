using NPOI.XWPF.UserModel;
using NPOI.XWPF.Extractor;

namespace GptLibrary.Converts
{
    public class WordToText : IFileToText
    {
        public Task<string> ToTextAsync(string filename)
        {
            var task = Task.Run(() =>
            {
                string result = string.Empty;

                // 讀取 Word 檔案
                XWPFDocument document = new XWPFDocument(File.OpenRead(filename));

                // 讀取所有內容
                XWPFWordExtractor extractor = new XWPFWordExtractor(document);
                result = extractor.Text;

                // 關閉 C# 檔案
                document.Close();

                return result;
            });
            return task;
        }
    }
}