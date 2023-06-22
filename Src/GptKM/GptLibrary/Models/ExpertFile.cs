namespace GptLibrary.Models
{
    public class ExpertRawFile
    {
        /// <summary>
        /// 檔案所在目錄名稱
        /// </summary>
        public string DirectoryName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案完整路徑名稱
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案名稱(沒有路徑資訊)
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案相關資訊(來自於 FileInfo 物件)
        /// </summary>
        public ExpertFileInfo FileInfo { get; set; }
        /// <summary>
        /// 副檔案名稱
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long Size { get; set; } = 0L;
    }
}