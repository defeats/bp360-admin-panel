using System;
using System.Collections.Generic;
using System.Text;

namespace bp360_admin_panel
{
    public class PlaceResponse
    {
        public List<Place> places { get; set; }
    }

    public class Place
    {
        public int id { get; set; }
        public int category_id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int post_code { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string? email { get; set; }
        public string? website { get; set; }
        public string description { get; set; }
        public bool outdoor_seating { get; set; }
        public bool wifi { get; set; }
        public bool pet_friendly { get; set; }
        public bool family_friendly { get; set; }
        public bool card_payment { get; set; }
        public bool free_parking { get; set; }
        public bool free_entry { get; set; }
        public bool photo_spot { get; set; }
        public bool accessible { get; set; }
        public bool student_discount { get; set; }
        public string status { get; set; }
        public int clicks { get; set; }
    }
}
