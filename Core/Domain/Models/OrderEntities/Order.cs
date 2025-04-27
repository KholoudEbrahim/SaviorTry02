using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.OrderEntities
{

    public class Order : BaseEntity
    {
        public int UserID { get; set; } //FK

        [Required]
        public double UserLatitude { get; set; }
        [Required]
        public double UserLongitude { get; set; }

        public TimeSpan? Duration { get; set; }
        public decimal ShippingPrice { get; set; } 
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public PaymentForOrder PaymentWay { get; set; }
        public string? DeliveryName { get; set; }
        public string? DeliveryPhone { get; set; }
        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
