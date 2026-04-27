using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
namespace bp360_admin_panel;

public partial class PlacesPage : ContentPage
{
    public HttpClient client;
    public HttpClientHandler handler;
    public PlacesPage()
	{
		InitializeComponent();
        handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        client = new HttpClient(handler);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            HttpResponseMessage res = await client.GetAsync("http://localhost:8000/api/places");
            if (res.IsSuccessStatusCode)
            {
                var jsonResponse = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PlaceResponse>(jsonResponse);
                if (data != null)
                {
                    PlacesListView.ItemsSource = data.places;
                }
                else
                {
                    await DisplayAlertAsync("Hiba", "Nem sikerült deszerializálni a választ.", "OK");
                }
            }
            else
            {
                await DisplayAlertAsync("Hiba", "HttpResponse hiba: " + res.StatusCode, "OK");
            }
        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Lekérési hiba: " + ex.Message, "OK");
        }
    }

    private void AddPlace_Clicked(object sender, EventArgs e)
    {

    }

    private void UpdatePlace_Clicked(object sender, EventArgs e)
    {

    }

    private async void DeletePlace_Clicked(object sender, EventArgs e)
    {
        Place selectedPlace = (Place)PlacesListView.SelectedItem;
        if (selectedPlace != null)
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage res = await client.DeleteAsync($"http://localhost:8000/api/places/{selectedPlace.id}");
                if (res.IsSuccessStatusCode)
                {
                    await DisplayAlertAsync("Siker", "Hely sikeresen törölve.", "OK");
                    PlacesListView.ItemsSource = null;
                    OnAppearing();
                }
                else
                {
                    await DisplayAlertAsync("Hiba", "HttpResponse hiba: " + res.StatusCode, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Hiba", "Törlési hiba: " + ex.Message, "OK");
            }
        } else
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely.", "OK");
        }
    }
}