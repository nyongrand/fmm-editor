using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System.Linq;

namespace FMM.Views;

public partial class MainWindow : Window
{
    private readonly MainView _mainView;
    private readonly CompetitionListView _competitionListView = new();
    private readonly ClubListView _clubListView = new();
    private readonly NationListView _nationListView = new();
    private readonly NameListView _nameListView = new();
    private readonly PeopleListView _peopleListView = new();

    public MainWindow()
    {
        InitializeComponent();

        _mainView = new MainView();
        navigateView.Content = _mainView;
    }

    private void NavigationView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem item && item.Tag is string target)
        {
            NavigateTo(target, false);
        }
    }

    public void NavigateTo(string target, bool updateSelection = true)
    {
        navigateView.Content = target switch
        {
            "Competitions" => _competitionListView,
            "Clubs" => _clubListView,
            "Nations" => _nationListView,
            "Names" => _nameListView,
            "People" => _peopleListView,
            _ => _mainView
        };

        if (updateSelection)
        {
            var match = navigateView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(i => i.Tag is string tag && tag == target);

            if (match != null && navigateView.SelectedItem != match)
            {
                navigateView.SelectedItem = match;
            }
        }
    }
}