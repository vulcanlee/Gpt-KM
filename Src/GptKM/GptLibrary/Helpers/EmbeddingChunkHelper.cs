using Domains.Models;
using EntityModel.Entities;
using GptLibrary.Gpt;
using GptLibrary.Gpts;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers
{
    public class EmbeddingChunkHelper
    {
        public async Task ToEmbeddingAsync(ContentTypeEnum contentTypeEnum, 
            BackendDBContext context, string directoryName)
        {
            ExtractFileToTextHelper extractFileToTextHelper = new ExtractFileToTextHelper();
            ExpertDirectory expertDirectory = await context.ExpertDirectory.FirstOrDefaultAsync(x => x.Path == directoryName);
            if (expertDirectory != null)
            {
                List<ExpertFile> expertFiles = await context.ExpertFile.Where(x => x.ExpertDirectoryId == expertDirectory.Id &&
                x.ProcessChunk == false)
                    .ToListAsync();
                var convertPath = expertDirectory.ConvertPath;

                Tokenizer tokenizer = new Tokenizer();
                AdaEmbeddingVector adaEmbeddingVector = new AdaEmbeddingVector();
                int convertIndex = 1;
                int count = 1;
                foreach (var expertFile in expertFiles)
                {
                    convertIndex = 1;
                    string fileContent = string.Empty;
                    switch (contentTypeEnum)
                    {
                        case ContentTypeEnum.PDF:
                            fileContent = await extractFileToTextHelper.ConvertToTextAsync(Models.ContentTypeEnum.PDF,
                                Path.Combine(directoryName, expertFile.FileName),
                                Path.Combine(convertPath, $"{expertFile.FileName}.ConvertedText"));
                            break;
                        case ContentTypeEnum.WORD:
                            break;
                        case ContentTypeEnum.EXCEL:
                            break;
                        case ContentTypeEnum.POWERPOINT:
                            break;
                        case ContentTypeEnum.HTML:
                            fileContent = await extractFileToTextHelper.ConvertToTextAsync(Models.ContentTypeEnum.HTML,
                                Path.Combine(directoryName, expertFile.FileName),
                                Path.Combine(convertPath, $"{expertFile.FileName}.ConvertedText"));
                            break;
                        case ContentTypeEnum.TEXT:
                            break;
                        case ContentTypeEnum.MARKDOWN:
                            break;
                        default:
                            break;
                    }
                    if (fileContent == null || fileContent.Length == 0) continue;
                    int tokens = 0;
                    string content = fileContent;
                    while (true)
                    {
                        int evaluateSize = AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002RealRequestTokens + 1000;
                        string reachMaxTokenOfText = content;
                        if(reachMaxTokenOfText.Length > evaluateSize)
                        {
                            reachMaxTokenOfText = reachMaxTokenOfText.Substring(0, evaluateSize);
                        }
                        bool needSplitAgain = false;
                        while (true)
                        {
                            tokens = tokenizer.CountToken(reachMaxTokenOfText);
                            if (tokens > (AzureOpenAIServicePricing.EmbeddingModelTextEmbeddingAda002RealRequestTokens))
                            {
                                reachMaxTokenOfText = reachMaxTokenOfText.Substring(0, reachMaxTokenOfText.Length - 100);
                                needSplitAgain = true;
                            }
                            else
                                break;
                        }

                        string chunkFileName = Path
                            .Combine(convertPath, $"{expertFile.FileName}.{convertIndex}.EmbeddingText");
                        await System.IO.File.WriteAllTextAsync(chunkFileName, reachMaxTokenOfText);
                        int startIndex = content.Length - reachMaxTokenOfText.Length;

                        content = content.Substring(reachMaxTokenOfText.Length);
                   
                        #region 新增一筆 Chunk 紀錄
                        var checkExpertFileChunk = await context.ExpertFileChunk
                            .FirstOrDefaultAsync(x => x.FullName == chunkFileName);
                        if (checkExpertFileChunk == null)
                        {
                            ExpertFileChunk expertFileChunk = new ExpertFileChunk()
                            {
                                Size = reachMaxTokenOfText.Length,
                                ExpertFileId = expertFile.Id,
                                DirectoryName = expertFile.DirectoryName,
                                FullName = chunkFileName,
                                FileName = Path.GetFileName(chunkFileName),
                            };
                            context.ExpertFileChunk.Add(expertFileChunk);
                            await context.SaveChangesAsync();
                            await Console.Out.WriteLineAsync($"{expertFile.FileName}");
                        }
                        else
                        {
                            checkExpertFileChunk.Size = reachMaxTokenOfText.Length;
                            context.ExpertFileChunk.Update(checkExpertFileChunk);
                            await context.SaveChangesAsync();
                        }

                        string chunkembeddingFileName = Path
                            .Combine(convertPath, $"{expertFile.FileName}.{convertIndex}.EmbeddingJson");
                        if (!File.Exists(chunkembeddingFileName))
                        {
                            float[] embeddings = await adaEmbeddingVector.GetEmbeddingAsync(reachMaxTokenOfText);
                            if (embeddings.Length > 100)
                            {
                                string embeddingContext = JsonConvert.SerializeObject(embeddings);
                                await System.IO.File.WriteAllTextAsync(chunkembeddingFileName, embeddingContext);
                            }
                        }

                        #endregion

                        if (needSplitAgain == false) break;
                        convertIndex++;
                    }

                    expertFile.ChunkSize = convertIndex;
                    expertFile.ProcessChunk = true;
                    context.ExpertFile.Update(expertFile);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
