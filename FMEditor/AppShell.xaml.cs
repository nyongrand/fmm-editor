using FMEditor.ViewModels;
using FMEditor.Views;

namespace FMEditor
{
    public partial class AppShell : Shell
    {
        public AppShell(MainViewModel viewModel)
        {
            BindingContext = viewModel;

            Routing.RegisterRoute("NationDetail", typeof(NationDetailPage));

            InitializeComponent();
        }
    }
}