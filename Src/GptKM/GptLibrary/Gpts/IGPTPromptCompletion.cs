namespace GptLibrary.Gpts
{
    public interface IGPTPromptCompletion
    {
        Task<string> GptAnswerQuestionAsync(string content, string prefix = "依據底下內容，回答這個問題");
        Task<string> GptSummaryAsync(string content, string prefix = "將底下內容，產生摘要說明內容");
    }
}