using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.EmergencyDTOs
{
    public class EmergencyRequest
    {
        public int UserID { get; set; }
        public string Location { get; set; }
        public EmergencyType Type { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
