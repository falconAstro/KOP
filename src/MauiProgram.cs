﻿using Firebase.Database;
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
            //Firebase client
            builder.Services.AddSingleton(new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/"));
            builder.Services.AddSingleton<PersonalTasks>();
            builder.Services.AddSingleton<SharedEvents>();
            return builder.Build();
        }
    }
}
