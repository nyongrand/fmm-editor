using FMEditor.ViewModels;

namespace FMEditor
{
    public partial class App : Application
    {
        public App(MainViewModel viewModel)
        {
            InitializeComponent();

            MainPage = new AppShell(viewModel);
        }
    }
}