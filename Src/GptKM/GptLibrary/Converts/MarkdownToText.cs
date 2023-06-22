using NPOI.XWPF.UserModel;
using NPOI.XWPF.Extractor;

namespace GptLibrary.Converts
{
    public class MarkdownToText
    {
        public string ToText(string filename)
        {
            string result = string.Empty;

            result = File.ReadAllText(filename);

            return result;
        }
    }
}