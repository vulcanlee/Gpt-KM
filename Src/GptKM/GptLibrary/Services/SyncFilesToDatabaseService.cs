using Domains.Models;
using GptLibrary.Models;
using Microsoft.Extensions.Logging;

namespace GptLibrary.Services;

/// <summary>
/// 將實體檔案系統資訊，同步到資料庫中
/// </summary>
public class SyncFilesToDatabaseService
{
    private readonly GptExpertFileService gptExpertFileService;
    private readonly GptExpertDirectoryService gptExpertDirectoryService;
    private readonly ILogger<SyncFilesToDatabaseService> logger;

    public SyncFilesToDatabaseService(GptExpertFileService gptExpertFileService,
        GptExpertDirectoryService gptExpertDirectoryService,
        ILogger<SyncFilesToDatabaseService> logger)
    {
        this.gptExpertFileService = gptExpertFileService;
        this.gptExpertDirectoryService = gptExpertDirectoryService;
        this.logger = logger;
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
        List<ExpertFile> expertAddFiles = new();
        List<ExpertFile> expertSyncFiles = new();

        var expertDirectoryResult = await gptExpertDirectoryService
            .GetAsync(expertContent.Name);
        if (expertDirectoryResult.Status == false) return expertFiles;
        // 需要該目錄物件內的 Id 屬性值
        var expertDirectory = expertDirectoryResult.Payload;

        foreach (var itemFile in expertContent.ExpertRawFiles)
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

                await Console.Out.WriteLineAsync($"Found File : {expertFile.FileName}");
                expertFiles.Add(expertFile);
                expertAddFiles.Add(expertFile);
                expertFilesNeedConvert.Add(itemFile);
            }
            else
            {
                // 檔案已存在，更新同步時間資訊(用來判斷哪些檔案紀錄在資料庫內已經過時了)
                var checkFile = checkFileResult.Payload;
                checkFile.SyncAt = DateTime.Now;
                if (checkFile.ProcessingStatus != CommonDomain.Enums.ExpertFileStatusEnum.Finish)
                {
                    expertFiles.Add(checkFile);
                }
                expertSyncFiles.Add(checkFile);
            }
        }

        await gptExpertFileService.CreateAsync(expertAddFiles);
        await gptExpertFileService.UpdateAsync(expertSyncFiles);

        return expertFiles;
    }
}
