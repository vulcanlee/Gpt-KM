using GptLibrary.Gpt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Gpts
{
    public class AzureOpenAIServicePricing
    {
        public static readonly decimal LanguageModelCodeDavinciCost = 0.10m;
        public static readonly int LanguageModelTextDavinci003MaxRequestTokens = 4_097;
        public static readonly decimal EmbeddingModelAdaCost = 0.0004m;
        public static readonly int EmbeddingModelTextEmbeddingAda002MaxRequestTokens = 8_191;
        public static readonly int EmbeddingModelTextEmbeddingAda002ResponseTokens = 500;
        public static readonly int EmbeddingModelTextEmbeddingAda002RealRequestTokens = 2500;

        public static decimal CalculateEmbeddingCost(int tokenCount)
        {
            var EmbeddingCost = AzureOpenAIServicePricing.EmbeddingModelAdaCost * tokenCount / 1000m;
            return EmbeddingCost;
        }

        public static decimal CalculateSummaryCost(int tokenCount)
        {
            var SummaryCost = AzureOpenAIServicePricing.LanguageModelCodeDavinciCost * tokenCount / 1000m;
            return SummaryCost;
        }
    }
}
