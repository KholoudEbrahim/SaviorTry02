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
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
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

                var subject = "Order Confirmation - Your Savior";
                var htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; padding: 20px;'>
                <h2 style='color: #4a90e2;'>Thank you for your order, {user.Fname}!</h2>
                <p>Your order <strong>#{order.Id}</strong> has been confirmed.</p>
                
                <h3 style='color: #4a90e2;'>Order Details</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'><strong>Order Date:</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'>{order.OrderDate.ToString("dd MMM yyyy HH:mm")}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'><strong>Estimated Delivery:</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'>{order.Duration?.TotalMinutes} minutes</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'><strong>Total Amount:</strong></td>
                        <td style='padding: 8px; border-bottom: 1px solid #e0e0e0;'>{order.TotalPrice:C}</td>
                    </tr>
                </table>
                
                <p style='margin-top: 20px;'>Thank you for choosing us!</p>
                
                <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #777;'>
                    <p>Your Savior Team</p>
                </div>
            </div>";

                await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);
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
