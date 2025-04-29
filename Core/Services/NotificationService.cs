using Domain.Contracts;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            EmailService emailService,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmailAsync(int userId, int orderId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

                if (user == null || order == null)
                {
                    _logger.LogWarning($"User {userId} or Order {orderId} not found");
                    return;
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    _logger.LogWarning($"User {userId} has no email address");
                    return;
                }

                await _emailService.SendOrderConfirmationEmailAsync(
                    user.Email,
                    user.Fname,
                    order.Id,
                    order.TotalPrice);

                _logger.LogInformation($"Confirmation email sent for order {orderId} to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending confirmation email for order {orderId}");
                throw;
            }
        }
    }
}
