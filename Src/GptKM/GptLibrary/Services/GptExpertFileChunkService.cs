using BAL.Helpers;
using CommonDomain.DataModels;
using Domains.Models;
using EFCore.BulkExtensions;
using EntityModel.Entities;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GptLibrary.Services;

/// <summary>
/// ExpertFileChunk Repository
/// </summary>
public class GptExpertFileChunkService
{
    private readonly BackendDBContext context;

    public GptExpertFileChunkService(BackendDBContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<List<ExpertFileChunk>>> GetAsync()
    {
        var expertFileChunks = await context.ExpertFileChunk
            .AsNoTracking().ToListAsync();
        return new ServiceResult<List<ExpertFileChunk>>(expertFileChunks);
    }

    public async Task<ServiceResult<List<ExpertFileChunk>>> GetAsync(ExpertFile expertFile)
    {
        var expertFileChunks = await context.ExpertFileChunk.AsNoTracking()
            .Where(x => x.ExpertFileId == expertFile.Id).ToListAsync();
            return new ServiceResult<List<ExpertFileChunk>>(expertFileChunks);
    }

    public async Task<ServiceResult<ExpertFileChunk>> GetAsync(int id)
    {
        var expertFileChunk = await context.ExpertFileChunk
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (expertFileChunk == null)
        {
            return new ServiceResult<ExpertFileChunk>($"ExpertFileChunk id : [{id}] not found.");
        }
        else
        {
            return new ServiceResult<ExpertFileChunk>(expertFileChunk);
        }
    }

    public async Task<ServiceResult<ExpertFileChunk>> CreateAsync(ExpertFileChunk ExpertFileChunk)
    {
        var ExpertFileChunkExist = await context.ExpertFileChunk.AsNoTracking()
            .FirstOrDefaultAsync(x => x.FullName == ExpertFileChunk.FullName);
        if (ExpertFileChunkExist != null)
        {
              return new ServiceResult<ExpertFileChunk>($"ExpertFileChunk name : [{ExpertFileChunk.FullName}] already exist.");
        }

        await context.ExpertFileChunk.AddAsync(ExpertFileChunk);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        return new ServiceResult<ExpertFileChunk>(ExpertFileChunk);
    }

    public async Task<ServiceResult<ExpertFileChunk>> UpdateAsync(ExpertFileChunk ExpertFileChunk)
    {
        var ExpertFileChunkExist = await context.ExpertFileChunk
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == ExpertFileChunk.Id);
        if (ExpertFileChunkExist == null)
        {
            return new ServiceResult<ExpertFileChunk>($"ExpertFileChunk id : [{ExpertFileChunk.Id}] not found.");
        }

        context.Entry(ExpertFileChunkExist).CurrentValues.SetValues(ExpertFileChunk);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        return new ServiceResult<ExpertFileChunk>(ExpertFileChunk);
    }

    public async Task<ServiceResult<ExpertFileChunk>> DeleteAsync(int id)
    {
        var ExpertFileChunkExist = await context.ExpertFileChunk
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (ExpertFileChunkExist == null)
        {
            return new ServiceResult<ExpertFileChunk>($"ExpertFileChunk id : [{id}] not found.");
        }

        context.ExpertFileChunk.Remove(ExpertFileChunkExist);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertFileChunk>(context);
        return new ServiceResult<ExpertFileChunk>(ExpertFileChunkExist);
    }
}
