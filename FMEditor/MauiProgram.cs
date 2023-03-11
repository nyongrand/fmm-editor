using FMEditor.ViewModels;
using FMEditor.Views;
using FMELibrary;
using Microsoft.Extensions.Logging;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FMEditor
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

            builder.Services.AddSingleton<NationParser>();
            builder.Services.AddSingleton<CompetitionParser>();
            builder.Services.AddSingleton<ClubParser>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<NationsPage>();
            builder.Services.AddTransient<NationDetailPage>();

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<NationsViewModel>();
            builder.Services.AddTransient<NationDetailViewModel>();

            return builder.Build();
        }
    }
}