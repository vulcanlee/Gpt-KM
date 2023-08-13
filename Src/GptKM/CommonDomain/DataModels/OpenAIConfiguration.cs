namespace CommonDomain.DataModels
{
    public class OpenAIConfiguration
    {
        public string AzureOpenAIKey { get; set; } = string.Empty;
        public string TextEmbeddingAdaModelName { get; set; } = "text-embedding-ada-002";
        public string AzureOpenAIEndpoint { get; set; } = "https://openailabtw.openai.azure.com/";
        public string ChatPromptCompletionModelName { get; set; } = "text-davinci-003";
        public float ChatPromptCompletionTemperature { get; set; } = 0.5f;
        public string DefaultExpertDirectoryName { get; set; } = "本機測試用";
        public string DefaultSourcePath { get; set; } = @"C:\Home\Source";
        public string DefaultConvertPath { get; set; } = @"C:\Home\Convert";
        public int EmbeddingModelTextEmbeddingAda002MaxRequestTokens { get; set; } 
        public int EmbeddingModelTextEmbeddingAda002ResponseTokens { get; set; } 
        public int EmbeddingModelTextEmbeddingAda002RealRequestTokens { get; set; } 
        public int EmbeddingModelTextEmbeddingAda002RealRequestPatchTokens { get; set; } 
        public int IncrementStringAmount { get; set; } 
        public int LanguageModelTextDavinci003MaxRequestTokens { get; set; } 
    }
}
