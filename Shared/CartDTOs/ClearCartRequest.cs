using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CartDTOs
{
    public record ClearCartRequest
    {
        [Required]
        public int UserID { get; set; }

       
    }
}
