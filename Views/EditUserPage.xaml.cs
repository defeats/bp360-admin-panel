using bp360_admin_panel.Models;
using bp360_admin_panel.Services;
using Newtonsoft.Json;
using System.Text;

namespace bp360_admin_panel;

[QueryProperty(nameof(UserData), "SelectedUserToEdit")]
public partial class EditUserPage : ContentPage
{
    private User userData;

    public User UserData
    {
        get => userData;
        set
        {
            userData = value;
            OnPropertyChanged();
            BindingContext = userData;
        }
    }

	public EditUserPage()
	{
		InitializeComponent();
	}

    private async void SaveUser_Clicked(object sender, EventArgs e)
    {
        if (userData == null) return;

        try
        {
            SaveUser.IsEnabled = false;
            bool isSuccess = await ApiService.PutAsync($"users/{UserData.id}", UserData);

            if (isSuccess)
            {
                await Shell.Current.GoToAsync("//adminpanel");
            }
        }
        finally
        {
            SaveUser.IsEnabled = true;
        }
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//adminpanel");
        UserData = null;
    }
}