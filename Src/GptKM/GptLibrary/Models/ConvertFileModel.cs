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
        public async void SplitContext(ExpertFile expertFile,BuildFilenameService buildFilenameService)
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

            int evaluateSize = AzureOpenAIServicePricing
                .EmbeddingModelTextEmbeddingAda002RealRequestTokens + 
                AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002RealRequestPatchTokens;
            int tokens = 0;
            string content = SourceText;
            while (true)
            {
                string reachMaxTokenOfText = content;
                if (reachMaxTokenOfText.Length > evaluateSize)
                {
                    reachMaxTokenOfText = reachMaxTokenOfText.Substring(0, evaluateSize);
                }
                bool needSplitAgain = false;
                while (true)
                {
                    tokens = tokenizer.CountToken(reachMaxTokenOfText);
                    if (tokens > (AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002RealRequestTokens))
                    {
                        reachMaxTokenOfText = reachMaxTokenOfText.Substring(0, reachMaxTokenOfText.Length - 100);
                        needSplitAgain = true;
                    }
                    else
                        break;
                }

                int startIndex = content.Length - reachMaxTokenOfText.Length;
                content = content.Substring(reachMaxTokenOfText.Length);

                #region 新增一筆 Chunk 紀錄
                ConvertFileSplitItemModel convertFileSplit = new ConvertFileSplitItemModel();
                int estimateTokens = tokenizer.CountToken(reachMaxTokenOfText);

                convertFileSplit.EmbeddingJsonFileName =
                    buildFilenameService.BuildEmbeddingText(expertFile.FullName, embeddingIndex++);
                convertFileSplit.EmbeddingTextFileName =
                    buildFilenameService.BuildEmbeddingText(expertFile.FullName, embeddingIndex++);
                convertFileSplit.Index = embeddingIndex;
                convertFileSplit.SourceText = reachMaxTokenOfText;
                convertFileSplit.SourceTextSize = reachMaxTokenOfText.Length;
                convertFileSplit.TokenSize = tokenizer.CountToken(reachMaxTokenOfText);
                convertFileSplit.EmbeddingCost = AzureOpenAIServicePricing
                    .CalculateEmbeddingCost(convertFileSplit.TokenSize);
                ConvertFileSplitItems.Add(convertFileSplit);
                #endregion

                if (needSplitAgain == false) break;
                embeddingIndex++;
            }
            #endregion
        }
    }
}