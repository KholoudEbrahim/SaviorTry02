using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MedicalStaffMember : BaseEntity
    {
        public string Name { get; set; }
        public MedicalRole Role { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public List<AvailabilityEntry> Availability { get; set; } = new();

        public bool ConfirmSummons()
        {
            // Logic for confirming summons (e.g., mark as assigned)
            return true;
        }
    }

}
