using CommunityToolkit.Mvvm.DependencyInjection;
using FMEViewer.ViewModels;

namespace FMEViewer
{
    public class ViewModelLocator
    {
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
    }
}
