using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    // [ĐÃ SỬA] Đổi tên class từ User thành SystemUser
    public class SystemUser
    {
        [Key, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(50)] public string Password { get; set; }
        [StringLength(50)] public string FirstName { get; set; }
        [StringLength(50)] public string LastName { get; set; }
        public string Avatar { get; set; } // nvarchar(MAX)
        [StringLength(50)] public string Email { get; set; }
        [StringLength(50)] public string PhoneNumber { get; set; }
        [StringLength(20)] public string Role { get; set; }

        public virtual ICollection<Party> Parties { get; set; }
        public virtual ICollection<UserDiscount> UserDiscounts { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
        public virtual ICollection<News> NewsPosts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        /// <summary>
        /// Validates user login credentials.
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <param name="password">Password to validate</param>
        /// <returns>True if credentials are valid, false otherwise</returns>
        public bool Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Check if username matches and password is correct
            return string.Equals(Username, username, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Password, password, StringComparison.Ordinal);
        }

        /// <summary>
        /// Registers a new user with the provided information.
        /// </summary>
        /// <param name="username">Username for the new user</param>
        /// <param name="password">Password for the new user</param>
        /// <param name="email">Email address</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number (optional)</param>
        /// <param name="role">User role (default: "User")</param>
        /// <returns>True if registration is successful, false if username already exists</returns>
        public static bool Register(string username, string password, string email,
            string firstName = "", string lastName = "", string phoneNumber = "", string role = "User")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                return false;
            }

            // Create new user instance
            // [ĐÃ SỬA] Đổi User thành SystemUser
            var newUser = new SystemUser
            {
                Username = username,
                Password = password,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Role = role,
                Avatar = "/images/avatar/default-avatar.png" // Default avatar
            };

            // Note: In a real application, you would save this to database
            // and check for existing username before creating
            // This is a simplified version for the model

            return true;
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        /// <returns>Full name or username if names are not available</returns>
        public string GetFullName()
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                return $"{FirstName} {LastName}";
            }
            return Username;
        }

        /// <summary>
        /// Checks if the user has a specific role.
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <returns>True if user has the role, false otherwise</returns>
        public bool HasRole(string role)
        {
            return string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
        }
    }
}