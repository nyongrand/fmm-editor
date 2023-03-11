using FMEditor.ViewModels;

namespace FMEditor.Views;

public partial class NationsPage : ContentPage
{
    public NationsPage(NationsViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}