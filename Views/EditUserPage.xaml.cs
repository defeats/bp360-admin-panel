using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Text;

namespace bp360_admin_panel;

[QueryProperty(nameof(UserData), "SelectedUserToEdit")]
public partial class EditUserPage : ContentPage
{
    private User userData;

    public User UserData
    {
        get => userData;
        set
        {
            userData = value;
            OnPropertyChanged();
            BindingContext = userData;
        }
    }

	public EditUserPage()
	{
		InitializeComponent();
	}

    private async void SaveUser_Clicked(object sender, EventArgs e)
    {
        if (userData == null) return;

        try
        {
            SaveUser.IsEnabled = false;

            string json = JsonConvert.SerializeObject(UserData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await ApiService.Client.PutAsync($"users/{UserData.id}", content);

            if (res.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Siker", "A felhasználó adatai frissítve lettek", "OK");
                await Shell.Current.GoToAsync("//adminpanel");
            }
            else
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        }
        finally
        {
            SaveUser.IsEnabled = true;
        }
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
        UserData = null;
    }
}