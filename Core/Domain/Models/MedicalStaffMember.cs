using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MedicalStaffMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public MedicalRole Role { get; set; }
        [Required]
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }


        public virtual ICollection<AvailabilityEntry> Availability { get; set; } = new List<AvailabilityEntry>();
        public bool ConfirmSummons()
        {
            // Logic for confirming summons (e.g., mark as assigned)
            return true;
        }
    }

}
