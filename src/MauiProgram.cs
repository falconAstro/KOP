using TimeManagementApp.Pages;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TimeManagementApp.Services;

namespace TimeManagementApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
            SecureStorage.SetAsync("fireBaseAuthApiKey", "AIzaSyDf5i5Ycs - ygbAFJfdPV3_kwRtTHRJra0Q");
            builder.Logging.AddDebug();
#endif
           
            builder.Services.AddSingleton<FirebaseService>();
            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<PersonalTasks>();
            builder.Services.AddTransient<SharedTasks>();
            builder.Services.AddTransient<SharedEvents>();
            builder.Services.AddTransient<ShoppingLists>();
            builder.Services.AddTransient<SettingsPage>();
            return builder.Build();
        }
    }
}