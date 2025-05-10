using Domain.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Emergency : BaseEntity
    {
        public int UserID { get; set; } 
        public string Location { get; set; }
        public TimeSpan Duration { get; set; }
        public EmergencyType Type { get; set; }
        public bool IsConfirmed { get; set; } = false;

        public bool ConfirmRequest()
        {
            IsConfirmed = true;
            return IsConfirmed;
        }
    }
}
