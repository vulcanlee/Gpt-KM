using EntityModel.Entities;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Services;

namespace GptLibrary.Models
{
    /// <summary>
    /// 進行文字轉換與切割處理需求之類別
    /// </summary>
    public class ConvertFileModel
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string SourceText { get; set; } = string.Empty;
        public long FileSize { get; set; } = 0L;
        public long SourceTextSize { get; set; } = 0L;
        public int TokenSize { get; set; } = 0;
        /// <summary>
        /// 將一個檔案切割成為不同 Chunk 的相關資訊
        /// </summary>
        public List<ConvertFileSplitItemModel> ConvertFileSplitItems = new List<ConvertFileSplitItemModel>();
        public Decimal EmbeddingCost { get; set; }
        public Decimal SummaryCost { get; set; }

        /// <summary>
        /// 將文字內容切割成為許多 Chunk
        /// </summary>
        public void SplitContext(ExpertFile expertFile,BuildFilenameService buildFilenameService)
        {
            #region 計算 Embedding 與 Summary 的成本
            EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(TokenSize);
            if (TokenSize > AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens)
                SummaryCost = AzureOpenAIServicePricing.CalculateSummaryCost(AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens);
            else
                SummaryCost = AzureOpenAIServicePricing.CalculateSummaryCost(TokenSize);
            #endregion

            #region 將文字內容切割成為許多 Chunk
            string cacheSourceText = SourceText;
            Tokenizer tokenizer = new Tokenizer();
            int embeddingIndex = 1;

            while (true)
            {
                ConvertFileSplitItemModel convertFileSplit = new ConvertFileSplitItemModel();
                int estimateTokens = tokenizer.CountToken(cacheSourceText);
                if (estimateTokens > AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens)
                {
                    #region 切割多個 Embedding
                    var cutLength = cacheSourceText.Length >
                        AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens ?
                        AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens :
                        cacheSourceText.Length;
                    string cutText = cacheSourceText.Substring(0, cutLength);

                    cacheSourceText = cacheSourceText.Substring(cutLength);

                    convertFileSplit.EmbeddingTextFileName = buildFilenameService.BuildEmbeddingText(expertFile.FullName,embeddingIndex++);
                    convertFileSplit.Index = embeddingIndex;
                    convertFileSplit.SourceText = cutText;
                    convertFileSplit.SourceTextSize = cutText.Length;
                    convertFileSplit.TokenSize = tokenizer.CountToken(cutText);
                    convertFileSplit.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFileSplit.TokenSize);
                    ConvertFileSplitItems.Add(convertFileSplit);
                    #endregion
                }
                else
                {
                    convertFileSplit.EmbeddingTextFileName = buildFilenameService.BuildEmbeddingText(expertFile.FullName, embeddingIndex);
                    convertFileSplit.EmbeddingJsonFileName = buildFilenameService.BuildEmbeddingJson(expertFile.FullName, embeddingIndex);
                    convertFileSplit.Index = embeddingIndex;
                    convertFileSplit.SourceText = cacheSourceText;
                    convertFileSplit.SourceTextSize = cacheSourceText.Length;
                    convertFileSplit.TokenSize = estimateTokens;
                    convertFileSplit.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFileSplit.TokenSize);
                    ConvertFileSplitItems.Add(convertFileSplit);
                    break;
                }
            }
            #endregion
        }
    }
}