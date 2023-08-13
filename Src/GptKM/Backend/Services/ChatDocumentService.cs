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
using Backend.Models;

namespace Backend.Services
{
    public class ChatDocumentService
    {
        private readonly BackendDBContext context;

        public IMapper Mapper { get; }

        public ChatDocumentService(BackendDBContext context, IMapper mapper
            )
        {
            this.context = context;
            Mapper = mapper;
        }

        public async Task<FileProcessingInformation> Get()
        {
            FileProcessingInformation fileProcessingInformation = new FileProcessingInformation();

            fileProcessingInformation.合計傳檔案數量 = await context.ExpertFile
                .CountAsync();
            fileProcessingInformation.合計尚未處理檔案數量 = await context.ExpertFile
                .Where(x => x.ProcessingStatus != ExpertFileStatusEnum.Finish)
                .CountAsync();
            fileProcessingInformation.合計檔案大小 = await context.ExpertFile
                .SumAsync(x => x.Size);
            fileProcessingInformation.合計檔案區塊數量 = await context.ExpertFile
                .SumAsync(x => x.ChunkSize) ;
            fileProcessingInformation.合計Token大小 = await context.ExpertFile
                .SumAsync(x => x.TokenSize);

            return fileProcessingInformation;
        }
    }
}
