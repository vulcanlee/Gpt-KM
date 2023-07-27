using AutoMapper;
using Backend.AdapterModels;
using Domains.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using BAL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Services.Interfaces;
using CommonDomain.DataModels;
using CommonDomain.Enums;

namespace Backend.Services
{
    public class RootFileUploadService : IRootFileUploadService
    {
        private readonly BackendDBContext context;
        private readonly OpenAIConfiguration openAIConfiguration;

        public IMapper Mapper { get; }
        public AuthenticationStateProvider AuthenticationStateProvider { get; }

        public RootFileUploadService(BackendDBContext context, IMapper mapper,
            AuthenticationStateProvider authenticationStateProvider,
            OpenAIConfiguration openAIConfiguration)
        {
            this.context = context;
            Mapper = mapper;
            AuthenticationStateProvider = authenticationStateProvider;
            this.openAIConfiguration = openAIConfiguration;
        }

        public async Task<ExpertDirectory> GetDefaultExpertDirectoryAsync(string name)
        {
            ExpertDirectory expertDirectory = null;
            expertDirectory = await context.ExpertDirectory
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            if (expertDirectory == null)
            {
                expertDirectory = new ExpertDirectory()
                {
                    Name = name,
                    ConvertPath = openAIConfiguration.DefaultConvertPath,
                    SourcePath = openAIConfiguration.DefaultSourcePath,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                };
                context.ExpertDirectory.Add(expertDirectory);
                await context.SaveChangesAsync();
            }
            return expertDirectory;
        }

        public async Task<ExpertFile> GetExpertFileAndResetStatusAsync(string name, ExpertDirectory expertDirectory)
        {
            ExpertFile expertFile = null;
            expertFile = await context.ExpertFile
                .Include(x => x.ExpertDirectory)
                .Include(x => x.ExpertFileChunk)
                .FirstOrDefaultAsync(x => x.FullName == name);
            if (expertFile != null)
            {
                expertFile.ProcessingStatus = ExpertFileStatusEnum.Begin;
                expertFile.UpdateAt = DateTime.Now;
                context.ExpertFile.Update(expertFile);
                await context.SaveChangesAsync();

                #region Todo 將原有的 Chunk Embedding 資料清除

                #endregion
            }
            else
            {
                #region 建立一筆新紀錄
                FileInfo fileInfo = new FileInfo(name);
                ExpertFile expertFileNew = new ExpertFile()
                {
                    ExpertDirectoryId = expertDirectory.Id,
                    FullName = name,
                    ProcessingStatus = ExpertFileStatusEnum.Begin,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    DirectoryName = Path.GetFullPath(expertDirectory.SourcePath),
                    Extension = Path.GetExtension(name),
                    FileName = Path.GetFileName(name),
                    Size = fileInfo.Length,
                };
                context.ExpertFile.Add(expertFileNew);
                await context.SaveChangesAsync();
                #endregion
            }
            return expertFile;
        }
    }
}
