using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace eParty
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var fromEmail = ConfigurationManager.AppSettings["emailUsername"];
            var password = ConfigurationManager.AppSettings["emailPassword"];
            var host = ConfigurationManager.AppSettings["smtpHost"];
            var portString = ConfigurationManager.AppSettings["smtpPort"];
            int port = int.Parse(portString);

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var mail = new MailMessage(fromEmail, message.Destination)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            try
            {
                await client.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                // QUAN TRỌNG: Ném ngoại lệ để lỗi hiển thị trong Visual Studio
                // Lỗi này thường là "Authentication failed" (4.7.14) nếu App Password sai.
                throw new Exception($"SMTP Error: Failed to send email to {message.Destination}. Code: {ex.StatusCode}. Message: {ex.Message}", ex);
            }
        }
    }
}