using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace eParty.Models
{
    public class Party
    {
        [Key] public int Id { get; set; }

        [StringLength(50)] public string Name { get; set; }
        public string Image { get; set; }               // nvarchar(MAX)
        [StringLength(20)] public string Type { get; set; }
        [StringLength(20)] public string Status { get; set; }
        public int Cost { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public int Slots { get; set; }
        [StringLength(100)] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // FK -> User
        [StringLength(50)]
        public string User { get; set; }

        // [ĐÃ SỬA] Đổi User thành SystemUser
        [ForeignKey(nameof(User))]
        public virtual SystemUser Owner { get; set; }

        // FK -> Menu
        public int? Menu { get; set; }
        [ForeignKey(nameof(Menu))]
        public virtual Menu MenuRef { get; set; }

        public virtual ICollection<StaffParty> StaffParties { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }

        //METHOD
        public string GetImagePath()
        {
            if (string.IsNullOrEmpty(Image))
            {
                return "/images/party/party-default.png"; // Default image path
            }
            return Image;
        }

        public double GetAverageRating()
        {
            if (Rates == null || Rates.Count == 0)
            {
                return 0; // No ratings available
            }
            return Rates.Average(r => r.Stars);
        }

        public int GetDiscountedPrice(int originalPrice, int? discountPercentage)
        {
            if (discountPercentage.HasValue && discountPercentage.Value > 0 && discountPercentage.Value <= 100)
            {
                return originalPrice - (originalPrice * discountPercentage.Value / 100);
            }
            return originalPrice; // No discount applied
        }

        public string GetStatusBadge()
        {
            switch (Status?.ToLower())
            {
                case "upcoming":
                    return "<span class='badge badge-info'>Upcoming</span>";
                case "ongoing":
                    return "<span class='badge badge-success'>Ongoing</span>";
                case "completed":
                    return "<span class='badge badge-secondary'>Completed</span>";
                case "cancelled":
                    return "<span class='badge badge-danger'>Cancelled</span>";
                default:
                    return "<span class='badge badge-light'>Unknown</span>";
            }
        }

        public bool IsUserOwner(string username)
        {
            return string.Equals(User, username, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsPartyActive()
        {
            return string.Equals(Status, "ongoing", StringComparison.OrdinalIgnoreCase);
        }

        public bool HasUserRated(string username)
        {
            return Rates != null && Rates.Any(r => r.User == username);
        }

        public string GetFormattedDateRange()
        {
            if (BeginTime.HasValue && EndTime.HasValue)
            {
                return $"{BeginTime.Value:MMM dd, yyyy} - {EndTime.Value:MMM dd, yyyy}";
            }
            return "Date not specified";
        }

        // New methods added

        /// <summary>
        /// Sends a confirmation email to the related User (Owner) containing a random code and returns the code.
        /// Assumes User model has an 'Email' property.
        /// </summary>
        /// <param name="smtpHost">SMTP server host (e.g., "smtp.gmail.com")</param>
        /// <param name="smtpPort">SMTP server port (e.g., 587)</param>
        /// <param name="fromEmail">Sender's email address</param>
        /// <param name="fromPassword">Sender's email password</param>
        /// <returns>The generated random code</returns>
        public string SendConfirmedEmail(string smtpHost, int smtpPort, string fromEmail, string fromPassword)
        {
            if (Owner == null || string.IsNullOrEmpty(Owner.Email))
            {
                throw new InvalidOperationException("Owner or Owner's email is not available.");
            }

            // Generate a random code (e.g., 6-digit alphanumeric)
            string randomCode = GenerateRandomCode(6);

            // Email content
            string subject = "Party Confirmation Code";
            string body = $"Your party '{Name}' has been confirmed. Your confirmation code is: {randomCode}";

            SendEmail(Owner.Email, subject, body, smtpHost, smtpPort, fromEmail, fromPassword);

            return randomCode;
        }

        /// <summary>
        /// Sends a rejection email to the related User (Owner) with a message about the event being canceled.
        /// Assumes User model has an 'Email' property.
        /// </summary>
        /// <param name="smtpHost">SMTP server host (e.g., "smtp.gmail.com")</param>
        /// <param name="smtpPort">SMTP server port (e.g., 587)</param>
        /// <param name="fromEmail">Sender's email address</param>
        /// <param name="fromPassword">Sender's email password</param>
        public void SendRejectedEmail(string smtpHost, int smtpPort, string fromEmail, string fromPassword)
        {
            if (Owner == null || string.IsNullOrEmpty(Owner.Email))
            {
                throw new InvalidOperationException("Owner or Owner's email is not available.");
            }

            // Email content
            string subject = "Party Rejection Notification";
            string body = $"Unfortunately, your party '{Name}' has been canceled. Please contact support for more details.";

            SendEmail(Owner.Email, subject, body, smtpHost, smtpPort, fromEmail, fromPassword);
        }

        /// <summary>
        /// Saves history by creating PriceHistory records based on the current Menu and MenuDetails.
        /// Assumes Menu has a collection of MenuDetails, and each MenuDetail has properties like ItemName and Price.
        /// Also assumes PriceHistory model has properties like PartyId, ItemName, Price, Date.
        /// </summary>
        /// <param name="dbContext">The database context to add and save the PriceHistory records</param>
        public void SaveHistory(AppDbContext dbContext)
        {
            if (MenuRef == null || MenuRef.MenuDetails == null || !MenuRef.MenuDetails.Any())
            {
                throw new InvalidOperationException("Menu or MenuDetails are not available.");
            }

            foreach (var detail in MenuRef.MenuDetails)
            {
                var cost = detail.FoodRef != null ? detail.FoodRef.Cost : 0;
                var amount = detail.Amount;

                var history = new PriceHistory
                {
                    Party = this.Id,
                    Food = detail.Food,
                    Cost = cost,
                    Amount = amount
                };

                if (PriceHistories == null)
                {
                    PriceHistories = new List<PriceHistory>();
                }
                PriceHistories.Add(history);
            }

            dbContext.SaveChanges(); // Save to database
        }

        // Helper method to generate a random alphanumeric code
        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[data[i] % chars.Length]);
            }
            return result.ToString();
        }

        // Helper method to send email using SMTP
        private void SendEmail(string toEmail, string subject, string body, string smtpHost, int smtpPort, string fromEmail, string fromPassword)
        {
            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);

                using (var mailMessage = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                })
                {
                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}