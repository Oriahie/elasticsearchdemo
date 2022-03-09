using ElasticSearchDemo.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Core.Interfaces.Service
{
    public interface IElasticSearchService
    {
        //upload file
        Task<bool> UploadFile(IFormFile file, RecordType Type);
        Task<(List<Management>, List<Property>)> SearchFile(string searchParam, List<string> market, int pageSize = 25);
        
    }
}
