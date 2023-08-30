namespace CommonDomain.DataModels
{
    public enum AIEngineEnum
    {
        AzureOpenAI,
        Twcs,
    }

    public class OpenAIConfiguration
    {
        public AIEngineEnum AIEngine { get; set; } = AIEngineEnum.AzureOpenAI;
        public string AzureOpenAIKey { get; set; } = string.Empty;
        public string TextEmbeddingAdaModelName { get; set; } = "text-embedding-ada-002";
        public string ChatPromptCompletionModelName { get; set; } = "text-davinci-003";
        public string AzureOpenAIEndpoint { get; set; } = "https://openailabtw.openai.azure.com/";
        public float ChatPromptCompletionTemperature { get; set; } = 0.5f;
        public string TwcsAPI_KEY { get; set; } = "";
        public string TwcsGPTEndpoint { get; set; } = "";
        public string TwcsEmbeddingEndpoint { get; set; } = "";
        public string DefaultExpertDirectoryName { get; set; } = "本機測試用";
        public string DefaultSourcePath { get; set; } = @"C:\Home\Source";
        public string DefaultConvertPath { get; set; } = @"C:\Home\Convert";
        /// <summary>
        /// Ada002 模型的最大輸入 Token 數量
        /// </summary>
        public int Ada002MaxRequestTokens { get; set; }
        /// <summary>
        /// 若尚未抵達 Ada002 模型最大支援 Token 數量時，若要增加字串大小時，一次要增加多少字串
        /// </summary>
        public int IncrementStringAmount { get; set; } 
    }
}
