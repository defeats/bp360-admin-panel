using bp360_admin_panel.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace bp360_admin_panel;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(500);
        await CheckTokenAndNavigate();
    }

    private async Task CheckTokenAndNavigate()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("token");

            if (string.IsNullOrEmpty(token))
            {
                await GoToLogin();
                return;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage res = await client.GetAsync("http://localhost:8000/api/checkTokenExpiryDate");

            if (res.IsSuccessStatusCode)
            {
                var jsonRes = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserTokenInfo>(jsonRes);

                if (data != null && !data.is_expired)
                {
                    await Shell.Current.GoToAsync("//adminpanel");
                }
                else
                {
                    await HandleExpiredSession("A munkamenet lejárt.");
                }
            }
            else if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HandleExpiredSession(null);
            }
            else
            {
                await HandleExpiredSession("Szerver hiba történt.");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Hiba", "Nem sikerült kapcsolódni a szerverhez: " + ex.Message, "OK");
            return;
        }
    }

    private async Task HandleExpiredSession(string? message)
    {
        SecureStorage.Default.Remove("token");
        if (!string.IsNullOrEmpty(message))
        {
            await Shell.Current.DisplayAlertAsync("Figyelem", message, "OK");
        }
        await GoToLogin();
    }

    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("//login");
    }
}