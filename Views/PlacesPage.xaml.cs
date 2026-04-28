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
        SetButtonsEnabled(false);
        PlacesListView.ItemsSource = null;

        try
        {
            var data = await ApiService.GetAsync<PlaceResponse>("places");
            PlacesListView.ItemsSource = data.places;
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

        await ApiService.DelAsync($"places/{selectedPlace.slug}", selectedPlace.name, LoadPlaces);
    }
}