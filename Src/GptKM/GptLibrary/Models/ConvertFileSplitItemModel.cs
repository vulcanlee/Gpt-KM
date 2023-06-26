namespace GptLibrary.Models
{
    /// <summary>
    /// 切割後的 Chunk 檔案資訊
    /// </summary>
    public class ConvertFileSplitItemModel
    {
        /// <summary>
        /// 檔案索引,代表第幾個 Chunk 項目
        /// </summary>
        public int Index { get; set; }
        public string EmbeddingTextFileName { get; set; } = string.Empty;
        public string EmbeddingJsonFileName { get; set; } = string.Empty;
        public string SourceText { get; set; } = string.Empty;
        public long SourceTextSize { get; set; } = 0L;
        public long ConvertTextSize { get; set; } = 0L;
        public List<float> Embedding { get; set; }
        public string Summary { get; set; } = string.Empty;
        public int TokenSize { get; set; } = 0;
        public Decimal EmbeddingCost { get; set; }
        public Decimal SummaryCost { get; set; }
    }
}