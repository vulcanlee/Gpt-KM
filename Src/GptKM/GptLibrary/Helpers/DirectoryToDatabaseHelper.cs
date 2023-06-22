using Domains.Models;
using EFCore.BulkExtensions;
using EntityModel.Entities;
using GptLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptLibrary.Helpers
{
    public class DirectoryToDatabaseHelper
    {
        private readonly BackendDBContext context;

        public DirectoryToDatabaseHelper(BackendDBContext context)
        {
            this.context = context;
        }
        public async Task SaveAsync(ExpertContent expertContent)
        {
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
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
                Name = expertContent.TargetDirectory,
                Path = expertContent.TargetDirectory,
                ConvertPath = expertContent.ConvertDirectory,
            };

            var findDirectory = context.ExpertDirectory.FirstOrDefault(x => x.Path == expertDirectory.Path);
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
}
