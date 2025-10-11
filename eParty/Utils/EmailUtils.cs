using System;
using System.Net;
using System.Net.Mail;

namespace eParty.Utils
{
    public class EmailUtils
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        public EmailUtils(string from, string to, string subject, string description)
        {
            From = from;
            To = to;
            Subject = subject;
            Description = description;
        }

        public bool Send(string from, string to)
        {
            try
            {
                var mailMessage = new MailMessage(from, to, Subject, Description);
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi email: " + ex.Message);
            }
        }

        public bool Send()
        {
            return Send(From, To);
        }
    }
}