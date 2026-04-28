using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
namespace bp360_admin_panel;

public partial class PlacesPage : ContentPage
{
    public PlacesPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPlaces();
    }

    private async Task LoadPlaces()
    {
        try
        {
            SetButtonsEnabled(false);
            PlacesListView.ItemsSource = null;

            HttpResponseMessage res = await ApiService.Client.GetAsync("places");

            if (res.IsSuccessStatusCode)
            {
                var jsonRes = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PlaceResponse>(jsonRes);

                if (data != null)   PlacesListView.ItemsSource = data.places;
                else await DisplayAlertAsync("Hiba", "Nem sikerült deszerializálni a választ.", "OK");
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
        EditPlace.IsEnabled = isEnabled;
        DeletePlace.IsEnabled = isEnabled;
    }

    private async void EditPlace_Clicked(object sender, EventArgs e)
    {
        if (PlacesListView.SelectedItem is not Place selectedPlace)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
            return;
        }

        var navParam = new Dictionary<string, object>
        {
            { "SelectedPlaceToEdit", selectedPlace }
        };

        await Shell.Current.GoToAsync("//editplace", navParam);
    }

    private async void DeletePlace_Clicked(object sender, EventArgs e)
    {
        if (PlacesListView.SelectedItem is not Place selectedPlace)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
            return;
        }

        bool confirm = await DisplayAlertAsync("Megerősítés", $"Biztosan törlöd a következő helyet: {selectedPlace.name}?", "Igen", "Nem");
        if (!confirm) return;

        try
        {
            HttpResponseMessage res = await ApiService.Client.DeleteAsync($"places/{selectedPlace.slug}");
            if (res.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Siker", "Hely sikeresen törölve.", "OK");
                await LoadPlaces();
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
    }
}