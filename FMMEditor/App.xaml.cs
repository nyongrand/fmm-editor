using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using FMMEditor.ViewModels;
using MaterialDesignThemes.Wpf;

namespace FMMEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddSingleton<IDialogService, DialogService>()
                    .AddSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>()
                    .AddTransient<MainViewModel>()
                    .AddTransient<CompetitionsViewModel>()
                    .AddTransient<NamesViewModel>()
                    .AddTransient<PeopleViewModel>()
                    .AddTransient<ClubsViewModel>()
                    .BuildServiceProvider());
        }
    }
}
