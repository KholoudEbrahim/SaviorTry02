using Domain.Models.OrderEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Fname { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Lname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;


        [Required]
        public string Password { get; set; } = string.Empty;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
