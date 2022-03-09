using ElasticSearchDemo.Core.Interfaces.Service;
using ElasticSearchDemo.Infrastructure.Models;
using ElasticSearchDemo.Infrastructure.Models.DTO;
using ElasticSearchDemo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchDemo.API.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IElasticSearchService _elasticSearchService;
        public HomeController(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        [HttpPost("uploadfile")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequestDTO model)
        {
            try
            {
                var res = await _elasticSearchService.UploadFile(model.File,model.Type);
                return Ok(new Response<bool>
                {
                    Data = res,
                    Succeeded = true,
                    Message = res ? "Operation Successful" : "Operation Failed"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = ex.Message
                });
            }
            
        }


        [HttpPost("search")]
        public async Task<IActionResult> SearchPhase([FromForm] SearchRequestDTO model)
        {
            try
            {
                var searchresult = await _elasticSearchService.SearchFile(model.SearchPhase,model.Market);
                var res = Mapper.Map(searchresult.Item1, searchresult.Item2);
                return Ok(new Response<List<SearchResponseDTO>>
                {
                    Data = res,
                    Succeeded = true,
                    Message = "Operation Completed"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>
                {
                    Succeeded = false,
                    Message = ex.Message
                });
            }

        }

    }
}
