using FMEditor.ViewModels;
using UraniumUI.Pages;

namespace FMEditor.Views;

public partial class NationDetailPage : UraniumContentPage
{
    public NationDetailPage(NationDetailViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();
    }
}