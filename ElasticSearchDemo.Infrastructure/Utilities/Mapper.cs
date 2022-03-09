using ElasticSearchDemo.Core.Entities;
using ElasticSearchDemo.Infrastructure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchDemo.Infrastructure.Utilities
{
    public static class Mapper
    {

        public static SearchResponseDTO Map(this Management management)
        {
            if (management == null) return null;

            return new SearchResponseDTO
            {
                Type = management.Type,
                MgmtID = management.mgmt.mgmtID,
                Name = management.mgmt.name,
                Market = management.mgmt.market,
                State = management.mgmt.state
            };
        }

        public static SearchResponseDTO Map(this Property model)
        {
            if (model == null) return null;
            return new SearchResponseDTO
            {
                Type = model.Type,
                PropertyID = model.property.propertyID,
                StreetAddress = model.property.streetAddress,
                Name = model.property.name,
                FormerName = model.property.formerName,
                City = model.property.city,
                Market = model.property.market,
                State = model.property.state,
                Lat = model.property.lat,
                Lng = model.property.lng
            };
        }

        public static List<SearchResponseDTO> Map(List<Management> management, List<Property> property)
        {
            var res = new List<SearchResponseDTO>();
            res.AddRange(property.Select(x => x.Map()).ToList());
            res.AddRange(management.Select(x => x.Map()).ToList());
            return res;
        }
    }
}
