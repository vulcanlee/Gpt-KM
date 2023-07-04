using Domains.Models;
using GptLibrary.Models;

namespace GptLibrary.Services
{
    public class SyncDirectoryService
    {
        private readonly ConvertFileExtensionMatchService convertFileExtensionMatch;
        private readonly GptExpertDirectoryService gptExpertDirectoryService;
        private readonly GptExpertFileService gptExpertFileService;

        public SyncDirectoryService(ConvertFileExtensionMatchService convertFileExtensionMatch,
            GptExpertDirectoryService gptExpertDirectoryService,
            GptExpertFileService gptExpertFileService)
        {
            this.convertFileExtensionMatch = convertFileExtensionMatch;
            this.gptExpertDirectoryService = gptExpertDirectoryService;
            this.gptExpertFileService = gptExpertFileService;
        }

        /// <summary>
        /// 開始掃描指定目錄內的所有檔案
        /// </summary>
        /// <param name="expertConfiguration">資料庫內的檔案路徑的定義物件</param>
        /// <returns></returns>
        public async Task<ExpertContent> ScanSourceDirectory(ExpertDirectory expertDirectory)
        {
            ExpertContent expertContent = new ExpertContent();
            expertContent.Name = expertDirectory.Name;

            #region 檢查此對應目錄是否存在
            var expertDirectoryResult = await gptExpertDirectoryService.GetAsync(expertDirectory.Id);
            if (expertDirectoryResult.Status == false)
            {
                return expertContent;
            }
            #endregion

            expertContent.SourceDirectory = expertDirectory.SourcePath;
            expertContent.ConvertDirectory = expertDirectory.ConvertPath;
            await ExplorerDirectoryAsync(expertContent);
            await PrepareConvertDirectoryAsync(expertContent);

            return expertContent;
        }

        /// <summary>
        /// 準備要轉換後檔案需要用到的目錄
        /// </summary>
        /// <param name="expertContent"></param>
        async Task PrepareConvertDirectoryAsync(ExpertContent expertContent)
        {
            await Task.Run(() =>
            {
                string baseTargetDirectory = expertContent.SourceDirectory;
                string baseConvertDirectory = expertContent.ConvertDirectory;
                var allDirectories = expertContent.ExpertRawFiles
                    .Select(x => x.DirectoryName).Distinct();
                foreach (var directory in allDirectories)
                {
                    string convertDirectory = directory.Replace(baseTargetDirectory, baseConvertDirectory);
                    if (System.IO.Directory.Exists(convertDirectory) == false)
                    {
                        System.IO.Directory.CreateDirectory(convertDirectory);
                    }
                }
            });
        }

        /// <summary>
        /// 分析指定目錄下，蒐集所有檔案的副檔名符合可以進行文字轉換的清單
        /// </summary>
        /// <param name="expertContent"></param>
        async Task ExplorerDirectoryAsync(ExpertContent expertContent)
        {
            string sourceDirectoryPath = expertContent.SourceDirectory;

            #region Inline Method : Process the list of files found in the directory
            async Task ProcessDirectoryAsync(DirectoryInfo directoryInfo)
            {
                // Process all files in the current directory
                var files = directoryInfo.GetFiles();
                foreach (var fileInfo in files)
                {
                    if (convertFileExtensionMatch.IsMatch(fileInfo.Name))
                    {
                        var expertFileResult =
                            await gptExpertFileService.GetAsync(fileInfo.FullName);
                        bool addFile = true;

                        #region 除非該檔案已經處理完成，否則，若資料庫內有此紀錄，一樣要繼續處理
                        if (expertFileResult.Status == true)
                        {
                            var expertFile = expertFileResult.Payload;
                            if(expertFile.ProcessingStatus != CommonDomain.Enums.ExpertFileStatusEnum.Finish)
                            {
                                addFile = true;
                            }
                        }
                        #endregion

                        if (addFile)
                        {
                            #region 僅處理不存在資料庫內的檔案
                            ExpertRawFile expertRawFile = new ExpertRawFile()
                            {
                                Extension = fileInfo.Extension.ToLower(),
                                FileInfo = new ExpertFileInfo().FromFileInfo(fileInfo),
                                FullName = fileInfo.FullName,
                                FileName = fileInfo.Name,
                                Size = fileInfo.Length,
                                DirectoryName = $@"{fileInfo.DirectoryName}\",
                            };
                            expertContent.ExpertRawFiles.Add(expertRawFile);
                            #endregion
                        }
                    }
                }

                #region Recursively process all subdirectories
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    await ProcessDirectoryAsync(subDirectoryInfo);
                }
                #endregion
            }
            #endregion

            // 開始探索這個目錄，找出所有可以轉換成為文字的類型檔案
            await ProcessDirectoryAsync(new DirectoryInfo(sourceDirectoryPath));
            return;
        }
    }
}
