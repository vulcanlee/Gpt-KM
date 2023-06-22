using GptLibrary.Converts;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers
{
    public class ExtractFileToTextHelper
    {
        public async Task<string> ConvertToTextAsync(ContentTypeEnum contentTypeEnum,
            string SourceFile, string ConvertFile)
        {
            string sourceText = string.Empty;
            PdfToText pdfToText = new PdfToText();
            HtmlToText htmlToText = new HtmlToText();
            Tokenizer tokenizer = new Tokenizer();
            int count = 0;
            //Console.Write($"{count} ");
            //if (count > 10)
            //    break;
            count++;
            if (contentTypeEnum == ContentTypeEnum.PDF)
            {
                #region PDF 2 Text
                if (!(SourceFile.ToLower().EndsWith(".pdf") == true))
                {
                    return "";
                }

                sourceText = pdfToText.ToText(SourceFile);
                await System.IO.File.WriteAllTextAsync(ConvertFile, sourceText);

                #endregion
            }
            else if (contentTypeEnum == ContentTypeEnum.HTML)
            {
                #region HTML 2 Text
                if(!(SourceFile.ToLower().EndsWith(".html") == true)|| (SourceFile.ToLower().EndsWith(".htm") == true))
                {
                    return "";
                }
                sourceText = htmlToText.ToText(SourceFile);
                await System.IO.File.WriteAllTextAsync(ConvertFile, sourceText);
                #endregion
            }
            return sourceText;
        }

        List<ExpertRawFile> GetExamFiles(ContentTypeEnum contentTypeEnum,
            ExpertConfiguration expertConfiguration, ExpertContent expertContent)
        {
            List<ExpertRawFile> convertFiles = new List<ExpertRawFile>();
            var contentTypes = ContentType.GetContentType(contentTypeEnum);
            var expertFiles = expertContent.ExpertFiles.Where(x => contentTypes.Contains(x.Extension));

            foreach (var extension in expertFiles)
            {
                convertFiles.Add(extension);
            }
            var foo = convertFiles.Count();
            return convertFiles;
        }

        void CountFileExtensions(ExpertConfiguration expertConfiguration, ExpertContent expertContent)
        {
            string directoryPath = expertConfiguration.SourceDirectory;
            void ProcessDirectory(DirectoryInfo directoryInfo)
            {
                // Process all files in the current directory
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    ExpertRawFile expertFile = new ExpertRawFile()
                    {
                        Extension = fileInfo.Extension.ToLower(),
                        FileInfo = new ExpertFileInfo().FromFileInfo(fileInfo),
                        FullName = fileInfo.FullName,
                        FileName = fileInfo.Name,
                        Size = fileInfo.Length,
                        DirectoryName = $@"{fileInfo.DirectoryName}\",
                    };
                    expertContent.ExpertFiles.Add(expertFile);
                    string extension = fileInfo.Extension.ToLower();

                    if (extension == ".app")
                    {
                        int foo = 1;
                    }
                    if (expertContent.Extensions.Contains(extension) == false)
                    {
                        expertContent.Extensions.Add(extension);
                    }
                }

                // Recursively process all subdirectories
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    ProcessDirectory(subDirectoryInfo);
                }
            }

            ProcessDirectory(new DirectoryInfo(directoryPath));
            return;
        }

        void GetExtensionSummary(ExpertContent expertContent)
        {

            foreach (var item in expertContent.Extensions)
            {
                ExtensionSummary extensionSummary = new()
                {
                    Extension = item,
                };
                var foo1 = expertContent.ExpertFiles.Where(x => x.FileInfo.Extension == item);
                extensionSummary.Count = expertContent.ExpertFiles.Where(x => x.FileInfo.Extension.ToLower() == item.ToLower()).Count();
                // 計算出所有 Extension == item 的檔案大小總計
                extensionSummary.Size = expertContent.ExpertFiles.Where(x => x.FileInfo.Extension.ToLower() == item.ToLower()).Sum(x => x.FileInfo.Length);
                expertContent.ExtensionSummaries.Add(extensionSummary);
            }
        }
    }
}
