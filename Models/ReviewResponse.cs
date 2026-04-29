using System;
using System.Collections.Generic;
using System.Text;

namespace bp360_admin_panel.Models
{
    public class ReviewResponse
    {
        public List<Review> reviews { get; set; }
    }

    public class Review
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int place_id { get; set; }
        public string? comment { get; set; }
        public int star { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}
