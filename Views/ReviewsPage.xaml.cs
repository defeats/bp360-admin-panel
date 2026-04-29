using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;

namespace bp360_admin_panel;

public partial class ReviewsPage : ContentPage
{
	public ReviewsPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReviews();
    }

    private async Task LoadReviews()
    {
        SetButtonsEnabled(false);
        ReviewsListView.ItemsSource = null;
        try
        {
            var data = await ApiService.GetAsync<ReviewResponse>("reviews");
            ReviewsListView.ItemsSource = data.reviews;
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
        EditReview.IsEnabled = isEnabled;
        DeleteReview.IsEnabled = isEnabled;
    }

    private async void EditReview_Clicked(object sender, EventArgs e)
    {
        if (ReviewsListView.SelectedItem is not Review selectedReview)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva vélemény", "OK");
            return;
        }

        var navParam = new Dictionary<string, object>
        {
            { "SelectedReviewToEdit", selectedReview }
        };

        await Shell.Current.GoToAsync("//editreview", navParam);
    }

    private async void DeleteReview_Clicked(object sender, EventArgs e)
    {
        if (ReviewsListView.SelectedItem is not Review selectedReview)
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva vélemény", "OK");
            return;
        }

        await ApiService.DelAsync($"reviews/{selectedReview.id}", $"ID: {selectedReview.id}", LoadReviews);
    }
}