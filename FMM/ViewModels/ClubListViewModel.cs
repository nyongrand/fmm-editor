using FMM.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FMM.ViewModels;

public partial class ClubListViewModel : ViewModelBase
{
    [Reactive] private string? _folderPath;
    [Reactive] private bool _isDatabaseLoaded;
    [Reactive] private string _searchQuery = string.Empty;
    [Reactive] private ClubDisplayModel? _selectedClub;
    [Reactive] private string _statusMessage = string.Empty;

    public ObservableCollection<ClubDisplayModel> ClubsList { get; } = [];
    public ObservableCollection<Nation> Nations { get; } = [];
    public ObservableCollection<Competition> Competitions { get; } = [];

    public IEnumerable<ClubDisplayModel> FilteredClubs => FilterBySearch(ClubsList);
    public bool HasSearchQuery => !string.IsNullOrWhiteSpace(SearchQuery);
    public bool IsDatabaseEmpty => !IsDatabaseLoaded;
    public bool IsEditingEnabled => false;

    private NationParser? nationParser;
    private ClubParser? clubParser;
    private CompetitionParser? competitionParser;
    private AlwaysLoadParser? alwaysLoadMaleParser;
    private AlwaysLoadParser? alwaysLoadFemaleParser;

    private Dictionary<short, string> nationLookup = [];
    private Dictionary<short, string> competitionLookup = [];
    private HashSet<int> alwaysLoadMaleUids = [];
    private HashSet<int> alwaysLoadFemaleUids = [];

    readonly IObservable<bool> _canSave;

    public ClubListViewModel()
    {
        _canSave = this.WhenAnyValue(vm => vm.IsDatabaseLoaded);

        ClubsList.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(FilteredClubs));

