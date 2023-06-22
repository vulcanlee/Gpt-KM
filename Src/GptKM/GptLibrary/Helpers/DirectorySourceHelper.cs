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
    public class DirectorySourceHelper
    {
        public ExpertContent Scan(ExpertConfiguration expertConfiguration)
        {
            ExpertContent expertContent = new ExpertContent();
            expertContent.TargetDirectory = expertConfiguration.TargetDirectory;
            expertContent.ConvertDirectory = expertConfiguration.ConvertDirectory;
            CountFileExtensions(expertConfiguration, expertContent);
            GetExtensionSummary(expertContent);

            return expertContent;
        }

        public void Save(string filename, ExpertContent expertContent)
        {
            var content = Newtonsoft.Json.JsonConvert.SerializeObject(expertContent);
            System.IO.File.WriteAllText(filename, content);
        }

        public void Print(ExpertContent expertContent)
        {
            var sum = expertContent.ExpertFiles.Sum(x => x.Size);
            var sumG = sum / 1024.0 / 1024.0;
            Console.WriteLine($"Total Size : {sumG}MB");

            Console.WriteLine($"Summary By File Counts");
            var orderExtensionCounts = expertContent.ExtensionSummaries.OrderByDescending(x => x.Count);
            foreach (var extensionSummary in orderExtensionCounts)
            {
                Console.WriteLine($"{extensionSummary.Extension} / Count:{extensionSummary.Count} / Size:{extensionSummary.Size / 1024 / 1024} MB");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"------------------------------------");
            Console.WriteLine($"Summary By File Size");
            var orderExtensionSize = expertContent.ExtensionSummaries.OrderByDescending(x => x.Size);
            foreach (var extensionSummary in orderExtensionSize)
            {
                Console.WriteLine($"{extensionSummary.Extension} / Count:{extensionSummary.Count} / Size:{extensionSummary.Size / 1024 / 1024} MB");
            }
        }

        public void PrepareConvertDirectory(ExpertConfiguration expertConfiguration, ExpertContent expertContent)
        {
            string baseTargetDirectory = expertConfiguration.TargetDirectory;
            string baseConvertDirectory = expertConfiguration.ConvertDirectory;
            var allDirectories = expertContent.ExpertFiles.Select(x => x.DirectoryName).Distinct();
            foreach (var directory in allDirectories)
            {
                string convertDirectory = directory.Replace(baseTargetDirectory, baseConvertDirectory);
                if (System.IO.Directory.Exists(convertDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(convertDirectory);
                }
            }
        }

        public async Task PrintConverResultAsync(List<ConvertFile> convertFiles)
        {
            DavinciPromptCompletion davinciPromptCompletion = new DavinciPromptCompletion();

            foreach (var file in convertFiles)
            {
                //if (file.FullName.Contains("【TOP】【程式更新歷程】【成大】【FALCONRIS】相關程式修改歷程"))
                //{
                //    int foo = 1;
                //}
                //else
                //    continue;
                Console.WriteLine($"{file.FullName}");
                Console.WriteLine($"Files Size:{file.FileSize / 1024.0 / 1024.0}MB");
                Console.WriteLine($"Text Size:{file.SourceTextSize} , Token:{file.TokenSize}, Embedding:{file.EmbeddingCost}");
                if (file.TokenSize < 3500) continue;
                var summary = await davinciPromptCompletion.GptSummaryAsync(file);
                await Console.Out.WriteLineAsync($"{summary}");
                Console.WriteLine();
            }

            Console.WriteLine($"------------------------------------------------");
            var totalFile = convertFiles.Count;
            var totalFileSize = convertFiles.Sum(x => x.FileSize) / 1024.0 / 1024.0;
            var totalSize = convertFiles.Sum(x => x.SourceTextSize) / 1024.0 / 1024.0;
            var totalTokenSize = convertFiles.Sum(x => x.TokenSize);
            var totalEmbeddingCost = convertFiles.Sum(x => x.EmbeddingCost);
            var totalEmbeddingCostTW = convertFiles.Sum(x => x.EmbeddingCost) * 30;
            Console.WriteLine($"Files:{totalFile} , Files Size:{totalFileSize}MB");
            Console.WriteLine($"Text Size:{totalSize}MB , Token:{totalTokenSize}, Embedding:${totalEmbeddingCost} / NT${totalEmbeddingCostTW}");
        }

        public List<ConvertFile> GenerateText(ContentTypeEnum contentTypeEnum,
            ExpertConfiguration expertConfiguration, ExpertContent expertContent)
        {
            List<ConvertFile> convertFiles = new List<ConvertFile>();
            var allFiles = GetExamFiles(contentTypeEnum, expertConfiguration, expertContent);
            PdfToText pdfToText = new PdfToText();
            HtmlToText htmlToText = new HtmlToText();
            Tokenizer tokenizer = new Tokenizer();
            int count = 0;
            foreach (var file in allFiles)
            {
                Console.Write($"{count} ");
                //if (count > 10)
                //    break;
                count++;
                if (contentTypeEnum == ContentTypeEnum.PDF)
                {
                    #region PDF 2 Text
                    if (!(file.Extension.ToLower() == ".pdf"))
                        continue;
                    //var sourceText = pdfToText.ToText(file.FullName);
                    string sourceText = "";
                    ConvertFile convertFile = new ConvertFile()
                    {
                        FileName = file.FileName,
                        Extension = file.Extension,
                        DirectoryName = file.DirectoryName,
                        FullName = file.FullName,
                    };
                    convertFile.FileName = file.FullName;
                    convertFile.FileSize = file.Size;
                    convertFile.SourceText = sourceText;
                    convertFile.SourceTextSize = sourceText.Length;
                    convertFile.TokenSize = tokenizer.CountToken(sourceText);
                    convertFile.SplitContext();
                    convertFiles.Add(convertFile);
                    #endregion
                }
                else if (contentTypeEnum == ContentTypeEnum.HTML)
                {
                    #region HTML 2 Text
                    if (!(file.Extension.ToLower() == ".htm" || file.Extension.ToLower() == ".html"))
                        continue;
                    var sourceText = htmlToText.ToText(file.FullName);

                    ConvertFile convertFile = new ConvertFile()
                    {
                        FileName = file.FileName,
                        Extension = file.Extension,
                        DirectoryName = file.DirectoryName,
                        FullName = file.FullName,
                    };
                    convertFile.FileName = file.FullName;
                    convertFile.FileSize = file.Size;
                    convertFile.SourceText = sourceText;
                    convertFile.SourceTextSize = sourceText.Length;
                    convertFile.TokenSize = tokenizer.CountToken(sourceText);
                    convertFile.SplitContext();
                    convertFiles.Add(convertFile);
                    #endregion
                }
            }
            Console.WriteLine();
            return convertFiles;
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
            string directoryPath = expertConfiguration.TargetDirectory;
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
