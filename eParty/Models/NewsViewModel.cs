using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eParty.Models
{
        public class NewsViewModel
        {
            public List<News> TrendingNews { get; set; } // Tin nổi bật (slider)
            public List<News> WhatsNew { get; set; } // Tin mới
            public List<News> MostPopularNews { get; set; } // Tin phổ biến
            public List<News> RecentArticles { get; set; } // Bài viết gần đây
            public List<News> WeeklyNews { get; set; } // Tin hàng tuần
        }
}