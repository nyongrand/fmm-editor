using ReactiveUI;

namespace FMM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string greeting = "Welcome to Avalonia!";

    public string Greeting
    {
        get => greeting;
        set => this.RaiseAndSetIfChanged(ref greeting, value);
    }
}
