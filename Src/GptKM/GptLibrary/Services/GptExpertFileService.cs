﻿using CommonDomain.DataModels;
using Domains.Models;
using EFCore.BulkExtensions;
using EntityModel.Entities;
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
        var expertFiles = await context.ExpertFile.ToListAsync();
        return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<List<ExpertFile>>> GetAsync(ExpertDirectory expertDirectory)
    {
        var expertFiles = await context.ExpertFile
            .Where(x => x.ExpertDirectoryId == expertDirectory.Id).ToListAsync();
            return new ServiceResult<List<ExpertFile>>(expertFiles);
    }

    public async Task<ServiceResult<ExpertFile>> GetAsync(int id)
    {
        var expertFile = await context.ExpertFile.FirstOrDefaultAsync(x => x.Id == id);
        if (expertFile == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{id}] not found.");
        }
        else
        {
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
        return new ServiceResult<ExpertFile>(ExpertFile);
    }

    public async Task<ServiceResult<ExpertFile>> UpdateAsync(ExpertFile ExpertFile)
    {
        var ExpertFileExist = await context.ExpertFile
            .FirstOrDefaultAsync(x => x.Id == ExpertFile.Id);
        if (ExpertFileExist == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{ExpertFile.Id}] not found.");
        }

        context.Entry(ExpertFileExist).CurrentValues.SetValues(ExpertFile);
        await context.SaveChangesAsync();
        return new ServiceResult<ExpertFile>(ExpertFile);
    }

    public async Task<ServiceResult<ExpertFile>> DeleteAsync(int id)
    {
        var ExpertFileExist = await context.ExpertFile
            .FirstOrDefaultAsync(x => x.Id == id);
        if (ExpertFileExist == null)
        {
            return new ServiceResult<ExpertFile>($"ExpertFile id : [{id}] not found.");
        }

        context.ExpertFile.Remove(ExpertFileExist);
        await context.SaveChangesAsync();
        return new ServiceResult<ExpertFile>(ExpertFileExist);
    }
}