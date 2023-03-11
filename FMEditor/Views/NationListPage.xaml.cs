using FMEditor.ViewModels;

namespace FMEditor.Views;

public partial class NationListPage : ContentPage
{
    public NationListPage(NationListViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}