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

        public async Task<ExpertFile> GetExpertFileAsync(string name, ExpertDirectory expertDirectory)
        {
            ExpertFile expertFile = null;
            expertFile = await context.ExpertFile
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
            return expertFile;
        }
    }
}
