using Domains.Models;
using EFCore.BulkExtensions;
using EntityModel.Entities;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GptLibrary.Services;

/// <summary>
/// 將實體檔案系統資訊，同步到資料庫中
/// </summary>
public class SyncDatabase
{
    private readonly BackendDBContext context;

    public SyncDatabase(BackendDBContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// 將檔案資訊儲存到資料庫
    /// </summary>
    /// <param name="expertContent"></param>
    /// <returns></returns>
    public async Task SaveAsync(ExpertContent expertContent)
    {
        await ProcessSyncData(expertContent);
    }

    /// <summary>
    /// 將掃描後的目錄內所有檔案，更新到資料庫內
    /// </summary>
    /// <param name="expertContent"></param>
    /// <param name="expertDirectory"></param>
    /// <returns></returns>
    private async Task ProcessSyncData(ExpertContent expertContent)
    {

        List<ExpertFile> expertFiles = new();
        List<ExpertFile> expertSyncFiles = new();
        foreach (var itemFile in expertContent.ExpertFiles)
        {
            var expertDirectory = context.ExpertDirectory
                .FirstOrDefault(x => x.SourcePath == expertContent.SourceDirectory);
            if (expertDirectory != null)
            {
                var checkFile = await context.ExpertFile.FirstOrDefaultAsync(x => x.FullName == itemFile.FullName);
                if (checkFile == null)
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
                }
                else
                {
                    // 檔案已存在，更新同步時間資訊(用來判斷哪些檔案紀錄在資料庫內已經過時了)
                    checkFile.SyncAt = DateTime.Now;
                    expertSyncFiles.Add(checkFile);
                }
            }
        }
        await context.BulkInsertAsync(expertFiles);
        await context.BulkUpdateAsync(expertSyncFiles);
    }
}
