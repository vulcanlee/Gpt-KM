namespace GptLibrary.Models
{
    public class ExtensionSummary
    {
        /// <summary>
        /// 副檔案名稱
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// 檔案大小
        /// </summary>
        public long Size { get; set; } = 0L;
        /// <summary>
        /// 檔案數量
        /// </summary>
        public long Count { get; set; } = 0L;
    }
}