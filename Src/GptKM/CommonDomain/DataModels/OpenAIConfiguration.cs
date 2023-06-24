namespace CommonDomain.DataModels
{
    public class OpenAIConfiguration
    {
        public string AzureOpenAIKey { get; set; } = string.Empty;
        public string TextEmbeddingAdaModelName { get; set; } = string.Empty;
        public string AzureOpenAIEndpoint { get; set; } = string.Empty;
        public string TextDavinciModelName { get; set; } = string.Empty;
    }
}
