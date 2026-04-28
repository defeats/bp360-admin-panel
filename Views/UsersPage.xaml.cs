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
        SetButtonsEnabled(false);
        UsersListView.ItemsSource = null;

        try
        {
            var data = await ApiService.GetAsync<UserResponse>("users");
            UsersListView.ItemsSource = data.users;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", ex.Message, "OK");
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

    private async void AddUser_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adduser");
    }

    private async void EditUser_Clicked(object sender, EventArgs e)
    {
        if (UsersListView.SelectedItem is not User selectedUser)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva felhasználó", "OK");
            return;
        }

        var navParam = new Dictionary<string, object>
        {
            { "SelectedUserToEdit", selectedUser }
        };

        await Shell.Current.GoToAsync("//edituser", navParam);
    }

    private async void DeleteUser_Clicked(object sender, EventArgs e)
    {
        if (UsersListView.SelectedItem is not User selectedUser)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva felhasználó", "OK");
            return;
        }

        await ApiService.DelAsync($"users/{selectedUser.id}", selectedUser.name, LoadUsers);
    }
}