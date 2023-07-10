using BAL.Helpers;
using CommonDomain.DataModels;
using Domains.Models;
using EFCore.BulkExtensions;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GptLibrary.Services;

/// <summary>
/// ExpertDirectory Repository
/// </summary>
public class GptExpertDirectoryService
{
    private readonly BackendDBContext context;

    public GptExpertDirectoryService(BackendDBContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<List<ExpertDirectory>>> GetAsync()
    {
        var expertDirectories = await context.ExpertDirectory
            .AsNoTracking()
            .ToListAsync();
        return new ServiceResult<List<ExpertDirectory>>(expertDirectories);
    }

    public async Task<ServiceResult<ExpertDirectory>> GetAsync(string name)
    {
        var expertDirectory = await context.ExpertDirectory
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name);
        if (expertDirectory == null)
        {
            return new ServiceResult<ExpertDirectory>($"ExpertDirectory name : [{name}] not found.");
        }
        else
        {
            return new ServiceResult<ExpertDirectory>(expertDirectory);
        }
    }

    public async Task<ServiceResult<ExpertDirectory>> GetAsync(int id)
    {
        var expertDirectory = await context.ExpertDirectory
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (expertDirectory == null)
        {
            return new ServiceResult<ExpertDirectory>($"ExpertDirectory id : [{id}] not found.");
        }
        else
        {
            return new ServiceResult<ExpertDirectory>(expertDirectory);
        }
    }

    public async Task<ServiceResult<ExpertDirectory>> CreateAsync(ExpertDirectory expertDirectory)
    {
        var expertDirectoryExist = await context.ExpertDirectory.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == expertDirectory.Name);
        if (expertDirectoryExist != null)
        {
              return new ServiceResult<ExpertDirectory>($"ExpertDirectory name : [{expertDirectory.Name}] already exist.");
        }

        await context.ExpertDirectory.AddAsync(expertDirectory);
        await context.SaveChangesAsync();
        CleanTrackingHelper.Clean<ExpertDirectory>(context);
        return new ServiceResult<ExpertDirectory>(expertDirectory);
    }
}
