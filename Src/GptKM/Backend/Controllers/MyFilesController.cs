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

        #region 列出
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string result = string.Empty;
            var allFiles = await expertFileService.GetAllAsync();
            foreach (var item in allFiles)
            {
                result += $"{item.FileName}\r\n";
            }
            return Ok(result);
        }

        #endregion
    }
}
