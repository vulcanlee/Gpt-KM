using NPOI.XWPF.UserModel;
using NPOI.XWPF.Extractor;

namespace GptLibrary.Converts
{
    public class TextToText : IFileToText
    {
        public Task<string> ToTextAsync(string filename)
        {
            var task = Task.Run(() =>
            {
                string result = string.Empty;

                result = File.ReadAllText(filename);

                return result;
            });
            return task;
        }
    }
}