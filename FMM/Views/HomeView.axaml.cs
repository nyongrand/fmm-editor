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

    private void OpenCompetitions_Click(object? sender, RoutedEventArgs e)
    {
        NavigateFromDashboard("Competitions");
    }

    private void OpenClubs_Click(object? sender, RoutedEventArgs e)
    {
        NavigateFromDashboard("Clubs");
    }

    private void OpenNations_Click(object? sender, RoutedEventArgs e)
    {
        NavigateFromDashboard("Nations");
    }

    private void OpenNames_Click(object? sender, RoutedEventArgs e)
    {
        NavigateFromDashboard("Names");
    }

    private void OpenPeople_Click(object? sender, RoutedEventArgs e)
    {
        NavigateFromDashboard("People");
    }

    private void NavigateFromDashboard(string target)
    {
    }
}