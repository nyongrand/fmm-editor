using FMM.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FMM.ViewModels;

public partial class NationListViewModel : ViewModelBase
{
    [Reactive]
    private string? _folderPath = null;

    [Reactive]
    private bool _isDatabaseLoaded = false;

    [Reactive]
    private string _searchQuery = string.Empty;

    [Reactive]
    private NationDisplayModel? _selectedNation = null;

    public ObservableCollection<NationDisplayModel> Nations { get; } = [];

    public IEnumerable<NationDisplayModel> FilteredNations => string.IsNullOrWhiteSpace(SearchQuery)
        ? Nations
        : Nations.Where(n =>
            n.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.Nationality.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.CodeName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.ContinentName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.StadiumName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.RegionName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

    private NationParser? nationParser;
    private ContinentParser? continentParser;
    private StadiumParser? stadiumParser;
    private RegionParser? regionParser;

    private Dictionary<short, string> continentLookup = [];
    private Dictionary<short, string> stadiumLookup = [];
    private Dictionary<short, string> regionLookup = [];

    readonly IObservable<bool> _canSave;

    public NationListViewModel()
    {
        _canSave = this.WhenAnyValue(vm => vm.IsDatabaseLoaded);

        Nations.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredNations));

        this.WhenAnyValue(vm => vm.SearchQuery)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(FilteredNations)));

        this.WhenAnyValue(vm => vm.FolderPath)
            .WhereNotNull()
            .InvokeCommand(LoadCommand);
    }

    [ReactiveCommand]
    private async Task CopyUid(Nation? nation)
    {
        if (nation != null)
        {
            // TODO: Use Avalonia clipboard service to copy nation uid to clipboard
        }
    }

    [ReactiveCommand]
    private async Task CopyUidHex(Nation? nation)
    {
        if (nation != null)
        {
            byte[] bytes = BitConverter.GetBytes(nation.Uid);
            string hex = Convert.ToHexString(bytes);
            // TODO: Use Avalonia clipboard service to copy nation uid to clipboard
        }
    }

    [ReactiveCommand]
    private void ClearSearch() => SearchQuery = string.Empty;

    [ReactiveCommand]
    public async Task LoadAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var nationPath = Path.Combine(path, "nation.dat");
        if (!File.Exists(nationPath)) return;

        nationParser = await NationParser.Load(nationPath);
        continentParser = await LoadOptional(Path.Combine(path, "continent.dat"), ContinentParser.Load);
        stadiumParser = await LoadOptional(Path.Combine(path, "stadium.dat"), StadiumParser.Load);
        regionParser = await LoadOptional(Path.Combine(path, "regions.dat"), RegionParser.Load);

        continentLookup = continentParser?.Items.ToDictionary(c => c.Id, c => c.Name) ?? [];
        stadiumLookup = stadiumParser?.Items.ToDictionary(s => (short)s.Id, s => s.Name) ?? [];
        regionLookup = regionParser?.Items.ToDictionary(r => r.Id, r => r.Name) ?? [];

        FolderPath = path;
        IsDatabaseLoaded = true;

        RefreshNations();
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsync()
    {
        if (!IsDatabaseLoaded || nationParser == null) return;
        await nationParser.Save();
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsAsync(string? path)
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

        this.RaisePropertyChanged(nameof(FilteredNations));
    }

    private static async Task<TParser?> LoadOptional<TParser>(string filePath, Func<string, Task<TParser>> loader)
    {
        if (!File.Exists(filePath)) return default;
        return await loader(filePath);
    }
}
