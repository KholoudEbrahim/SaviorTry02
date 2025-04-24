using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CartEntities
{
    public class Cart : BaseEntity
    {
   
        public int UserId { get; set; } 

        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
