using TimeManagementApp.Pages;

namespace TimeManagementApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
       
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(PersonalTasks), typeof(PersonalTasks));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(SharedTasks), typeof(SharedTasks));
            Routing.RegisterRoute(nameof(ShoppingLists), typeof(ShoppingLists));
            Routing.RegisterRoute(nameof(SharedEvents), typeof(SharedEvents));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
