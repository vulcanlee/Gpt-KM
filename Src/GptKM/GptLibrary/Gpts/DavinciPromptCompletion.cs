using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GptLibrary.Models;
using GptLibrary.Gpt;
using CommonDomain.DataModels;

namespace GptLibrary.Gpts
{
    public class DavinciPromptCompletion
    {
        private readonly OpenAIConfiguration openAIConfiguration;

        public DavinciPromptCompletion(OpenAIConfiguration openAIConfiguration)
        {
            this.openAIConfiguration = openAIConfiguration;
        }
        public async Task<string> GptSummaryAsync(ConvertFileModel convertFile, string prefix = "請將底下內容整理出摘要內容，並使用zh-tw生成內容")
        {
            string content = convertFile.SourceText;
            int maxCotentLength = 0;
            if (convertFile.SourceText.Length > AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens)
            {
                maxCotentLength = AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens;
                content = content.Substring(0, maxCotentLength);
            }

            Tokenizer tokenizer = new Tokenizer();
            int tokens = 0;
            while (true)
            {
                tokens = tokenizer.CountToken(content);
                if (tokens > (AzureOpenAIServicePricing.LanguageModelTextDavinci003MaxRequestTokens - 200))
                    content = content.Substring(0, content.Length - 100);
                else
                    break;
            }
            var result = await GptSummaryAsync(content);
            return result;
        }

        public async Task<string> GptSummaryAsync(string content, string prefix = "做出底下摘要內容,控制摘要在300字內")
        {
            #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
            var apiKey = openAIConfiguration.AzureOpenAIKey;
            string endpoint = openAIConfiguration.AzureOpenAIEndpoint;
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            #region 準備使用 OpenAI GPT 的 Prompt / Completion 模式呼叫 API

            string prompt = $"{prefix}\n\n{content}:";
            string completion = string.Empty;
            //await Console.Out.WriteLineAsync(prompt);

            #region GPT 3.5 / 4 使用
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, prefix),
                    new ChatMessage(ChatRole.User, prompt),
                }
            };

            Response<StreamingChatCompletions> response = await client
                .GetChatCompletionsStreamingAsync(
                deploymentOrModelName: openAIConfiguration.ChatPromptCompletionModelName,
                chatCompletionsOptions);
            using StreamingChatCompletions streamingChatCompletions = response.Value;

            StringBuilder sb = new StringBuilder();
            await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
            {
                await foreach (ChatMessage message in choice.GetMessageStreaming())
                {
                    sb.Append(message.Content);
                }
            }
            completion = sb.ToString();
            return completion;

            #endregion
            #endregion

            //var completionsOptions = new CompletionsOptions()
            //{
            //    Prompts = { prompt },
            //    MaxTokens = 500,
            //    Temperature = 0.3f,
            //};

            //string deploymentName = openAIConfiguration.TextPromptCompletionModelName;

            //Response<Completions> completionsResponse = await client
            //    .GetCompletionsAsync(deploymentName, completionsOptions);
            //if (completionsResponse != null)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (var item in completionsResponse.Value.Choices)
            //    {
            //        sb.Append(item.Text);
            //    }
            //    completion = sb.ToString();
            //    //await Console.Out.WriteLineAsync($"底下是 OpenAI 回覆的內容:");
            //    //Console.WriteLine($"{completion}");
            //}
            //#endregion
            //return completion;
        }
    }
}
