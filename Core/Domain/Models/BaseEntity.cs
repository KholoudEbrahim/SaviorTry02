using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public abstract class BaseEntity<T> 
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
