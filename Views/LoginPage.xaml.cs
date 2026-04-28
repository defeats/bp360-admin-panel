using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;
using bp360_admin_panel.Helpers;

namespace bp360_admin_panel;

public partial class LoginPage : ContentPage
{
    private HttpClient client;
    private HttpClientHandler handler;
    public LoginPage()
	{
		InitializeComponent();
        handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };

        client = new HttpClient(handler);


	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {

        var loginData = new { email = EmailEntry.Text, password = PasswordEntry.Text };
        string url = "http://localhost:8000/api/login";

        if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlertAsync("Hiba", "Hiányos mezők", "OK");
        }

        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(url, loginData);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AdminValidation>(jsonResponse);

                if (data.user.role != "admin")
                {
                    await DisplayAlertAsync("Hiba", "Nincs admin jogosultságod", "OK");
                    return;
                } else
                {
                    try
                    {
                        await SecureStorage.Default.SetAsync("token", data.token);
                        await Shell.Current.GoToAsync("//adminpanel");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlertAsync("Hiba", $"Token validálási hiba: {ex.Message}", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlertAsync("Hiba", $"HttpResponse hiba: {response.StatusCode}", "OK");
            }

        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", $"Hiba történt: {ex.Message}", "OK");
        }
    }
}