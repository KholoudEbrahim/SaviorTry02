using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicalStaffDTOs
{
    public record AvailabilityEntryDto
    {
        public string Day { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
