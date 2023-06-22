using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GptLibrary.Models;
using GptLibrary.Gpt;

namespace GptLibrary.Gpts
{
    public class DavinciPromptCompletion
    {
        public async Task<string> GptSummaryAsync(ConvertFile convertFile, string prefix = "請將底下內容整理出摘要內容，並使用zh-tw生成內容")
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
            var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");
            string endpoint = "https://openailabtw.openai.azure.com/";
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            #endregion

            #region 準備使用 OpenAI GPT 的 Prompt / Completion 模式呼叫 API

            string prompt = $"{prefix}\n\n{content}:";
            string completion = string.Empty;
            //await Console.Out.WriteLineAsync(prompt);

            var completionsOptions = new CompletionsOptions()
            {
                Prompts = { prompt },
                MaxTokens = 500,
                Temperature = 0.3f,
            };

            string deploymentName = "text-davinci-003";

            Response<Completions> completionsResponse = await client
                .GetCompletionsAsync(deploymentName, completionsOptions);
            if (completionsResponse != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in completionsResponse.Value.Choices)
                {
                    sb.Append(item.Text);
                }
                completion = sb.ToString();
                //await Console.Out.WriteLineAsync($"底下是 OpenAI 回覆的內容:");
                //Console.WriteLine($"{completion}");
            }
            #endregion
            return completion;
        }
    }
}
