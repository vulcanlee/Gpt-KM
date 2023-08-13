using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace GptLibrary.Converts
{
    public class PdfPigToText : IFileToText
    {
        private readonly ILogger<ConverterToTextFactory> logger;

        public PdfPigToText(ILogger<ConverterToTextFactory> logger)
        {
            this.logger = logger;
        }
        public Task<string> ToTextAsync(string filename)
        {
            var task = Task.Run(() =>
            {
                StringBuilder sb = new StringBuilder();
                using (PdfDocument document = PdfDocument.Open(filename))
                {
                    foreach (Page page in document.GetPages())
                    {
                        IReadOnlyList<Letter> letters = page.Letters;
                        string example = string.Join(string.Empty, letters.Select(x => x.Value));
                        sb.AppendLine(example);
                        IEnumerable<Word> words = page.GetWords();
                        IEnumerable<IPdfImage> images = page.GetImages();
                    }
                }
                var alltext = sb.ToString();
                return alltext;
            });
            return task;
        }
    }
}