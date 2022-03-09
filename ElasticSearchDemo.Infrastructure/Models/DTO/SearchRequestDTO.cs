using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Models.DTO
{
    public class SearchRequestDTO
    {
        [Required]
        public string SearchPhase { get; set; }
        public List<string> Market { get; set; } = new List<string>();
    }
}
