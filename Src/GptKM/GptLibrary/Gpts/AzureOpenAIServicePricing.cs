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
        public static decimal LanguageModelCodeDavinciCost = 0.10m;
        public static decimal EmbeddingModelAdaCost = 0.0004m;
        /// <summary>
        /// Ada002 模型的最大輸入 Token 數量
        /// </summary>
        public static int Ada002MaxRequestTokens = 8_191;
        /// <summary>
        /// 若尚未抵達 Ada002 模型最大支援 Token 數量時，若要增加字串大小時，一次要增加多少字串
        /// </summary>
        public static int IncrementStringAmount = 500;

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
