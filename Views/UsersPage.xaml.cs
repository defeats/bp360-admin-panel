using bp360_admin_panel.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using bp360_admin_panel.Services;
namespace bp360_admin_panel;

public partial class UsersPage : ContentPage
{
	public UsersPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        try
        {
            SetButtonsEnabled(false);
            UsersListView.ItemsSource = null;

            HttpResponseMessage res = await ApiService.Client.GetAsync("users");    
            if (res.IsSuccessStatusCode)
            {
                var jsonRes = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserResponse>(jsonRes);

                if (data != null)   UsersListView.ItemsSource = data.users;
                else await DisplayAlertAsync("Hiba", "Üres vagy érvénytelen adatobjektum", "OK");
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
            SetButtonsEnabled(true);
        }
    }

    private void SetButtonsEnabled(bool isEnabled)
    {
        AddUser.IsEnabled = isEnabled;
        EditUser.IsEnabled = isEnabled;
        DeleteUser.IsEnabled = isEnabled;
    }

    private void AddUser_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//adduser");
    }

    private void EditUser_Clicked(object sender, EventArgs e)
    {
        // TODO: edituser page
    }

    private async void DeleteUser_Clicked(object sender, EventArgs e)
    {
        if (UsersListView.SelectedItem is not User selectedUser)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva felhasználó", "OK");
            return;
        }

        bool confirm = await DisplayAlertAsync("Megerősítés", $"Biztosan törlöd {selectedUser.name} felhasználót?", "Igen", "Nem");
        if (!confirm) return;

        try
        {
            HttpResponseMessage res = await ApiService.Client.DeleteAsync($"users/{selectedUser.id}");
            if (res.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Siker", "Felhasználó sikeresen törölve.", "OK");
                await LoadUsers();
            } 
            else
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
            }
        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        }
    }
}