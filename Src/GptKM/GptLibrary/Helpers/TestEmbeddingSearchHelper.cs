using Domains.Models;
using EntityModel.Entities;
using GptLibrary.Gpts;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertLinkLibrary.Helpers
{
    public class EmbeddingItem
    {
        public string FileName { get; set; } = string.Empty;
        public int ChunkIndex { get; set; } = 0;
        public string ChunkContent { get; set; }
        public Vector<float> Embedding { get; set; } = Vector<float>.Build.Dense(0);
        public double CosineSimilarity { get; set; }
    }
    public class TestEmbeddingSearchHelper
    {
        public async Task TestEmbeddingSearch(BackendDBContext context)
        {
            List<EmbeddingItem> allDocumentsEmbedding = new();
            var allFiles = await context.ExpertFile
                .Include(x => x.ExpertDirectory)
                .Where(x => x.ProcessChunk == true)
                .ToListAsync();

            #region 建立 Embedding DB
            await Console.Out.WriteLineAsync($"讀取 Embedding Vectors");
            int count = 1;
            foreach (var file in allFiles)
            {
                for (int convertIndex = 1; convertIndex <= file.ChunkSize; convertIndex++)
                {
                    string chunkembeddingContentFileName = Path
                        .Combine(file.ExpertDirectory.ConvertPath, $"{file.FileName}.{convertIndex}.EmbeddingText");

                    string chunkembeddingFileName = Path
                        .Combine(file.ExpertDirectory.ConvertPath, $"{file.FileName}.{convertIndex}.EmbeddingJson");
                    string embeddingContentContext = await System.IO.File.ReadAllTextAsync(chunkembeddingContentFileName);
                    string embeddingContext = await System.IO.File.ReadAllTextAsync(chunkembeddingFileName);
                    var fileEmbedding = JsonConvert.DeserializeObject<List<float>>(embeddingContext);
                    float[] allValues = fileEmbedding.ToArray();
                    MathNet.Numerics.LinearAlgebra.Vector<float> theEmbedding;
                    theEmbedding = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(allValues);
                    EmbeddingItem embeddingItem = new EmbeddingItem()
                    {
                        ChunkIndex = convertIndex,
                        Embedding = theEmbedding,
                        FileName = file.FullName,
                        ChunkContent = embeddingContentContext,
                    };
                    allDocumentsEmbedding.Add(embeddingItem);
                    await Console.Out.WriteAsync($"{count++} ");
                }
            }
            #endregion

            AdaEmbeddingVector adaEmbeddingVector = new AdaEmbeddingVector();
            while (true)
            {
                await Console.Out.WriteLineAsync();
                Console.WriteLine("請輸入你的問題:");
                string question = Console.ReadLine();
                float[] questionEmbedding = await adaEmbeddingVector.GetEmbeddingAsync(question);

                List<EmbeddingItem> allDocumentsCosineSimilarity = new();
                foreach (var item in allDocumentsEmbedding)
                {
                    // calculate cosine similarity
                    var v2 = item.Embedding;
                    var v1 = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(questionEmbedding); ;
                    var cosineSimilarity = v1.DotProduct(v2) / (v1.L2Norm() * v2.L2Norm());
                    item.CosineSimilarity = cosineSimilarity;
                    allDocumentsCosineSimilarity.Add(item);
                }
                var items = allDocumentsCosineSimilarity.OrderByDescending(x => x.CosineSimilarity).Take(8).ToList();
                int countChunk = 1;
                foreach (var item in items)
                {
                    await Console.Out.WriteLineAsync($"Id:{countChunk++}  Chunk:{item.ChunkIndex}   {item.CosineSimilarity} - {item.FileName}");
                }
                string chunkMessage = "";
                while (true)
                {
                    Console.WriteLine("要查看的 Chunk 內容(Esc 結束，或輸入 Id 數字:");
                    var chunkId = Console.ReadKey();
                    if (chunkId.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    try
                    {
                        int number = chunkId.Key - ConsoleKey.D1;
                        chunkMessage = items[number].ChunkContent;
                        await Console.Out.WriteLineAsync(chunkMessage);
                        await Console.Out.WriteLineAsync();
                        Console.WriteLine("是否要交由 GPT 做出這段文字的摘要 (請輸入 Y 或 N):");
                        var askGPTSummary = Console.ReadKey();
                        if (askGPTSummary.Key == ConsoleKey.Y)
                        {
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync("請稍後，送至 GPT 做出這段文字的摘要中");
                            DavinciPromptCompletion davinciPromptCompletion = new DavinciPromptCompletion();
                            var answer = await davinciPromptCompletion.GptSummaryAsync(chunkMessage+"\n\n摘要內容:");
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync(answer);
                            await Console.Out.WriteLineAsync();
                        }
                        Console.WriteLine("是否要交由 GPT 回答此問題 (請輸入 Y 或 N):");
                        var askGPT = Console.ReadKey();
                        if (askGPT.Key == ConsoleKey.Y)
                        {
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync("請稍後，送至 GPT 分析中");
                            DavinciPromptCompletion davinciPromptCompletion = new DavinciPromptCompletion();
                            var answer = await davinciPromptCompletion.GptSummaryAsync(chunkMessage, $"請使用底下提示文字，回答這個問題:\"{question}\"");
                            await Console.Out.WriteLineAsync();
                            await Console.Out.WriteLineAsync(answer);
                            await Console.Out.WriteLineAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync(ex.Message);
                    }
                }
            }
        }
    }
}
