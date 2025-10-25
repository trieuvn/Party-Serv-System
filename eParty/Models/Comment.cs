using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Comment
    {
        // Diagram ghi "user int" nhưng để nhất quán PK User là string:
        [Key, Column(Order = 0), StringLength(50)]
        public string User { get; set; }
        [Key, Column(Order = 1)]
        public int News { get; set; }
        public int Stars { get; set; }
        public string Description { get; set; }

        // [ĐÃ SỬA] Đổi User thành SystemUser
        [ForeignKey(nameof(User))] public virtual SystemUser UserRef { get; set; }
        [ForeignKey(nameof(News))] public virtual News NewsRef { get; set; }
    }
}