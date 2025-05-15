using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicalStaffDTOs
{
    public record MedicalRequestDto
    {
        public int UserId { get; set; }
        public int MedicalStaffMemberId { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Notes { get; set; }
    }
}
