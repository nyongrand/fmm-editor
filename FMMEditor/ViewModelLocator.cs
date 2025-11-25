using CommunityToolkit.Mvvm.DependencyInjection;
using FMMEditor.ViewModels;

namespace FMMEditor
{
    public class ViewModelLocator
    {
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
    }
}
