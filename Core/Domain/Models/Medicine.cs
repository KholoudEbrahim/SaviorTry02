using Domain.Models.OrderEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Models
{
    public class Medicine : BaseEntity
    {


        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string Image { get; set; } = string.Empty;


        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? StripPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BoxPrice { get; set; }

        // Navigation properties
        public virtual ICollection<Supplies> Supplies { get; set; } = new List<Supplies>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
