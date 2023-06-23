using EntityModel.Entities;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Services;

namespace GptLibrary.Models
{
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
        List<ConvertFileItemModel> ConvertFileItems = new List<ConvertFileItemModel>();
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
                ConvertFileItemModel convertFile = new ConvertFileItemModel();
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

                    convertFile.EmbeddingTextFileName = buildFilenameService.BuildEmbeddingText(expertFile.FullName,embeddingIndex++);
                    convertFile.Index = embeddingIndex;
                    convertFile.SourceText = cutText;
                    convertFile.SourceTextSize = cutText.Length;
                    convertFile.TokenSize = tokenizer.CountToken(cutText);
                    convertFile.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFile.TokenSize);
                    ConvertFileItems.Add(convertFile);
                    #endregion
                }
                else
                {
                    convertFile.EmbeddingTextFileName = buildFilenameService.BuildEmbeddingText(expertFile.FullName, embeddingIndex);
                    convertFile.EmbeddingJsonFileName = buildFilenameService.BuildEmbeddingJson(expertFile.FullName, embeddingIndex);
                    convertFile.Index = embeddingIndex;
                    convertFile.SourceText = cacheSourceText;
                    convertFile.SourceTextSize = cacheSourceText.Length;
                    convertFile.TokenSize = estimateTokens;
                    convertFile.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFile.TokenSize);
                    ConvertFileItems.Add(convertFile);
                    break;
                }
            }
            #endregion
        }
    }
}