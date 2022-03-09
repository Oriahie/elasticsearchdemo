using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Models
{
    public class Response<T>
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public T Data { get; set; }
    }
}
