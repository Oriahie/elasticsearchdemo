using ElasticSearchDemo.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Models.DTO
{
    public class UploadFileRequestDTO
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public RecordType Type { get; set; }
    }
}
