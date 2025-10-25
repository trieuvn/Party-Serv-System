using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class News
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        [StringLength(100)] public string Subject { get; set; }
        public string Description { get; set; }
        [StringLength(20)] public string Status { get; set; }

        // FK -> User (author)
        [StringLength(50)]
        public string User { get; set; }

        // [ĐÃ SỬA] Đổi User thành SystemUser
        [ForeignKey(nameof(User))] public virtual SystemUser Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        /// <summary>
        /// Gets the average star rating from all comments on this news.
        /// Returns 0 if no comments or ratings are available.
        /// </summary>
        /// <returns>Average star rating of the news</returns>
        public double GetAvgStar()
        {
            if (Comments == null || !Comments.Any())
            {
                return 0;
            }

            // Calculate average of all star ratings
            return Comments.Average(c => c.Stars);
        }
    }
}