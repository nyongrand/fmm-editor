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
                .RegisterServices()
                .RegisterPages()
                .RegisterViewModels()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<NationParser>();
            mauiAppBuilder.Services.AddSingleton<CompetitionParser>();
            mauiAppBuilder.Services.AddSingleton<ClubParser>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterPages(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddTransient<MainPage>();
            mauiAppBuilder.Services.AddTransient<NationListPage>();
            mauiAppBuilder.Services.AddTransient<NationDetailPage>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<MainViewModel>();
            mauiAppBuilder.Services.AddTransient<NationListViewModel>();
            mauiAppBuilder.Services.AddTransient<NationDetailViewModel>();

            return mauiAppBuilder;
        }
    }
}