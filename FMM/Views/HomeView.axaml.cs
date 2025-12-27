using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace FMM.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            window.Close();
        }
    }
}