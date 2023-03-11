using FMEditor.ViewModels;

namespace FMEditor.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}