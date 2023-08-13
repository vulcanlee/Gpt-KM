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
        public static int LanguageModelTextDavinci003MaxRequestTokens = 4_097;
        public static decimal EmbeddingModelAdaCost = 0.0004m;
        public static int EmbeddingModelTextEmbeddingAda002MaxRequestTokens = 8_191;
        public static int EmbeddingModelTextEmbeddingAda002ResponseTokens = 500;
        /// <summary>
        /// 要取得的字串大小，此時，不是使用 Token 來計算
        /// </summary>
        public static int EmbeddingModelTextEmbeddingAda002RealRequestTokens = 2500;
        /// <summary>
        /// 若要增加字串大小時，一次要增加多少字串
        /// </summary>
        public static int EmbeddingModelTextEmbeddingAda002RealRequestPatchTokens = 1000;
        /// <summary>
        /// 若要增加字串大小時，一次要增加多少字串
        /// </summary>
        public static int IncrementStringAmount = 800;

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