        this.WhenAnyValue(vm => vm.SearchQuery)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(FilteredClubs));
                this.RaisePropertyChanged(nameof(HasSearchQuery));
            });

        this.WhenAnyValue(vm => vm.IsDatabaseLoaded)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(IsDatabaseEmpty)));

        this.WhenAnyValue(vm => vm.FolderPath)
            .WhereNotNull()
            .InvokeCommand(LoadCommand);
    }

    private IEnumerable<ClubDisplayModel> FilterBySearch(ObservableCollection<ClubDisplayModel> clubs)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return clubs;

        return clubs.Where(club =>
            club.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            club.ShortName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            club.NationName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
            club.CompetitionName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
    }

    [ReactiveCommand]
    private void ClearSearch() => SearchQuery = string.Empty;

    [ReactiveCommand]
    public async Task LoadAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var nationPath = Path.Combine(path, "nation.dat");
        var clubPath = Path.Combine(path, "club.dat");
        var competitionPath = Path.Combine(path, "competition.dat");
        var alwaysLoadMalePath = Path.Combine(path, "clubs_to_always_load_male.dat");
        var alwaysLoadFemalePath = Path.Combine(path, "clubs_to_always_load_female.dat");

        if (!File.Exists(nationPath)
            || !File.Exists(clubPath)
            || !File.Exists(competitionPath)
            || !File.Exists(alwaysLoadMalePath)
            || !File.Exists(alwaysLoadFemalePath))
        {
            StatusMessage = "Missing required club database files.";
            FolderPath = path;
            ClearData();
            return;
        }

        try
        {
            nationParser = await NationParser.Load(nationPath);
            clubParser = await ClubParser.Load(clubPath);
            competitionParser = await CompetitionParser.Load(competitionPath);
            alwaysLoadMaleParser = await AlwaysLoadParser.Load(alwaysLoadMalePath);
            alwaysLoadFemaleParser = await AlwaysLoadParser.Load(alwaysLoadFemalePath);

            FolderPath = path;
            IsDatabaseLoaded = true;

            nationLookup = nationParser.Items.ToDictionary(n => n.Id, n => n.Name);
            competitionLookup = competitionParser.Items.ToDictionary(c => c.Id, c => c.FullName);
            alwaysLoadMaleUids = [.. alwaysLoadMaleParser.Items];
            alwaysLoadFemaleUids = [.. alwaysLoadFemaleParser.Items];

            Nations.Clear();
            foreach (var nation in nationParser.Items.OrderBy(n => n.Name))
                Nations.Add(nation);

            Competitions.Clear();
            foreach (var competition in competitionParser.Items.OrderBy(c => c.FullName))
                Competitions.Add(competition);

            RefreshClubsDisplay();
            StatusMessage = "Database loaded";
        }
        catch (Exception ex)
        {
            FolderPath = path;
            ClearData();
            StatusMessage = $"Load error: {ex.Message}";
        }
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsync()
    {
        if (!IsDatabaseLoaded || clubParser == null)
            return;

        try
        {
            await clubParser.Save();
            if (alwaysLoadMaleParser != null)
                await SaveAlwaysLoadParser(alwaysLoadMaleParser, alwaysLoadMaleUids);
            if (alwaysLoadFemaleParser != null)
                await SaveAlwaysLoadParser(alwaysLoadFemaleParser, alwaysLoadFemaleUids);

            StatusMessage = "Save successful";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    [ReactiveCommand(CanExecute = nameof(_canSave))]
    private async Task SaveAsAsync(string? path)
    {
        if (!IsDatabaseLoaded || clubParser == null || string.IsNullOrWhiteSpace(path))
            return;

        try
        {
            await clubParser.Save(Path.Combine(path, "club.dat"));
            if (alwaysLoadMaleParser != null)
                await SaveAlwaysLoadParser(alwaysLoadMaleParser, alwaysLoadMaleUids,
                    Path.Combine(path, "clubs_to_always_load_male.dat"));
            if (alwaysLoadFemaleParser != null)
                await SaveAlwaysLoadParser(alwaysLoadFemaleParser, alwaysLoadFemaleUids,
                    Path.Combine(path, "clubs_to_always_load_female.dat"));

            StatusMessage = "Save successful";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    [ReactiveCommand]
    private Task AddAsync()
    {
        StatusMessage = "Editing not implemented yet.";
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private Task EditAsync(ClubDisplayModel? club)
    {
        StatusMessage = "Editing not implemented yet.";
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private Task CopyAsync(ClubDisplayModel? club)
    {
        StatusMessage = "Editing not implemented yet.";
        return Task.CompletedTask;
    }

    private void RefreshClubsDisplay()
    {
        if (clubParser == null)
            return;

        ClubsList.Clear();

        foreach (var club in clubParser.Items)
        {
            var isMale = club.Gender == 0;
            var display = new ClubDisplayModel(club)
            {
                NationName = nationLookup.GetValueOrDefault(club.NationId, string.Empty),
                BasedName = nationLookup.GetValueOrDefault(club.BasedId, string.Empty),
                CompetitionName = competitionLookup.GetValueOrDefault(club.LeagueId, string.Empty),
                IsAlwaysLoad = isMale ? alwaysLoadMaleUids.Contains(club.Uid) : alwaysLoadFemaleUids.Contains(club.Uid)
            };

            display.PropertyChanged += OnClubPropertyChanged;
            ClubsList.Add(display);
        }

        this.RaisePropertyChanged(nameof(FilteredClubs));
    }

    private void OnClubPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ClubDisplayModel.IsAlwaysLoad) && sender is ClubDisplayModel club)
        {
            UpdateAlwaysLoadSet(club);
        }
    }

    private void UpdateAlwaysLoadSet(ClubDisplayModel club)
    {
        var isMale = club.Gender == 0;
        var targetSet = isMale ? alwaysLoadMaleUids : alwaysLoadFemaleUids;

        if (club.IsAlwaysLoad)
            targetSet.Add(club.Uid);
        else
            targetSet.Remove(club.Uid);
    }

    private void ClearData()
    {
        nationParser = null;
        clubParser = null;
        competitionParser = null;
        alwaysLoadMaleParser = null;
        alwaysLoadFemaleParser = null;

        nationLookup = [];
        competitionLookup = [];
        alwaysLoadMaleUids = [];
        alwaysLoadFemaleUids = [];

        Nations.Clear();
        Competitions.Clear();
        ClubsList.Clear();

        IsDatabaseLoaded = false;
        SelectedClub = null;
    }

    private static async Task SaveAlwaysLoadParser(AlwaysLoadParser parser, HashSet<int> uids, string? filePath = null)
    {
        var bytes = ToAlwaysLoadBytes(parser, uids);
        await File.WriteAllBytesAsync(filePath ?? parser.FilePath, bytes);
    }

    private static byte[] ToAlwaysLoadBytes(AlwaysLoadParser parser, HashSet<int> uids)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriterEx(stream);

        writer.Write(parser.Header);
        writer.Write(uids.Count);
        writer.Write(0);

        foreach (var uid in uids.OrderBy(x => x))
            writer.Write(uid);

        return stream.ToArray();
    }
}
