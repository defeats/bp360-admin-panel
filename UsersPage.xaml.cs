using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
namespace bp360_admin_panel;

public partial class UsersPage : ContentPage
{
    public HttpClient client;
    public HttpClientHandler handler;
	public UsersPage()
	{
		InitializeComponent();
        handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };
        client = new HttpClient(handler);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            AddUser.IsEnabled = false;
            EditUser.IsEnabled = false;
            DeleteUser.IsEnabled = false;
            UsersListView.ItemsSource = null;

            var token = await SecureStorage.Default.GetAsync("token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage res = await client.GetAsync("http://localhost:8000/api/users");
            if (res.IsSuccessStatusCode)
            {
                var jsonRes = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserResponse>(jsonRes);
                if (data != null)
                {
                    UsersListView.ItemsSource = data.users;
                } else
                {
                    await DisplayAlertAsync("Hiba", "Nem sikerült deszerializálni a választ.", "OK");
                }
            } else
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
            }
        } catch (Exception ex)
        {
            await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
        } finally
        {
            AddUser.IsEnabled = true;
            EditUser.IsEnabled = true;
            DeleteUser.IsEnabled = true;
        }
    }

    private void AddUser_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//adduser");
    }

    private void EditUser_Clicked(object sender, EventArgs e)
    {

    }

    private async void DeleteUser_Clicked(object sender, EventArgs e)
    {
        User selectedUser = (User)UsersListView.SelectedItem;

        if (selectedUser != null)
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage res = await client.DeleteAsync($"http://localhost:8000/api/users/{selectedUser.id}");
                if (res.IsSuccessStatusCode)
                {
                    await DisplayAlertAsync("Siker", "Felhasználó sikeresen törölve.", "OK");
                } 
                else
                {
                    string errorBody = await res.Content.ReadAsStringAsync();
                    await DisplayAlertAsync("Hiba", $"Szerver hiba ({res.StatusCode}): {errorBody}", "OK");
                }
            } catch (Exception ex)
            {
                await DisplayAlertAsync("Hiba", "Hálózati hiba: " + ex.Message, "OK");
            } finally
            {
                OnAppearing();
            }
        } else
        {
            await DisplayAlertAsync("Hiba", "Nincs kiválasztva felhasználó", "OK");
        }
    }
}