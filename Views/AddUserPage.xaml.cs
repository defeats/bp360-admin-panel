using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using System.Text.Unicode;
namespace bp360_admin_panel;

public partial class AddUserPage : ContentPage
{
	public AddUserPage()
	{
        InitializeComponent();
    }

    private async void CreateUser_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(name.Text) ||
            string.IsNullOrWhiteSpace(email.Text) ||
            string.IsNullOrWhiteSpace(password.Text))
        {
            await DisplayAlertAsync("Hiba", "Minden mezőt ki kell tölteni!", "OK");
            return;
        }

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
            HttpResponseMessage res = await ApiService.Client.PostAsync("register", content);

            if (res.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Siker", "Felhasználó létrehozva", "OK");
                name.Text = email.Text = password.Text = string.Empty;
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
    } 
    

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
    }
}