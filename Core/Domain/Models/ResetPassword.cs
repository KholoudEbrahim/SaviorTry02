using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ResetPassword
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
