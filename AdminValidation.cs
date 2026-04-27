using System;
using System.Collections.Generic;
using System.Text;

namespace bp360_admin_panel
{
    public class AdminValidation
    {
        public UserData user { get; set; }
        public string token { get; set; }
    }

    public class UserTokenInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime? last_used_at { get; set; }
        public DateTime? expires_at { get; set; }
        public bool is_expired { get; set; }
        public int? days_until_expiry { get; set; }
    }
}
