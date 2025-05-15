using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MedicalRequest : BaseEntity
    {
        public int UserId { get; set; } 
        public int MedicalStaffMemberId { get; set; } 
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public bool IsConfirmed { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual MedicalStaffMember MedicalStaffMember { get; set; } = null!;
    }
}
