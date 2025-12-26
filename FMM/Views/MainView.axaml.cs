using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using System.Linq;

namespace FMM.Views;

public partial class MainView : UserControl
{
    private readonly HomeView _homeView;
    private readonly CompetitionListView _competitionListView = new();
    private readonly ClubListView _clubListView = new();
    private readonly NationListView _nationListView = new();
    private readonly NameListView _nameListView = new();
    private readonly PeopleListView _peopleListView = new();

    public MainView()
    {
        InitializeComponent();

        _homeView = new HomeView();
        navigateView.Content = _homeView;
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
            _ => _homeView
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