using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.OrderEntities
{
    public class DeliveryPerson: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    
    }
}
