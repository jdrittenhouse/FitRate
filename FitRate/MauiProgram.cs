using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FitRate.Core.Abstractions;
using FitRate.Core.Services;
using FitRate.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace FitRate.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ITokenStorage,MauiTokenStorage>();
            builder.Services.AddSingleton<FirebaseAuthService>();
            builder.Services.AddSingleton<FirebaseDataService>(serviceProvider => { 
                string firebaseDatabaseUrl = "https://firestore.googleapis.com/v1/projects/fitrateco/databases/(default)/documents";
                var httpClient = serviceProvider.GetRequiredService<HttpClient>();
                return new FirebaseDataService(httpClient, firebaseDatabaseUrl);
            });
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
            var mauiApp = builder.Build();
            
            return mauiApp;
        }
    }
}
