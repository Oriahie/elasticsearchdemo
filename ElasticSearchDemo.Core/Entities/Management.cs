using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Core.Entities
{

    public class Management
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RecordType Type { get; set; } = RecordType.MANAGEMENT;
        public ManagementPayload mgmt { get; set; }
    }

    
    public class ManagementPayload
    {
        public int mgmtID { get; set; }
        
        public string name { get; set; }
        public string market { get; set; }
        public string state { get; set; }

    }

}
