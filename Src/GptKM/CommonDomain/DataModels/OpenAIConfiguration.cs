namespace CommonDomain.DataModels
{
    public class OpenAIConfiguration
    {
        public string AzureOpenAIKey { get; set; } = string.Empty;
        public string TextEmbeddingAdaModelName { get; set; } = "text-embedding-ada-002";
        public string AzureOpenAIEndpoint { get; set; } = "https://openailabtw.openai.azure.com/";
        public string TextDavinciModelName { get; set; } = "text-davinci-003";
    }
}
