using GptLibrary.Converts;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Models;
using GptLibrary.Services;

namespace GptLibrary.Helpers
{
    public class DirectorySourceHelper
    {
        private readonly ConvertFileExtensionMatchService convertFileExtensionMatch;
        private readonly ConverterToTextFactory converterToTextFactory;
        private readonly DavinciPromptCompletion davinciPromptCompletion;

        public DirectorySourceHelper(ConvertFileExtensionMatchService convertFileExtensionMatch,
            ConverterToTextFactory converterToTextFactory, DavinciPromptCompletion davinciPromptCompletion)
        {
            this.convertFileExtensionMatch = convertFileExtensionMatch;
            this.converterToTextFactory = converterToTextFactory;
            this.davinciPromptCompletion = davinciPromptCompletion;
        }
        public ExpertContent Scan(ExpertConfiguration expertConfiguration)
        {
            ExpertContent expertContent = new ExpertContent();
            expertContent.SourceDirectory = expertConfiguration.SourceDirectory;
            expertContent.ConvertDirectory = expertConfiguration.ConvertDirectory;
            CountFileExtensions(expertContent);

            return expertContent;
        }

        /// <summary>
        /// 將搜尋到的可轉換成為文字的所有檔案定義資訊，寫入到 JSON 檔案內
        /// </summary>
        /// <param name="filename">定義檔案名稱</param>
        /// <param name="expertContent">搜尋到的所有檔案資訊</param>
        public async Task SaveAsync(string filename, ExpertContent expertContent)
        {
            var content = Newtonsoft.Json.JsonConvert.SerializeObject(expertContent);
            await System.IO.File.WriteAllTextAsync(filename, content);
        }

        /// <summary>
        /// 將搜尋到的可轉換成為文字的所有檔案定義資訊，列印出來
        /// </summary>
        /// <param name="expertContent">搜尋到的所有檔案資訊</param>
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

        /// <summary>
        /// 準備要轉換後檔案需要用到的目錄
        /// </summary>
        /// <param name="expertConfiguration"></param>
        /// <param name="expertContent"></param>
        public void PrepareConvertDirectory(ExpertConfiguration expertConfiguration, ExpertContent expertContent)
        {
            string baseTargetDirectory = expertConfiguration.SourceDirectory;
            string baseConvertDirectory = expertConfiguration.ConvertDirectory;
            var allDirectories = expertContent.ExpertFiles
                .Select(x => x.DirectoryName).Distinct();
            foreach (var directory in allDirectories)
            {
                string convertDirectory = directory.Replace(baseTargetDirectory, baseConvertDirectory);
                if (System.IO.Directory.Exists(convertDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(convertDirectory);
                }
            }
        }

        /// <summary>
        /// 使用 Davinci GPT 模型，將檔案轉換成為所有文字內容，生成出摘要
        /// </summary>
        /// <param name="convertFiles"></param>
        /// <returns></returns>
        public async Task PrintConverResultAsync(List<ConvertFileModel> convertFiles)
        {
            foreach (var file in convertFiles)
            {
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

        /// <summary>
        /// 進行所有檔案的轉成文字化動作
        /// </summary>
        /// <param name="contentTypeEnum"></param>
        /// <param name="expertContent"></param>
        /// <returns></returns>
        public List<ConvertFileModel> GenerateText(ContentTypeEnum contentTypeEnum,
            ExpertContent expertContent)
        {
            List<ConvertFileModel> convertFiles = new List<ConvertFileModel>();
            var allFiles = GetExamFiles(contentTypeEnum, expertContent);
            IFileToText fileToText = converterToTextFactory.Create(contentTypeEnum);
            Tokenizer tokenizer = new Tokenizer();
           
            #region 列舉出所有的符合檔案
            foreach (var file in allFiles)
            {
                #region 將檔案內容，轉換成為文字
                string sourceText = fileToText.ToText(file.FullName);
                ConvertFileModel convertFile = new ConvertFileModel()
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
                //convertFile.SplitContext();
                convertFiles.Add(convertFile);
                #endregion
            }
            #endregion
            return convertFiles;
        }

        /// <summary>
        /// 找出所指定類型的所有相關檔案清單
        /// </summary>
        /// <param name="contentTypeEnum">指定類型</param>
        /// <param name="expertContent">已經找出來在實體目錄下的所有檔案清單</param>
        /// <returns></returns>
        List<ExpertRawFile> GetExamFiles(ContentTypeEnum contentTypeEnum,
            ExpertContent expertContent)
        {
            List<ExpertRawFile> convertFiles = new List<ExpertRawFile>();
            var contentTypes = ContentType.GetContentType(contentTypeEnum);
            var expertFiles = expertContent.ExpertFiles
                .Where(x => contentTypes.Contains(x.Extension));

            foreach (var extension in expertFiles)
            {
                convertFiles.Add(extension);
            }
            return convertFiles;
        }

        /// <summary>
        /// 分析指定目錄下，所有檔案的副檔名符合可以進行文字轉換的清單
        /// </summary>
        /// <param name="expertContent"></param>
        void CountFileExtensions(ExpertContent expertContent)
        {
            string sourceDirectoryPath = expertContent.SourceDirectory;

            #region Inline Method : Process the list of files found in the directory
            void ProcessDirectory(DirectoryInfo directoryInfo)
            {
                // Process all files in the current directory
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    if (convertFileExtensionMatch.IsMatch(fileInfo.Name))
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
                    }
                }

                #region Recursively process all subdirectories
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    ProcessDirectory(subDirectoryInfo);
                }
                #endregion
            }
            #endregion

            // 開始探索這個目錄，找出所有可以轉換成為文字的類型檔案
            ProcessDirectory(new DirectoryInfo(sourceDirectoryPath));
            return;
        }
    }
}
