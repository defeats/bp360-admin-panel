namespace bp360_admin_panel;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
	}

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlertAsync("Kijelentkezés", "Biztosan ki szeretnél lépni?", "Igen", "Mégse");

        if (answer)
        {
            SecureStorage.Default.Remove("token");
            await Shell.Current.GoToAsync("//login");
        }
    }
}