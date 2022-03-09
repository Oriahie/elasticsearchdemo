using ElasticSearchDemo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Models.DTO
{
    public class SearchResponseDTO
    {
        public RecordType Type { get; set; }
        public int MgmtID { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
        public int PropertyID { get; set; }
        public string FormerName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
