using Newtonsoft.Json;
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

        public static async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            HttpResponseMessage res = await Client.GetAsync(endpoint);

            if (!res.IsSuccessStatusCode)
            {
                string error = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Szerver hiba ({res.StatusCode}): {error}");
            }

            var jsonRes = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(jsonRes);

            if (result == null)
            {
                throw new JsonException("Nem sikerült deszerializálni a választ.");
            }

            return result;
        }

        public static async Task DelAsync(string endpoint, string itemName, Func<Task> onSuccess)
        {
            bool confirm = await Shell.Current.DisplayAlertAsync("Megerősítés", $"Biztosan törlöd a következőt: {itemName}?", "Igen", "Nem");
            if (!confirm) return;

            try
            {
                HttpResponseMessage res = await Client.DeleteAsync(endpoint);

                if (res.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlertAsync("Siker", "Sikeres törlés", "OK");
                    await onSuccess();
                }
                else
                {
                    string error = await res.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
            }
        }

        public static HttpClient Client => client;
    }
}
