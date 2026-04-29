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

    private async void SavePlace_Clicked(object sender, EventArgs e)
    {
        if (PlaceData == null) return;

        try
        {
            SavePlace.IsEnabled = false;
            bool isSuccess = await ApiService.PutAsync($"places/{PlaceData.slug}", PlaceData);

            if (isSuccess)
            {
                await Shell.Current.GoToAsync("//adminpanel");
            }
        }
        finally
        {
            SavePlace.IsEnabled = true;
        }
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
        PlaceData = null;
    }
}