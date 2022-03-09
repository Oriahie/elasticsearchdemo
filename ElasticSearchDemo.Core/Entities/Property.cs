using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Core.Entities
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RecordType Type { get; set; } = RecordType.PROPERTY;
        public PropertyPayload property { get; set; }
    }

    public class PropertyPayload
    {
        public int propertyID { get; set; }
        public string name { get; set; }
        public string formerName { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string market { get; set; }
        public string state { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
