using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface INotificationService
    {
        Task SendOrderConfirmationEmailAsync(int userId, int orderId);
    }

}








