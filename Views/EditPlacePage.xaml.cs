using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace bp360_admin_panel;

[QueryProperty(nameof(PlaceData), "SelectedPlaceToEdit")]
public partial class EditPlacePage : ContentPage
{
    private Place placeData;

    public Place PlaceData
    {
        get => placeData;
        set
        {
            placeData = value;
            if (placeData != null && !string.IsNullOrEmpty(placeData.address))
            {
                placeData.address = placeData.address.Replace("Budapest,", "").TrimStart();
            }

            OnPropertyChanged();
            BindingContext = placeData;
        }
    }

    public EditPlacePage()
    {
        InitializeComponent();
    }

    private async void SaveChanges_Clicked(object sender, EventArgs e)
    {
        if (PlaceData == null) return;

        try
        {
            SaveChangesButton.IsEnabled = false;

            string json = JsonConvert.SerializeObject(PlaceData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await ApiService.Client.PutAsync($"places/{PlaceData.slug}", content);

            if (res.IsSuccessStatusCode)
            {
                await DisplayAlertAsync("Siker", "A hely adatai frissítve lettek", "OK");
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
            SaveChangesButton.IsEnabled = true;
        }
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
        PlaceData = null;
    }
}