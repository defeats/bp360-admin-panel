using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using System.Text;
using Newtonsoft.Json;

namespace bp360_admin_panel;

[QueryProperty(nameof(ReviewData), "SelectedReviewToEdit")]
public partial class EditReviewPage : ContentPage
{
    private Review reviewData;

    public Review ReviewData
    {
        get => reviewData;
        set
        {
            reviewData = value;
            OnPropertyChanged();
            BindingContext = reviewData;
        }
    }

	public EditReviewPage()
	{
		InitializeComponent();
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
        ReviewData = null;
    }

    private async void SaveReview_Clicked(object sender, EventArgs e)
    {
        if (reviewData == null) return;

        try
        {
            SaveReview.IsEnabled = false;
            bool isSuccess = await ApiService.PutAsync($"reviews/{ReviewData.id}", ReviewData);

            if (isSuccess)
            {
                await Shell.Current.GoToAsync("//adminpanel");
            }
        }
        finally
        {
            SaveReview.IsEnabled = true;
        }
    }

    private void StarEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int value))
        {
            if (value < 1 || value > 5)
            {
                var entry = (Entry)sender;
                entry.TextChanged -= StarEntry_TextChanged;
                entry.Text = e.OldTextValue;
                entry.TextChanged += StarEntry_TextChanged;
            }
        }
        else
        {
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}