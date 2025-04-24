using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Supplies
    {
        public int PharmacyId { get; set; }
        public int MedicineId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation properties
        public virtual Pharmacy Pharmacy { get; set; } = null!;
        public virtual Medicine Medicine { get; set; } = null!;

    }
}
