using EntityModel.Entities;
using GptLibrary.Models;

namespace GptLibrary.Services;

/// <summary>
/// 針對單一目錄對應要進行的同步工作
/// </summary>
public class SyncProcessingService
{
    private readonly SyncDirectoryService syncDirectoryService;
    private readonly SyncDatabaseService syncDatabaseService;
    private readonly ConvertToTextService convertToTextService;
    private readonly ConvertToEmbeddingService convertToEmbeddingService;

    public SyncProcessingService(SyncDirectoryService syncDirectoryService,
        SyncDatabaseService syncDatabaseService,
        ConvertToTextService convertToTextService,
        ConvertToEmbeddingService convertToEmbeddingService)
    {
        this.syncDirectoryService = syncDirectoryService;
        this.syncDatabaseService = syncDatabaseService;
        this.convertToTextService = convertToTextService;
        this.convertToEmbeddingService = convertToEmbeddingService;
    }

    public async Task BeginSyncDirectoryAsync(ExpertDirectory expertDirectory)
    {
        ExpertContent expertContent = null;
        if (expertDirectory == null) return;

        #region 檢查目錄與取得可用的檔案清單與建立轉換後的目錄結構
        var task = Task.Run(() =>
        {
            expertContent = syncDirectoryService.ScanSourceDirectory(expertDirectory);
        });
        await task;
        #endregion

        #region 將實體檔案系統資訊，同步到資料庫中
        if (expertContent == null) return;
        var expertFilesNeedConvert = await syncDatabaseService.SaveAsync(expertContent);
        #endregion

        #region 將檔案內容轉換成為文字檔案
        if (expertFilesNeedConvert == null) return;
        foreach (var item in expertFilesNeedConvert)
        {
            ConvertFileModel convertFileModels =
                await convertToTextService.ConvertAsync(item);
            #region 將文字內容與切割後的文字Chunk，寫入到檔案內
            foreach (var itemChunk in convertFileModels.ConvertFileSplitItems)
            {
                await convertToEmbeddingService
                    .ConvertAsync(item, convertFileModels, itemChunk.Index);
            }
            #endregion
        }
        #endregion


        #region 將 Chunk 文字內容轉換成為 Embedding 向量
        #endregion
    }
}
