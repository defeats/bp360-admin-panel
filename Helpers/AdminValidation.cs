using bp360_admin_panel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace bp360_admin_panel.Helpers
{
    public class AdminValidation
    {
        public User user { get; set; }
        public string token { get; set; }
    }

    public class TokenCheckResponse
    {
        public bool has_tokens { get; set; }
        public int user_id { get; set; }
        public List<UserTokenInfo> tokens { get; set; }
    }

    public class UserTokenInfo
    {
        public int id { get; set; }
        public string name { get; set; }

        [JsonProperty("last_used_at")]
        public DateTime? last_used_at { get; set; }

        [JsonProperty("expires_at")]
        public DateTime? expires_at { get; set; }
        public bool is_expired { get; set; }
        public int? days_until_expiry { get; set; }
    }
}
