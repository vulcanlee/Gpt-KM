using CommonDomain.DataModels;
using Domains.Models;
using GptLibrary.Models;
using GptLibrary.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers;

public class SearchingHelper
{
    private readonly GptExpertFileService gptExpertFileService;
    private readonly GptExpertDirectoryService gptExpertDirectoryService;
    private readonly GptExpertFileChunkService gptExpertFileChunkService;
    private readonly ConvertFileModelService convertFileModelService;
    List<GptEmbeddingItem> allDocumentsEmbedding;

    public SearchingHelper(GptExpertFileService gptExpertFileService,
        GptExpertDirectoryService gptExpertDirectoryService,
        GptExpertFileChunkService gptExpertFileChunkService,
        ConvertFileModelService convertFileModelService)
    {
        this.gptExpertFileService = gptExpertFileService;
        this.gptExpertDirectoryService = gptExpertDirectoryService;
        this.gptExpertFileChunkService = gptExpertFileChunkService;
        this.convertFileModelService = convertFileModelService;
    }
    public async Task BuildEmbeddingDatabase()
    {
        string result = string.Empty;
        List<ExpertFile> allFiles = new List<ExpertFile>();
        allDocumentsEmbedding.Clear();
        #region 建立 Embedding DB

        #region 從資料庫內，取得所以已經產生 Embedding 紀錄的檔案清單
        ServiceResult<List<ExpertFile>> allFilesResult = await gptExpertFileService.GetAllEmbeddingAsync();
        if (allFilesResult != null && allFilesResult.Status == true)
        {
            allFiles = allFilesResult.Payload;
        }
        else { return; }
        #endregion

#if DEBUG
#endif
        try
        {
            foreach (var expertFileItem in allFiles)
            {
                foreach (var expertFileChunkItem in expertFileItem.ExpertFileChunk)
                {
                    string chunkembeddingContentFileName = expertFileChunkItem.EmbeddingTextFileName;
                    string chunkembeddingFileName = expertFileChunkItem.EmbeddingJsonFileName;
                    int convertIndex = expertFileChunkItem.ConvertIndex;
                    if (!File.Exists(chunkembeddingContentFileName)) { continue; }
                    if (!File.Exists(chunkembeddingFileName)) { continue; }

                    string embeddingContentContext = 
                        await File.ReadAllTextAsync(chunkembeddingContentFileName);
                    string embeddingContext = 
                        await File.ReadAllTextAsync(chunkembeddingFileName);
                    result = chunkembeddingFileName + Environment.NewLine;
                    var fileEmbedding = JsonConvert.DeserializeObject<List<float>>(embeddingContext);
                    float[] allValues = fileEmbedding.ToArray();
                    MathNet.Numerics.LinearAlgebra.Vector<float> theEmbedding;
                    theEmbedding = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(allValues);
                    GptEmbeddingItem embeddingItem = new GptEmbeddingItem()
                    {
                        ChunkIndex = convertIndex,
                        Embedding = theEmbedding,
                        FileName = expertFileChunkItem.FullName,
                        ChunkContent = embeddingContentContext,
                    };
                    allDocumentsEmbedding.Add(embeddingItem);
                }
            }
        }
        catch (Exception ex)
        {
            result += ex.ToString();
        }
        #endregion
        return ;
    }
}
