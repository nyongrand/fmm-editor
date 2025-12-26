using Avalonia.Controls;

namespace FMM.Views;

public partial class MainWindow : Window
{
    private MainView _mainView;

    public MainWindow()
    {
        InitializeComponent();

        _mainView = new MainView();
        navigateView.Content = _mainView;
    }
}