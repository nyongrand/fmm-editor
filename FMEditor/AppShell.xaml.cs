using FMEditor.ViewModels;

namespace FMEditor;

public partial class AppShell : Shell
{
    public AppShell(MainViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}
