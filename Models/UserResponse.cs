using System;
using System.Collections.Generic;
using System.Text;

namespace bp360_admin_panel.Models
{
    public class UserResponse
    {
        public List<User> users { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}
