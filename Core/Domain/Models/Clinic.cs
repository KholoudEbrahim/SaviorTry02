using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
     public class Clinic
    {
        [Key]
        public int ClinicID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty; 
        public string City { get; set; }
        public decimal BookingPrice { get; set; }


        [Required]
        public string Phone { get; set; } = string.Empty; 
    }
}
