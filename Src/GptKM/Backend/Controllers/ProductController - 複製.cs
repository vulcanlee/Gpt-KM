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

namespace Backend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SampleAutoMapperController : ControllerBase
    {
        #region 建構式
        public SampleAutoMapperController()
        {
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            APIResult apiResult;

            List<ProductDto> products = new();
            for (int i = 0; i < 100; i++)
            {
                var product = new ProductDto()
                {
                    Id = i,
                    Name = i.ToString(),
                    ListPrice = i,
                };
                products.Add(product);
            }
            apiResult = APIResultFactory.Build(true, StatusCodes.Status200OK,
                ErrorMessageEnum.None, payload: products);
            return Ok(apiResult);
        }
    }
}
