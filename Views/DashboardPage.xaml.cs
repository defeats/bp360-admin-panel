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
            HttpResponseMessage placesRes = await ApiService.Client.GetAsync("places");
            HttpResponseMessage usersRes = await ApiService.Client.GetAsync("users");

            if (placesRes.IsSuccessStatusCode && usersRes.IsSuccessStatusCode)
            {
                var placesJsonRes = await placesRes.Content.ReadAsStringAsync();
                var placesData = JsonConvert.DeserializeObject<PlaceResponse>(placesJsonRes);

                var usersJsonRes = await usersRes.Content.ReadAsStringAsync();
                var usersData = JsonConvert.DeserializeObject<UserResponse>(usersJsonRes);

                if (placesData?.places != null && usersData?.users != null)
                {
                    PlacesInDb.Text += placesData.places.Count.ToString();
                    UsersInDb.Text += usersData.users.Count.ToString();
                    //TODO: Reviews statistics
                }

                else PlacesInDb.Text += "0"; UsersInDb.Text += "0";
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