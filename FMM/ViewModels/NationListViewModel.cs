using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMMLibrary;
using FMM.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FMM.ViewModels;

public partial class NationListViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? folderPath;

    [ObservableProperty]
    private bool isDatabaseLoaded;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private NationDisplayModel? selectedNation;

    public ObservableCollection<NationDisplayModel> Nations { get; } = new();

    public IEnumerable<NationDisplayModel> FilteredNations => string.IsNullOrWhiteSpace(SearchQuery)
        ? Nations
        : Nations.Where(n =>
            n.Name.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase) ||
            n.Nationality.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase) ||
            n.CodeName.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase) ||
            n.ContinentName.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase) ||
            n.StadiumName.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase) ||
            n.RegionName.Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase));

    public IRelayCommand ClearSearchCommand { get; }
    public IAsyncRelayCommand<string?> LoadCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand<string?> SaveAsCommand { get; }

    private NationParser? nationParser;
    private ContinentParser? continentParser;
    private StadiumParser? stadiumParser;
    private RegionParser? regionParser;

    private Dictionary<short, string> continentLookup = new();
    private Dictionary<short, string> stadiumLookup = new();
    private Dictionary<short, string> regionLookup = new();

    public NationListViewModel()
    {
        ClearSearchCommand = new RelayCommand(() => SearchQuery = string.Empty);
        LoadCommand = new AsyncRelayCommand<string?>(LoadFromFolderAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => IsDatabaseLoaded);
        SaveAsCommand = new AsyncRelayCommand<string?>(SaveAsAsync, path => IsDatabaseLoaded);

        Nations.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FilteredNations));
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchQuery))
            {
                OnPropertyChanged(nameof(FilteredNations));
            }

            if (e.PropertyName == nameof(IsDatabaseLoaded))
            {
                SaveCommand.NotifyCanExecuteChanged();
                SaveAsCommand.NotifyCanExecuteChanged();
            }
        };
    }

    public async Task LoadFromFolderAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var nationPath = Path.Combine(path, "nation.dat");
        if (!File.Exists(nationPath)) return;

        nationParser = await NationParser.Load(nationPath);
        continentParser = await LoadOptional(Path.Combine(path, "continent.dat"), ContinentParser.Load);
        stadiumParser = await LoadOptional(Path.Combine(path, "stadium.dat"), StadiumParser.Load);
        regionParser = await LoadOptional(Path.Combine(path, "regions.dat"), RegionParser.Load);

        continentLookup = continentParser?.Items.ToDictionary(c => c.Id, c => c.Name) ?? new();
        stadiumLookup = stadiumParser?.Items.ToDictionary(s => (short)s.Id, s => s.Name) ?? new();
        regionLookup = regionParser?.Items.ToDictionary(r => r.Id, r => r.Name) ?? new();

        FolderPath = path;
        IsDatabaseLoaded = true;

        RefreshNations();
    }

    public async Task SaveAsync()
    {
        if (!IsDatabaseLoaded || nationParser == null) return;
        await nationParser.Save();
    }

    public async Task SaveAsAsync(string? path)
    {
        if (!IsDatabaseLoaded || nationParser == null || string.IsNullOrWhiteSpace(path)) return;
        var target = Path.Combine(path, "nation.dat");
        await nationParser.Save(target);
    }

    private void RefreshNations()
    {
        Nations.Clear();
        if (nationParser == null) return;

        foreach (var nation in nationParser.Items.OrderBy(n => n.Name))
        {
            Nations.Add(new NationDisplayModel(nation)
            {
                ContinentName = continentLookup.GetValueOrDefault(nation.ContinentId, "-"),
                StadiumName = stadiumLookup.GetValueOrDefault(nation.StadiumId, "-"),
                RegionName = regionLookup.GetValueOrDefault(nation.Region, "-")
            });
        }

        OnPropertyChanged(nameof(FilteredNations));
    }

    private static async Task<TParser?> LoadOptional<TParser>(string filePath, Func<string, Task<TParser>> loader)
    {
        if (!File.Exists(filePath)) return default;
        return await loader(filePath);
    }
}
