using EntityModel.Entities;
using GptLibrary.Converts;
using GptLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Services
{
    public class SyncDirectoryService
    {
        private readonly ConvertFileExtensionMatchService convertFileExtensionMatch;

        public SyncDirectoryService(ConvertFileExtensionMatchService convertFileExtensionMatch)
        {
            this.convertFileExtensionMatch = convertFileExtensionMatch;
        }

        /// <summary>
        /// 開始掃描指定目錄內的所有檔案
        /// </summary>
        /// <param name="expertConfiguration">資料庫內的檔案路徑的定義物件</param>
        /// <returns></returns>
        public ExpertContent ScanSourceDirectory(ExpertDirectory expertDirectory)
        {
            ExpertContent expertContent = new ExpertContent();
            expertContent.SourceDirectory = expertDirectory.SourcePath;
            expertContent.ConvertDirectory = expertDirectory.ConvertPath;
            CountFileExtensions(expertContent);
            PrepareConvertDirectory(expertContent);

            return expertContent;
        }

        /// <summary>
        /// 準備要轉換後檔案需要用到的目錄
        /// </summary>
        /// <param name="expertContent"></param>
        public void PrepareConvertDirectory(ExpertContent expertContent)
        {
            string baseTargetDirectory = expertContent.SourceDirectory;
            string baseConvertDirectory = expertContent.ConvertDirectory;
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
