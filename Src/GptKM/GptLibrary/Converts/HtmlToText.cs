using HtmlAgilityPack;

namespace GptLibrary.Converts
{
    public class HtmlToText : IFileToText
    {
        public Task<string> ToTextAsync(string filename)
        {
            var task = Task.Run(() =>
            {
                string htmlContent = File.ReadAllText(filename);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // 刪除所有 script 和 style 節點
                RemoveNodeByTag(htmlDoc, "script");
                RemoveNodeByTag(htmlDoc, "style");

                // 取得網頁的純文本內容
                string plainText = htmlDoc.DocumentNode.InnerText;

                // 刪除多餘的空格和換行符
                plainText = System.Text.RegularExpressions.Regex.Replace(plainText, @"\s+", " ").Trim();

                return plainText;
            });
            return task;
        }

        void RemoveNodeByTag(HtmlDocument htmlDoc, string tagName)
        {
            var nodes = htmlDoc.DocumentNode.SelectNodes($"//{tagName}");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }
        }
    }
}