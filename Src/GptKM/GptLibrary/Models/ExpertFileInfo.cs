namespace GptLibrary.Models
{
    public class ExpertFileInfo
    {
        /// <summary>
        /// 目錄名稱
        /// </summary>
        public string DirectoryName { get; set; } = string.Empty;
        /// <summary>
        /// 副檔案名稱
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// 完整路徑名稱
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// 檔案名稱(沒有路徑資訊)
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long Length { get; set; } = 0L;

        public ExpertFileInfo FromFileInfo(FileInfo fileInfo)
        {
            DirectoryName = fileInfo.DirectoryName;
            Extension = fileInfo.Extension;
            FullName = fileInfo.FullName;
            Name = fileInfo.Name;
            Length = fileInfo.Length;
            return this;
        }
    }
}