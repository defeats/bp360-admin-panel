using Microsoft.Extensions.DependencyInjection;

namespace bp360_admin_panel
{
    public partial class App : Application
    {
        private AppShell shell;
        public App()
        {
            InitializeComponent();
            shell = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await CheckTokenAndNavigate();
        }

        private async Task CheckTokenAndNavigate()
        {
            string token = await SecureStorage.Default.GetAsync("token");

            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//adminpanel");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}