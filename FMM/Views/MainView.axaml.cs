using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace FMM.Views;

public partial class MainView : UserControl
{
    public MainView()
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

    private void OpenCompetitions_Click(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenClubs_Click(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenNations_Click(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenNames_Click(object? sender, RoutedEventArgs e)
    {
    }

    private void OpenPeople_Click(object? sender, RoutedEventArgs e)
    {
    }
}