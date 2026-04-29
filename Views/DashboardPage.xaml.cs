namespace bp360_admin_panel;

using bp360_admin_panel.Helpers;
using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public partial class DashboardPage : ContentPage
{
    public List<Category> HardcodedCategories { get; set; } = new List<Category>
    {
        new Category { id = 1, name = "Éttermek" },
        new Category { id = 2, name = "Látnivalók" },
        new Category { id = 3, name = "Éjszakai élet" },
        new Category { id = 4, name = "Szállások" },
        new Category { id = 5, name = "Bevásárlóközpontok" },
        new Category { id = 6, name = "Kultúra" },
        new Category { id = 7, name = "Események" }
    };

    public DashboardPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadingData.IsVisible = true;
        CategoryContainer.IsVisible = false;
        SetLabelsVisibility(false);

        await LoadStatistics();
    }

    private async Task LoadStatistics()
    {
        try
        {
            LoadingData.Text = "Betöltés...";

            var placeData = await ApiService.GetAsync<PlaceResponse>("places");
            var userData = await ApiService.GetAsync<UserResponse>("users");
            var reviewData = await ApiService.GetAsync<ReviewResponse>("reviews");

            if (placeData?.places != null && userData?.users != null && reviewData?.reviews != null)
            {
                PlacesInDb.Text = "Helyek száma az adatbázisban: " + placeData.places.Count.ToString();
                UsersInDb.Text = "Felhasználók száma az adatbázisban: " + userData.users.Count.ToString();
                ReviewsInDb.Text = "Értékelések száma az adatbázisban: " + reviewData.reviews.Count.ToString() + "\n";
            }

            var tokenInfo = await ApiService.GetAsync<TokenCheckResponse>("checkTokenExpiryDate");
            TokenExpiryDate.Text = $"A tokened {tokenInfo.tokens[0].days_until_expiry} nap múlva lejár. \n";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        }
        finally
        {
            LoadingData.IsVisible = false;
            SetLabelsVisibility(true);
            CategoryContainer.IsVisible = true;
        }
    }

    private void SetLabelsVisibility(bool isEnabled)
    {
        PlacesInDb.IsVisible = isEnabled;
        UsersInDb.IsVisible = isEnabled;
        ReviewsInDb.IsVisible = isEnabled;
        TokenExpiryDate.IsVisible = isEnabled;
        CategoryLabel.IsVisible = isEnabled;
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlertAsync("Kijelentkezés", "Biztosan ki szeretnél lépni?", "Igen", "Mégse");

        if (answer)
        {
            SecureStorage.Default.Remove("token");
            ApiService.SetAuthToken(null);
            await Shell.Current.GoToAsync("//login");
        }
    }
}