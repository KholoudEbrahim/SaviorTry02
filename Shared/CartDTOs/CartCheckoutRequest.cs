using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CartDTOs
{
    public class CartCheckoutRequest
    {
        public int UserID { get; set; }

        [Range(-90, 90)]
        public double UserLatitude { get; set; }

        [Range(-180, 180)]
        public double UserLongitude { get; set; }

        public string UserPhone { get; set; }
    }
}