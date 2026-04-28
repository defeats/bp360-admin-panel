namespace bp360_admin_panel;
using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        PlacesInDb.Text = "Helyek száma az adatbázisban: ";
        ReviewsInDb.Text = "Értékelések száma az adatbázisban: ";
        UsersInDb.Text = "Felhasználók száma az adatbázisban: ";

        await LoadStatistics();
    }

    private async Task LoadStatistics()
    {
        try
        {
            var placeData = await ApiService.GetAsync<PlaceResponse>("places");
            var userData = await ApiService.GetAsync<UserResponse>("users");

            if (placeData?.places != null && userData?.users != null)
            {
                PlacesInDb.Text += placeData.places.Count.ToString();
                UsersInDb.Text += userData.users.Count.ToString();
                //TODO: Reviews statistics
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        }
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlertAsync("Kijelentkezés", "Biztosan ki szeretnél lépni?", "Igen", "Mégse");

        if (answer)
        {
            SecureStorage.Default.Remove("token");
            ApiService.SetAuthToken(null);
            await Shell.Current.GoToAsync("//login");
        }
    }
}