using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AvailabilityEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public string Day { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

        // Foreign Key for the parent entity (MedicalStaffMember)
        public int MedicalStaffMemberId { get; set; }
        public MedicalStaffMember MedicalStaffMember { get; set; }
    }
}
