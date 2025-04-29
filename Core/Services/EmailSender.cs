using Microsoft.Extensions.Configuration;
using ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
 using System;
 using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Services
{

    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Savior Team", "mzarad905@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("kholoudebrahim419@gmail.com", "xmac nayd agak xunf");
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendOrderConfirmationEmailAsync(string userEmail, string userName, int orderId, decimal totalAmount)
        {
            var subject = "Order Confirmation - Your Savior";
            var body = $@"
         Thank you for your order, {userName}!

             Your order #{orderId} has been confirmed.

              Order Details:
            - Order ID: {orderId}
              - Total Amount: {totalAmount:C}

             Thank you for choosing us!
                Your Savior Team";

            await SendEmailAsync(userEmail, subject, body);
        }
    }
}
