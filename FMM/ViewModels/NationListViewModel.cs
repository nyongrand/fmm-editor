using FMMLibrary;
using FMM.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FMM.ViewModels;

public class NationListViewModel : ViewModelBase
{
    private string? folderPath;
    private bool isDatabaseLoaded;
    private string searchQuery = string.Empty;
    private NationDisplayModel? selectedNation;

    public string? FolderPath
    {
        get => folderPath;
        set => this.RaiseAndSetIfChanged(ref folderPath, value);
    }

    public bool IsDatabaseLoaded
    {
        get => isDatabaseLoaded;
        set => this.RaiseAndSetIfChanged(ref isDatabaseLoaded, value);
    }

    public string SearchQuery
    {
        get => searchQuery;
        set => this.RaiseAndSetIfChanged(ref searchQuery, value);
    }

    public NationDisplayModel? SelectedNation
    {
        get => selectedNation;
        set => this.RaiseAndSetIfChanged(ref selectedNation, value);
    }

    public ObservableCollection<NationDisplayModel> Nations { get; } = new();

    public IEnumerable<NationDisplayModel> FilteredNations => string.IsNullOrWhiteSpace(SearchQuery)
        ? Nations
        : Nations.Where(n =>
            n.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.Nationality.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.CodeName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.ContinentName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.StadiumName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            n.RegionName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; }
    public ReactiveCommand<string?, Unit> LoadCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<string?, Unit> SaveAsCommand { get; }

    private NationParser? nationParser;
    private ContinentParser? continentParser;
    private StadiumParser? stadiumParser;
    private RegionParser? regionParser;

    private Dictionary<short, string> continentLookup = new();
    private Dictionary<short, string> stadiumLookup = new();
    private Dictionary<short, string> regionLookup = new();

    public NationListViewModel()
    {
        ClearSearchCommand = ReactiveCommand.Create(() => { SearchQuery = string.Empty; });
        LoadCommand = ReactiveCommand.CreateFromTask<string?, Unit>(async path =>
        {
            await LoadFromFolderAsync(path);
            return Unit.Default;
        });

        var canSave = this.WhenAnyValue(vm => vm.IsDatabaseLoaded);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
        SaveAsCommand = ReactiveCommand.CreateFromTask<string?, Unit>(async path =>
        {
            await SaveAsAsync(path);
            return Unit.Default;
        }, canSave);

        Nations.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredNations));

        this.WhenAnyValue(vm => vm.SearchQuery)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(FilteredNations)));
    [ReactiveCommand]
    private async Task CopyUid(Nation? nation)
    {
        if (nation != null)
        {
            // TODO: Use Avalonia clipboard service to copy nation uid to clipboard
        }
    }

    public async Task LoadFromFolderAsync(string? path)
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

        this.RaisePropertyChanged(nameof(FilteredNations));
    }

    private static async Task<TParser?> LoadOptional<TParser>(string filePath, Func<string, Task<TParser>> loader)
    {
        if (!File.Exists(filePath)) return default;
        return await loader(filePath);
    }
}
