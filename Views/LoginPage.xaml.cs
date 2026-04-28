using bp360_admin_panel.Helpers;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace bp360_admin_panel;

public partial class LoginPage : ContentPage
{
    public LoginPage()
	{
		InitializeComponent();
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlertAsync("Hiba", "Hiányos mezők", "OK");
        }

        var loginData = new { email = EmailEntry.Text, password = PasswordEntry.Text };

        try
        {
            HttpResponseMessage res = await ApiService.Client.PostAsJsonAsync("login", loginData);
            if (res.IsSuccessStatusCode)
            {
                var jsonRes = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AdminValidation>(jsonRes);

                if (data.user.role != "admin")
                {
                    await DisplayAlertAsync("Hiba", "Nincs admin jogosultságod", "OK");
                    return;
                }

                await SecureStorage.Default.SetAsync("token", data.token);
                ApiService.SetAuthToken(data.token);
                await Shell.Current.GoToAsync("//adminpanel");
            }
            else
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
            }
        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", $"Hálózati hiba: {ex.Message}", "OK");
        }
    }
}