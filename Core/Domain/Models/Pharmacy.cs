﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Pharmacy : BaseEntity<Guid> 
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        public int BuildingNumber { get; set; }

        [Required]
        public bool HasDelivery { get; set; }

        // Navigation properties
      //  public virtual ICollection<Supplies> Supplies { get; set; } = new List<Supplies>();
        //public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
