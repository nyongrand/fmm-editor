using FMEditor.ViewModels;

namespace FMEditor.Views;

public partial class NationDetailPage : ContentPage
{
    public NationDetailPage(NationDetailViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}