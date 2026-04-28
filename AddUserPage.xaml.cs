using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using System.Text.Unicode;
namespace bp360_admin_panel;

public partial class AddUserPage : ContentPage
{
    public HttpClient client;
    public HttpClientHandler handler;
	public AddUserPage()
	{
        InitializeComponent();
        handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        client = new HttpClient(handler);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async void CreateUser_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(name.Text) && !string.IsNullOrEmpty(email.Text) && !string.IsNullOrEmpty(password.Text))
        {
            var token = await SecureStorage.Default.GetAsync("token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                User newUser = new User
                {
                    name = name.Text,
                    email = email.Text,
                    password = password.Text
                };

                string jsonCred = JsonSerializer.Serialize(newUser);
                var content = new StringContent(jsonCred, Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync("http://localhost:8000/api/register", content);

                if (res.IsSuccessStatusCode)
                {
                    await DisplayAlertAsync("Siker", "Felhasználó létrehozva", "OK");
                    await Shell.Current.GoToAsync("//adminpanel");
                } else
                {
                    string errorBody = await res.Content.ReadAsStringAsync();
                    await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
                }
            } catch (Exception ex)
            {
                await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
            }
        } else
        {
            await DisplayAlertAsync("Hiba", "Valamelyik entry üres", "OK");
        }
    }

    private void GoBack_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//adminpanel");
    }
}