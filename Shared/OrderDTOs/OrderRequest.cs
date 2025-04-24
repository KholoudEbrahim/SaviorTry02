using Domain.Models.Enumerations;
using Domain.Models.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OrderDTOs
{

        public class OrderRequest
        {
            public int UserID { get; set; }
            public double UserLatitude { get; set; }
            public double UserLongitude { get; set; }
            public PaymentForOrder PaymentWay { get; set; }
            public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        }
    
}
