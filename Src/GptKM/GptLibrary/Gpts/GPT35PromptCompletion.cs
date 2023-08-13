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
    public class GPT35PromptCompletion
    {
        private readonly OpenAIConfiguration openAIConfiguration;

        public GPT35PromptCompletion(OpenAIConfiguration openAIConfiguration)
        {
            this.openAIConfiguration = openAIConfiguration;
        }

        public async Task<string> GptAnswerQuestionAsync(string content, 
            string prefix = "依據底下內容，回答這個問題")
        {
            #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
            var apiKey = openAIConfiguration.AzureOpenAIKey;
            string endpoint = openAIConfiguration.AzureOpenAIEndpoint;
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            #region 準備使用 OpenAI GPT 的 Prompt / Completion 模式呼叫 API

            string prompt = $"{prefix}\n\n{content}";
            string completion = string.Empty;
            //await Console.Out.WriteLineAsync(prompt);

            #region GPT 3.5 / 4 使用
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "若無法找出合適答案，請回覆，此問題找不到答案"),
                    new ChatMessage(ChatRole.User, prompt),
                },
                Temperature = openAIConfiguration.ChatPromptCompletionTemperature,
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
        }


        public async Task<string> GptSummaryAsync(string content,
            string prefix = "將底下內容，產生摘要說明內容")
        {
            #region 使用 Azure.AI.OpenAI 套件來 OpenAIClient 物件
            var apiKey = openAIConfiguration.AzureOpenAIKey;
            string endpoint = openAIConfiguration.AzureOpenAIEndpoint;
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            #region 準備使用 OpenAI GPT 的 Prompt / Completion 模式呼叫 API

            string prompt = $"{prefix}\n\n{content}";
            string completion = string.Empty;
            //await Console.Out.WriteLineAsync(prompt);

            #region GPT 3.5 / 4 使用
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "若無法找出合適答案，請回覆，此問題找不到答案"),
                    new ChatMessage(ChatRole.User, prompt),
                },
                Temperature = openAIConfiguration.ChatPromptCompletionTemperature,
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
        }
    }
}
