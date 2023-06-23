﻿using Domains.Models;
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
        ExpertDirectory expertDirectoryEntity = await CheckDirectoryEntityAsync(expertContent);
        await CheckFileEntityAsync(expertContent, expertDirectoryEntity);
    }

    private async Task CheckFileEntityAsync(ExpertContent expertContent,
        ExpertDirectory expertDirectory)
    {
        List<ExpertFile> expertFiles = new();
        foreach (var itemFile in expertContent.ExpertFiles)
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
                };
                expertFiles.Add(expertFile);
            }
        }
        await context.BulkInsertAsync(expertFiles);
    }

    private async Task<ExpertDirectory> CheckDirectoryEntityAsync(ExpertContent expertContent)
    {
        ExpertDirectory expertDirectory = new ExpertDirectory()
        {
            Name = expertContent.SourceDirectory,
            SourcePath = expertContent.SourceDirectory,
            ConvertPath = expertContent.ConvertDirectory,
        };

        var findDirectory = context.ExpertDirectory
            .FirstOrDefault(x => x.SourcePath == expertDirectory.SourcePath);

        if (findDirectory == null)
        {
            await context.ExpertDirectory.AddAsync(expertDirectory);
            await context.SaveChangesAsync();
        }
        else
            expertDirectory = findDirectory;

        return expertDirectory;
    }
}
