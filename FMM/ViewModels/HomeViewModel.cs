using ReactiveUI;
using System.Reactive;

namespace FMM.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ReactiveCommand<string, Unit>? NavigateCommand { get; set; }
    }
}
