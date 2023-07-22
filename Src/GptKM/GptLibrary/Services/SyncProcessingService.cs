using CommonDomain.DataModels;
using Domains.Models;
using GptLibrary.Helpers;
using GptLibrary.Models;

namespace GptLibrary.Services;

/// <summary>
/// 針對單一目錄對應要進行的同步工作
/// </summary>
public class SyncProcessingService
{
    private readonly SyncDirectoryService syncDirectoryService;
    private readonly SyncFilesToDatabaseService syncFilesToDatabase;
    private readonly ConvertToTextService convertToTextService;
    private readonly ConvertToEmbeddingService convertToEmbeddingService;
    private readonly GptExpertFileService gptExpertFileService;
    private readonly EmbeddingSearchHelper embeddingSearchHelper;

    public SyncProcessingService(SyncDirectoryService syncDirectoryService,
        SyncFilesToDatabaseService syncDatabaseService,
        ConvertToTextService convertToTextService,
        ConvertToEmbeddingService convertToEmbeddingService,
        GptExpertFileService gptExpertFileService,
        EmbeddingSearchHelper embeddingSearchHelper)
    {
        this.syncDirectoryService = syncDirectoryService;
        this.syncFilesToDatabase = syncDatabaseService;
        this.convertToTextService = convertToTextService;
        this.convertToEmbeddingService = convertToEmbeddingService;
        this.gptExpertFileService = gptExpertFileService;
        this.embeddingSearchHelper = embeddingSearchHelper;
    }

    public async Task BeginSyncDirectoryAsync(ExpertDirectory expertDirectory)
    {
        ExpertContent expertContent = null;
        if (expertDirectory == null) return;

        #region (開發測試用) 清空現在檔案內容
        //var expertFilesReslut = await gptExpertFileService.GetAsync();
        //if (expertFilesReslut.Status == true)
        //{
        //    var expertFiles = expertFilesReslut.Payload;
        //    foreach (var item in expertFiles)
        //    {
        //        await gptExpertFileService.DeleteAsync(item.Id);
        //    }
        //}
        #endregion

        #region 檢查目錄與取得可用的檔案清單與建立轉換後的目錄結構
        expertContent = await syncDirectoryService.ScanSourceDirectory(expertDirectory);
        #endregion

        #region 將實體檔案系統資訊，同步到資料庫中
        if (expertContent == null) return;
        var expertFilesCollectionNeedConvert = await syncFilesToDatabase.SaveAsync(expertContent);
        #endregion

        #region 將檔案內容轉換成為文字檔案
        if (expertFilesCollectionNeedConvert == null) return;
        foreach (var item in expertFilesCollectionNeedConvert)
        {
            ConvertFileModel convertFileModel =
                await convertToTextService.ConvertAsync(item);

            if(convertFileModel == null) continue;

            #region 將文字內容與切割後的文字Chunk，寫入到檔案內
            foreach (var itemChunk in convertFileModel.ConvertFileSplitItems)
            {
                await convertToEmbeddingService
                    .ConvertAsync(item, convertFileModel, itemChunk.Index);
            }
            #endregion

            await gptExpertFileService.ChangeStatusAsync(item.FullName, CommonDomain.Enums.ExpertFileStatusEnum.Finish);

            #region 產生 EmbeddingSearchHelper 集合物件
            ServiceResult<ExpertFile> expertFileResult = await gptExpertFileService.GetAsync(item.FullName);
            if (expertFileResult.Status == true)
            {
                ExpertFile expertFile = expertFileResult.Payload;
                await embeddingSearchHelper.AddAsync(expertFile);
            }
            #endregion
        }
        #endregion

        int foo = 0;
    }
}
