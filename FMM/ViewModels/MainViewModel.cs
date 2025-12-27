using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FMM.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly HomeViewModel _homeViewModel = new();
    private readonly CompetitionListViewModel _competitionListViewModel = new();
    private readonly ClubListViewModel _clubListViewModel = new();
    private readonly NationListViewModel _nationListViewModel = new();
    private readonly NameListViewModel _nameListViewModel = new();
    private readonly PeopleListViewModel _peopleListViewModel = new();

    [Reactive] private NavigationItem? _selectedNavigationItem;
    [Reactive] private ViewModelBase _currentViewModel = null!;

    public IReadOnlyList<NavigationItem> NavigationItems { get; }

    public MainViewModel()
    {
        NavigationItems =
        [
            new NavigationItem("Competitions", "Competitions"),
            new NavigationItem("Clubs", "Clubs"),
            new NavigationItem("Nations", "Nations"),
            new NavigationItem("Names", "Names"),
            new NavigationItem("People", "People")
        ];

        _homeViewModel.NavigateCommand = NavigateCommand;

        CurrentViewModel = _homeViewModel;

        this.WhenAnyValue(x => x.SelectedNavigationItem)
            .Where(item => item?.Target is not null)
            .Subscribe(item => Navigate(item!.Target));
    }

    [ReactiveCommand]
    private void Navigate(string target)
    {
        CurrentViewModel = target switch
        {
            "Competitions" => _competitionListViewModel,
            "Clubs" => _clubListViewModel,
            "Nations" => _nationListViewModel,
            "Names" => _nameListViewModel,
            "People" => _peopleListViewModel,
            _ => _homeViewModel
        };

        // Update selected navigation item if needed
        var selectedNavigationItem = NavigationItems.FirstOrDefault(i => i.Target == target);
        if (SelectedNavigationItem?.Target != selectedNavigationItem?.Target)
            SelectedNavigationItem = NavigationItems.FirstOrDefault(i => i.Target == target);
    }
}

public record NavigationItem(string Label, string Target);
