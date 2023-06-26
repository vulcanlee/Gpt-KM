using Domains.Models;
using EFCore.BulkExtensions;
using EntityModel.Entities;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GptLibrary.Services;

/// <summary>
/// 將實體檔案系統資訊，同步到資料庫中
/// </summary>
public class SyncDatabaseService
{
    private readonly GptExpertFileService gptExpertFileService;
    private readonly GptExpertDirectoryService gptExpertDirectoryService;

    public SyncDatabaseService(GptExpertFileService gptExpertFileService,
        GptExpertDirectoryService gptExpertDirectoryService)
    {
        this.gptExpertFileService = gptExpertFileService;
        this.gptExpertDirectoryService = gptExpertDirectoryService;
    }

    /// <summary>
    /// 將檔案資訊儲存到資料庫
    /// </summary>
    /// <param name="expertContent"></param>
    /// <returns></returns>
    public async Task<List<ExpertFile>> SaveAsync(ExpertContent expertContent)
    {
        var expertFilesNeedConvert = await ProcessSyncData(expertContent);
        return expertFilesNeedConvert;
    }

    /// <summary>
    /// 將掃描後的目錄內所有檔案，更新到資料庫內
    /// </summary>
    /// <param name="expertContent"></param>
    /// <param name="expertDirectory"></param>
    /// <returns></returns>
    private async Task<List<ExpertFile>> ProcessSyncData(ExpertContent expertContent)
    {
        List<ExpertRawFile> expertFilesNeedConvert = new List<ExpertRawFile>();
        List<ExpertFile> expertFiles = new();
        List<ExpertFile> expertSyncFiles = new();
        foreach (var itemFile in expertContent.ExpertFiles)
        {
            var expertDirectoryResult = await gptExpertDirectoryService
                .GetAsync(expertContent.SourceDirectory);
            if (expertDirectoryResult.Status == false) continue;
            var expertDirectory = expertDirectoryResult.Payload;
            if (expertDirectory != null)
            {
                var checkFileResult = await gptExpertFileService.GetAsync(itemFile.FullName);
                if (checkFileResult.Status == false)
                {
                    ExpertFile expertFile = new ExpertFile()
                    {
                        FullName = itemFile.FullName,
                        ExpertDirectoryId = expertDirectory.Id,
                        DirectoryName = itemFile.DirectoryName,
                        Extension = itemFile.Extension,
                        FileName = itemFile.FileName,
                        Size = itemFile.Size,
                        ChunkSize = 0,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now,
                        SyncAt = DateTime.Now,
                        ProcessingStatus = CommonDomain.Enums.ExpertFileStatusEnum.Begin,
                    };
                    expertFiles.Add(expertFile);
                    expertFilesNeedConvert.Add(itemFile);
                }
                else
                {
                    // 檔案已存在，更新同步時間資訊(用來判斷哪些檔案紀錄在資料庫內已經過時了)
                    var checkFile = checkFileResult.Payload;
                    checkFile.SyncAt = DateTime.Now;
                    expertSyncFiles.Add(checkFile);
                }
            }
        }

        await gptExpertFileService.CreateAsync(expertFiles);
        await gptExpertFileService.UpdateAsync(expertSyncFiles);

        #region 將 Chunk 區塊內容寫入到資料內
        foreach (var item in expertFiles)
        {
        }
        #endregion
        return expertFiles;
    }
}
