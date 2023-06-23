using GptLibrary.Gpt;
using GptLibrary.Gpts;

namespace GptLibrary.Models
{
    public class ConvertFile
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string SourceText { get; set; } = string.Empty;
        public long FileSize { get; set; } = 0L;
        public long SourceTextSize { get; set; } = 0L;
        public int TokenSize { get; set; } = 0;
        List<ConvertFileItem> ConvertFileItems = new List<ConvertFileItem>();
        public Decimal EmbeddingCost { get; set; }
        public Decimal SummaryCost { get; set; }

        /// <summary>
        /// 將文字內容切割成為許多 Chunk
        /// </summary>
        public void SplitContext()
        {
            EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(TokenSize);
            if (TokenSize > AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens)
                SummaryCost = AzureOpenAIServicePricing.CalculateSummaryCost(AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens);
            else
                SummaryCost = AzureOpenAIServicePricing.CalculateSummaryCost(TokenSize);

            string cacheSourceText = SourceText;
            Tokenizer tokenizer = new Tokenizer();
            while (true)
            {
                ConvertFileItem convertFile = new ConvertFileItem();
                int estimateTokens = tokenizer.CountToken(cacheSourceText);
                if (estimateTokens > AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens)
                {
                    #region 切割多個 Embedding
                    var cutLength = cacheSourceText.Length> AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens?
                        AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002MaxRequestTokens:cacheSourceText.Length;
                    string cutText = cacheSourceText.Substring(0, cutLength);

                    cacheSourceText = cacheSourceText.Substring(cutLength);
                    convertFile.SourceText = cutText;
                    convertFile.SourceTextSize = cutText.Length;
                    convertFile.TokenSize = tokenizer.CountToken(cutText);
                    convertFile.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFile.TokenSize);
                    ConvertFileItems.Add(convertFile);
                    #endregion
                }
                else
                {
                    convertFile.SourceText = cacheSourceText;
                    convertFile.SourceTextSize = cacheSourceText.Length;
                    convertFile.TokenSize = estimateTokens;
                    convertFile.EmbeddingCost = AzureOpenAIServicePricing.CalculateEmbeddingCost(convertFile.TokenSize);
                    ConvertFileItems.Add(convertFile);
                    break;
                }
            }
        }
    }
    public class ConvertFileItem
    {
        public string FileName { get; set; } = string.Empty;
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