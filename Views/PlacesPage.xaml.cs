using bp360_admin_panel.Models;
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
            EditPlace.IsEnabled = false;
            DeletePlace.IsEnabled = false;
            PlacesListView.ItemsSource = null;

            HttpResponseMessage res = await client.GetAsync("http://localhost:8000/api/places");
            if (res.IsSuccessStatusCode)
            {
                var jsonResponse = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PlaceResponse>(jsonResponse);
                if (data != null)
                {
                    PlacesListView.ItemsSource = data.places;
                } else
                {
                    await DisplayAlertAsync("Hiba", "Nem sikerült deszerializálni a választ.", "OK");
                }
            } else
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
            }
        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        } finally
        {
            EditPlace.IsEnabled = true;
            DeletePlace.IsEnabled = true;
        }
    }

    private async void EditPlace_Clicked(object sender, EventArgs e)
    {
        Place selectedPlace = (Place)PlacesListView.SelectedItem;

        if (selectedPlace != null)
        {
            var navParam = new Dictionary<string, object>
            {
                { "SelectedPlaceToEdit", selectedPlace }
            };

            await Shell.Current.GoToAsync("//editplace", navParam);
        }
        else
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
        }
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

                HttpResponseMessage res = await client.DeleteAsync($"http://localhost:8000/api/places/{selectedPlace.slug}");
                if (res.IsSuccessStatusCode)
                {
                    await DisplayAlertAsync("Siker", "Hely sikeresen törölve.", "OK");
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
            } finally
            {
                OnAppearing();
            }
        } else
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
        }
    }
}