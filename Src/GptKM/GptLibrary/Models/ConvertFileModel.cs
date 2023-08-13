using Domains.Models;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Services;
using NPOI.HPSF;

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
        public System.Decimal EmbeddingCost { get; set; }
        public System.Decimal SummaryCost { get; set; }

        /// <summary>
        /// 將文字內容切割成為許多 Chunk
        /// </summary>
        public async void SplitContext(ExpertFile expertFile, BuildFilenameService buildFilenameService)
        {
            #region 計算 Embedding 與 Summary 的成本
            if (TokenSize > AzureOpenAIServicePricing.Ada002MaxRequestTokens)
                SummaryCost = AzureOpenAIServicePricing
                    .CalculateSummaryCost(AzureOpenAIServicePricing.Ada002MaxRequestTokens);
            else
                SummaryCost = AzureOpenAIServicePricing.CalculateSummaryCost(TokenSize);
            EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(TokenSize);
            #endregion

            #region 將文字內容切割成為許多 Chunk
            string cacheSourceText = SourceText;
            Tokenizer tokenizer = new Tokenizer();
            int embeddingIndex = 1;

            int evaluateSize = AzureOpenAIServicePricing
                .Ada002MaxRequestTokens;
            int tokens = 0;
            string content = SourceText;
            while (true)
            {
                if (content.Length == 0) break;
                string processingContentString = content;

                #region 若第一次取得的 Token 過小，則先逐步擴大字串數量
                if (processingContentString.Length > evaluateSize)
                {
                    processingContentString = content.Substring(0, evaluateSize);
                    int incrementStringAmount = 0;
                    while (true)
                    {
                        tokens = tokenizer.CountToken(processingContentString);
                        if (tokens < (AzureOpenAIServicePricing.Ada002MaxRequestTokens))
                        {
                            if (processingContentString.Length >= content.Length)
                            {
                                processingContentString = content;
                                break;
                            }
                            else
                            {
                                incrementStringAmount += AzureOpenAIServicePricing.IncrementStringAmount;
                                var checkLength = evaluateSize + incrementStringAmount;
                                if (checkLength >= content.Length)
                                    checkLength = content.Length;
                                processingContentString = content
                                    .Substring(0, checkLength);
                            }
                        }
                        else
                            break;
                    }
                }
                #endregion

                #region 若第一次取得的 Token 過大，則逐步縮小字串數量
                while (true)
                {
                    tokens = tokenizer.CountToken(processingContentString);
                    if (tokens > (AzureOpenAIServicePricing.Ada002MaxRequestTokens))
                    {
                        processingContentString = processingContentString.Substring(0, processingContentString.Length - 100);
                    }
                    else
                        break;
                }
                #endregion

                int startIndex = content.Length - processingContentString.Length;
                content = content.Substring(processingContentString.Length);

                #region 新增一筆 Chunk 紀錄
                ConvertFileSplitItemModel convertFileSplit = new ConvertFileSplitItemModel();
                int estimateTokens = tokenizer.CountToken(processingContentString);

                #region 生成轉換後的目錄檔案名稱
                string convertFileName = expertFile.FullName
                    .Replace(expertFile.ExpertDirectory.SourcePath, expertFile.ExpertDirectory.ConvertPath);

                #endregion
                convertFileSplit.EmbeddingJsonFileName =
                    buildFilenameService.BuildEmbeddingJson(convertFileName, embeddingIndex);
                convertFileSplit.EmbeddingTextFileName =
                    buildFilenameService.BuildEmbeddingText(convertFileName, embeddingIndex);
                convertFileSplit.Index = embeddingIndex;
                convertFileSplit.SourceText = processingContentString;
                convertFileSplit.SourceTextSize = processingContentString.Length;
                convertFileSplit.TokenSize = tokenizer.CountToken(processingContentString);
                convertFileSplit.EmbeddingCost = AzureOpenAIServicePricing
                    .CalculateEmbeddingCost(convertFileSplit.TokenSize);
                ConvertFileSplitItems.Add(convertFileSplit);
                #endregion

                //if (needSplitAgain == false) break;
                embeddingIndex++;
                if (embeddingIndex == 11)
                {
                    int foo = 0;
                }
            }
            #endregion
        }
    }
}