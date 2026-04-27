namespace bp360_admin_panel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;

public partial class DashboardPage : ContentPage
{
    private HttpClient client;
    private HttpClientHandler handler;
    public DashboardPage()
	{
		InitializeComponent();
        handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        client = new HttpClient(handler);
	}

    protected override async void OnAppearing()
    {
        PlacesInDb.Text = "Helyek száma az adatbázisban: ";
        ReviewsInDb.Text = "Értékelések száma az adatbázisban: ";
        UsersInDb.Text = "Felhasználók száma az adatbázisban: ";
        base.OnAppearing();
        try
        {
            HttpResponseMessage placeResponse = await client.GetAsync("http://localhost:8000/api/places");
                if (placeResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await placeResponse.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<PlaceResponse>(jsonResponse);
                    if (data?.places != null)
                    {
                        PlacesInDb.Text += data.places.Count.ToString();
                    } else PlacesInDb.Text += "0";
                }
                else
                {
                    await DisplayAlertAsync("Hiba", "HttpResponse hiba: " + placeResponse.StatusCode, "OK");
                }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Lekérési hiba: " + ex.Message, "OK");
        }
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlertAsync("Kijelentkezés", "Biztosan ki szeretnél lépni?", "Igen", "Mégse");

        if (answer)
        {
            SecureStorage.Default.Remove("token");
            await Shell.Current.GoToAsync("//login");
        }
    }
}