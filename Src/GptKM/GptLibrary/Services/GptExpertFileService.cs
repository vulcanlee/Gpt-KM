using BAL.Helpers;
using CommonDomain.DataModels;
using CommonDomain.Enums;
using Domains.Models;
using EFCore.BulkExtensions;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GptLibrary.Services;

/// <summary>
/// ExpertFile Repository
/// </summary>
public class GptExpertFileService
{
    private readonly BackendDBContext context;

    public GptExpertFileService(BackendDBContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<List<ExpertFile>>> GetAsync()
    {
        var expertFiles = await context.ExpertFile
            .AsNoTracking()
            .Include(x => x.ExpertDirectory).ToListAsync();
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<List<ExpertFile>>> GetAllEmbeddingAsync()
    {
        var expertFiles = await context.ExpertFile
            .AsNoTracking()
            .Include(x => x.ExpertDirectory)
            .Include(x=>x.ExpertFileChunk)
            .Where(x=>x.ProcessingStatus == ExpertFileStatusEnum.Finish)
            .ToListAsync();
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<List<ExpertFile>>> GetAsync(ExpertDirectory expertDirectory)
    {
        var expertFiles = await context.ExpertFile
            .AsNoTracking()
            .Include(x => x.ExpertDirectory)
            .Where(x => x.ExpertDirectoryId == expertDirectory.Id).ToListAsync();
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<ExpertFile>> GetAsync(int id)
    {
        var expertFile = await context.ExpertFile
            .AsNoTracking()
            .Include(x => x.ExpertDirectory)
            .Include(x=>x.ExpertFileChunk)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (expertFile == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{id}] not found.");
        }
        else
        {
            return new ServiceResult<ExpertFile>(expertFile);
        }
    }

    public async Task<ServiceResult<ExpertFile>> GetAsync(string filename)
    {
        var expertFile = await context.ExpertFile
            .AsNoTracking()
            .Include(x => x.ExpertDirectory)
            .Include(x=>x.ExpertFileChunk)
            .FirstOrDefaultAsync(x => x.FullName == filename);
        if (expertFile == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile filename : [{filename}] not found.");
        }
        else
        {
            return new ServiceResult<ExpertFile>(expertFile);
        }
    }

    public async Task<ServiceResult<ExpertFile>> ChangeStatusAsync(string filename, ExpertFileStatusEnum expertFileStatusEnum)
    {
        CleanTrackingHelper.Clean<ExpertDirectory>(context);
        CleanTrackingHelper.Clean<ExpertFile>(context);
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        var expertFile = await context.ExpertFile.AsNoTracking()
            .Include(x => x.ExpertDirectory)
            .FirstOrDefaultAsync(x => x.FullName == filename);
        if (expertFile == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile filename : [{filename}] not found.");
        }
        else
        {
            expertFile.ProcessingStatus = expertFileStatusEnum;
            context.ExpertFile.Update(expertFile);
            var foo = await context.SaveChangesAsync();
            return new ServiceResult<ExpertFile>(expertFile);
        }
    }

    public async Task<ServiceResult<ExpertFile>> CreateAsync(ExpertFile ExpertFile)
    {
        var ExpertFileExist = await context.ExpertFile
            .FirstOrDefaultAsync(x => x.FullName == ExpertFile.FullName);
        if (ExpertFileExist != null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile name : [{ExpertFile.FullName}] already exist.");
        }

        await context.ExpertFile.AddAsync(ExpertFile);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFile>(context);
        return new ServiceResult<ExpertFile>(ExpertFile);
    }

    /// <summary>
    /// 使用 BulkInsertAsync 來產生紀錄
    /// </summary>
    /// <param name="expertFiles"></param>
    /// <returns></returns>
    public async Task<ServiceResult<List<ExpertFile>>> CreateAsync(List<ExpertFile> expertFiles)
    {
        await context.BulkInsertAsync(expertFiles);
        CleanTrackingHelper.Clean<ExpertFile>(context);
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<ExpertFile>> UpdateAsync(ExpertFile ExpertFile)
    {
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        CleanTrackingHelper.Clean<ExpertFile>(context);
        CleanTrackingHelper.Clean<ExpertDirectory>(context);
        var ExpertFileExist = await context.ExpertFile
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == ExpertFile.Id);
        if (ExpertFileExist == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{ExpertFile.Id}] not found.");
        }

        context.ExpertFile.Update(ExpertFile);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        CleanTrackingHelper.Clean<ExpertFile>(context);
        CleanTrackingHelper.Clean<ExpertDirectory>(context);
        return new ServiceResult<ExpertFile>(ExpertFile);
    }

    public async Task<ServiceResult<List<ExpertFile>>> UpdateAsync(List<ExpertFile> expertFiles)
    {
        await context.BulkUpdateAsync(expertFiles);
        CleanTrackingHelper.Clean<ExpertFile>(context);
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<ExpertFile>> DeleteAsync(int id)
    {
        var ExpertFileExist = await context.ExpertFile.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (ExpertFileExist == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{id}] not found.");
        }

        context.ExpertFile.Remove(ExpertFileExist);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFile>(context);
        return new ServiceResult<ExpertFile>(ExpertFileExist);
    }
}
