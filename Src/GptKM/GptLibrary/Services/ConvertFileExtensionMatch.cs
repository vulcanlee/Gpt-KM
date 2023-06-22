using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Services
{
    public class ConvertFileExtensionMatch
    {
        List<string> canConvertFileExtensions;
        public ConvertFileExtensionMatch()
        {
            canConvertFileExtensions = new List<string>()
            {
                ".pdf",
                ".html",
                ".xlsx",
                ".docx",
                ".txt",
                ".pptx",
                ".md"
            };
        }
        public bool IsMatch(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLower();
            if (canConvertFileExtensions.Contains(extension))
            {
                return true;
            }
            return false;
        }
    }
}
