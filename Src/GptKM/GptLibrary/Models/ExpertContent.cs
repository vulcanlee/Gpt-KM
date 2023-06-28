namespace GptLibrary.Models
{
    /// <summary>
    /// 要進行轉換文字處理的檔案定義類別，將會有指定目錄的掃描結果
    /// </summary>
    public class ExpertContent
    {
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 要進行處理的目標目錄名稱
        /// </summary>
        public string SourceDirectory { get; set; } = string.Empty;
        /// <summary>
        /// 進行檔案轉換後要儲存紀錄的目錄名稱
        /// </summary>
        public string ConvertDirectory { get; set; } = string.Empty;
        /// <summary>
        /// 在指定目錄下，所有搜尋到的檔案紀錄
        /// </summary>
        public List<ExpertRawFile> ExpertRawFiles { get; set; } = new List<ExpertRawFile>();
        /// <summary>
        /// 有紀錄的文件副檔案名稱
        /// </summary>
        public List<string> Extensions { get; set; } = new List<string>();
        /// <summary>
        /// 有紀錄的文件副檔案名稱相關統計摘要
        /// </summary>
        public List<ExtensionSummary> ExtensionSummaries { get; set; } = new List<ExtensionSummary>();
    }
}