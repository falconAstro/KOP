using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Logging;
using TimeManagementApp.Pages;

namespace TimeManagementApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            //Firebase client pre databazu
            builder.Services.AddSingleton(new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/"));
            //Firebase client pre Auth
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyDf5i5Ycs - ygbAFJfdPV3_kwRtTHRJra0Q",
                AuthDomain = "timemanagement-4d83d.web.app",
                Providers = [new EmailProvider()]
            }));

            //Pages
            builder.Services.AddSingleton<PersonalTasks>();
            builder.Services.AddSingleton<SharedEvents>();
            builder.Services.AddSingleton<SignUpPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SettingsPage>();

            return builder.Build();
        }
    }
}
