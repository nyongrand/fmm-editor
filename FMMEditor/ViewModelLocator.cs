using CommunityToolkit.Mvvm.DependencyInjection;
using FMMEditor.ViewModels;

namespace FMMEditor
{
    public class ViewModelLocator
    {
        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();
        public CompetitionsViewModel Competitions => Ioc.Default.GetRequiredService<CompetitionsViewModel>();
        public NamesViewModel Names => Ioc.Default.GetRequiredService<NamesViewModel>();
        public PeopleViewModel People => Ioc.Default.GetRequiredService<PeopleViewModel>();
    }
}
