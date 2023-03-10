using FMEditor.ViewModels;

namespace FMEditor.Views;

public partial class ClubsPage : ContentPage
{
    public ClubsPage(MainViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}