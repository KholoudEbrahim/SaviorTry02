using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MedicalStaffDTOs
{
    public record MedicalStaffMemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MedicalRole Role { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }

        public List<AvailabilityEntryDto> Availability { get; set; }
    }
}
