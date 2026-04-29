using bp360_admin_panel.Models;
using bp360_admin_panel.Services;

namespace bp360_admin_panel;

public partial class PendingPlacesPage : ContentPage
{
	public PendingPlacesPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPendingPlaces();
    }

    private async Task LoadPendingPlaces()
    {
        SetButtonsEnabled(false);
        PendingPlacesListView.ItemsSource = null;

        try
        {
            var data = await ApiService.GetAsync<PlaceResponse>("pendingPlaces");
            PendingPlacesListView.ItemsSource = data.places;
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
        ApprovePlace.IsEnabled = isEnabled;
        RejectPlace.IsEnabled = isEnabled;
    }

    private async void ApprovePlace_Clicked(object sender, EventArgs e)
    {
        if (PendingPlacesListView.SelectedItem is not Place selectedPlace)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
            return;
        }

        bool isSuccess = await ApiService.PutAsync($"approvePlace/{selectedPlace.slug}", new { });

        if (isSuccess)
        {
            await LoadPendingPlaces();
        }
    }

    private async void RejectPlace_Clicked(object sender, EventArgs e)
    {
        if (PendingPlacesListView.SelectedItem is not Place selectedPlace)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva hely", "OK");
            return;
        }
        
        bool isSuccess = await ApiService.PutAsync($"rejectPlace/{selectedPlace.slug}", new { });

        if (isSuccess)
        {
            await LoadPendingPlaces();
        }
    }
}