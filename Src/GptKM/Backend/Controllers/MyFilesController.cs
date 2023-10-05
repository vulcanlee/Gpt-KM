using AutoMapper;
using Backend.AdapterModels;
using Backend.Services;
using DTOs.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BAL.Factories;
using BAL.Helpers;
using CommonDomain.DataModels;
using CommonDomain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Services.Interfaces;
using System;

namespace Backend.Controllers
{
    [Produces("text/plain")]
    [Route("api/[controller]")]
    [ApiController]
    public class MyFilesController : ControllerBase
    {
        private readonly IExpertFileService expertFileService;
        private readonly IMapper mapper;

        #region 建構式
        public MyFilesController(IExpertFileService expertFileService,
            IMapper mapper)
        {
            this.expertFileService = expertFileService;
            this.mapper = mapper;
        }
        #endregion

        #region 列出與下載
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string result = string.Empty;
            var allFiles = await expertFileService.GetAllAsync();
            allFiles = allFiles.OrderBy(allFiles => allFiles.FileName).ToList();
            //var baseUri = $"{Request.Scheme}://{Request.Host}:{Request.Host.Port ?? 80}";
            var baseUri = $"{Request.Scheme}://{Request.Host}";
            foreach (var item in allFiles)
            {
                int id = item.Id;
                string url = $"{baseUri}/api/MyFiles/{id}";
                result += $"[{item.FileName}]({url}){Environment.NewLine}{Environment.NewLine}";
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var file = await expertFileService.GetAsync(id);
            var filepath = file.FullName;
            return File(await System.IO.File.ReadAllBytesAsync(filepath), 
                "application/octet-stream", System.IO.Path.GetFileName(filepath));
        }

        #endregion
    }
}
