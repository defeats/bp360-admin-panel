using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace bp360_admin_panel.Services
{
    public class ApiService
    {
        private static readonly HttpClient client;
        private static readonly HttpClientHandler handler;

        static ApiService()
        {
            handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
            };

            client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:8000/api/"),
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void SetAuthToken(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static HttpClient Client => client;
    }
}
