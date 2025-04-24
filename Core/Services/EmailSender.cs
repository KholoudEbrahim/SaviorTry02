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

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {

            var testMode = bool.Parse(_configuration["EmailSettings:TestMode"]);
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

            var fromName = _configuration["EmailSettings:FromName"];
            var fromAddress = _configuration["EmailSettings:FromAddress"];
            var testEmail = _configuration["EmailSettings:TestEmail"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));

    
            var recipient = testMode ? testEmail : toEmail;

            message.To.Add(new MailboxAddress("Customer", recipient));
            message.Subject = subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                        _configuration["EmailSettings:SmtpServer"],
                        smtpPort,
                        enableSsl);
                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:SmtpUsername"],
                        _configuration["EmailSettings:SmtpPassword"]);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    throw;
                }
            }
        }
    }
}