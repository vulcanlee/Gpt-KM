namespace GptLibrary.Models
{
    public class ExpertConfiguration
    {
        /// <summary>
        /// 要進行處理的目標目錄名稱
        /// </summary>
        public string SourceDirectory { get; set; }=string.Empty;
        /// <summary>
        /// 進行檔案轉換後要儲存紀錄的目錄名稱
        /// </summary>
        public string ConvertDirectory { get; set; }=string.Empty;
    }
}