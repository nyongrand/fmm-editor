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
    [Reactive] private bool _showEditDialog;
    [Reactive] private ClubEditViewModel? _editViewModel;
    [Reactive] private string _statusMessage = string.Empty;

    public ObservableCollection<ClubDisplayModel> ClubsList { get; } = [];
    public ObservableCollection<Nation> Nations { get; } = [];
    public ObservableCollection<Stadium> Stadiums { get; } = [];
    public ObservableCollection<Competition> Competitions { get; } = [];

    public IEnumerable<ClubDisplayModel> FilteredClubs => FilterBySearch(ClubsList);
    public bool HasSearchQuery => !string.IsNullOrWhiteSpace(SearchQuery);
    public bool IsDatabaseEmpty => !IsDatabaseLoaded;
    public bool IsEditingEnabled => IsDatabaseLoaded;

    private NationParser? nationParser;
    private ClubParser? clubParser;
    private CompetitionParser? competitionParser;
    private PeopleParser? peopleParser;
    private NameParser? firstNameParser;
    private NameParser? secondNameParser;
    private NameParser? commonNameParser;
    private StadiumParser? stadiumParser;
    private AlwaysLoadParser? alwaysLoadMaleParser;
    private AlwaysLoadParser? alwaysLoadFemaleParser;

    private Dictionary<short, string> nationLookup = [];
    private Dictionary<short, string> competitionLookup = [];
    private Dictionary<int, People> peopleLookup = [];
    private Dictionary<int, string> firstNameLookup = [];
    private Dictionary<int, string> lastNameLookup = [];
    private Dictionary<int, string> commonNameLookup = [];
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
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDatabaseEmpty));
                this.RaisePropertyChanged(nameof(IsEditingEnabled));
            });

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
        var peoplePath = Path.Combine(path, "people.dat");
        var firstNamePath = Path.Combine(path, "first_names.dat");
        var secondNamePath = Path.Combine(path, "second_names.dat");
        var commonNamePath = Path.Combine(path, "common_names.dat");
        var stadiumPath = Path.Combine(path, "stadium.dat");
        var alwaysLoadMalePath = Path.Combine(path, "clubs_to_always_load_male.dat");
        var alwaysLoadFemalePath = Path.Combine(path, "clubs_to_always_load_female.dat");

        if (!File.Exists(nationPath)
            || !File.Exists(clubPath)
            || !File.Exists(competitionPath)
            || !File.Exists(peoplePath)
            || !File.Exists(firstNamePath)
            || !File.Exists(secondNamePath)
            || !File.Exists(commonNamePath)
            || !File.Exists(stadiumPath)
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
            peopleParser = await PeopleParser.Load(peoplePath);
            firstNameParser = await NameParser.Load(firstNamePath);
            secondNameParser = await NameParser.Load(secondNamePath);
            commonNameParser = await NameParser.Load(commonNamePath);
            stadiumParser = await StadiumParser.Load(stadiumPath);
            alwaysLoadMaleParser = await AlwaysLoadParser.Load(alwaysLoadMalePath);
            alwaysLoadFemaleParser = await AlwaysLoadParser.Load(alwaysLoadFemalePath);

            FolderPath = path;
            IsDatabaseLoaded = true;

            nationLookup = nationParser.Items.ToDictionary(n => n.Id, n => n.Name);
            competitionLookup = competitionParser.Items.ToDictionary(c => c.Id, c => c.FullName);
            peopleLookup = peopleParser.Items.ToDictionary(p => p.Id, p => p);
            firstNameLookup = firstNameParser.Items.ToDictionary(n => n.Id, n => n.Value);
            lastNameLookup = secondNameParser.Items.ToDictionary(n => n.Id, n => n.Value);
            commonNameLookup = commonNameParser.Items.ToDictionary(n => n.Id, n => n.Value);
            alwaysLoadMaleUids = [.. alwaysLoadMaleParser.Items];
            alwaysLoadFemaleUids = [.. alwaysLoadFemaleParser.Items];

            Nations.Clear();
            foreach (var nation in nationParser.Items.OrderBy(n => n.Name))
                Nations.Add(nation);

            Competitions.Clear();
            foreach (var competition in competitionParser.Items.OrderBy(c => c.FullName))
                Competitions.Add(competition);

            Stadiums.Clear();
            foreach (var stadium in stadiumParser.Items.OrderBy(s => s.Name))
                Stadiums.Add(stadium);

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
        if (!IsDatabaseLoaded)
            return Task.CompletedTask;

        var editViewModel = CreateEditViewModel();
        editViewModel.InitializeForAdd();
        OpenEditDialog(editViewModel);
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private Task EditAsync(ClubDisplayModel? club)
    {
        if (!IsDatabaseLoaded || club == null)
            return Task.CompletedTask;

        var editViewModel = CreateEditViewModel();
        editViewModel.InitializeForEdit(club);
        OpenEditDialog(editViewModel);
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private Task CopyAsync(ClubDisplayModel? club)
    {
        if (!IsDatabaseLoaded || club == null)
            return Task.CompletedTask;

        var editViewModel = CreateEditViewModel();
        editViewModel.InitializeForCopy(club);
        OpenEditDialog(editViewModel);
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
        peopleParser = null;
        firstNameParser = null;
        secondNameParser = null;
        commonNameParser = null;
        stadiumParser = null;
        alwaysLoadMaleParser = null;
        alwaysLoadFemaleParser = null;

        nationLookup = [];
        competitionLookup = [];
        peopleLookup = [];
        firstNameLookup = [];
        lastNameLookup = [];
        commonNameLookup = [];
        alwaysLoadMaleUids = [];
        alwaysLoadFemaleUids = [];

        Nations.Clear();
        Competitions.Clear();
        Stadiums.Clear();
        ClubsList.Clear();

        IsDatabaseLoaded = false;
        SelectedClub = null;
        ShowEditDialog = false;
        EditViewModel = null;
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

    private ClubEditViewModel CreateEditViewModel()
    {
        return new ClubEditViewModel(Nations, Stadiums, Competitions, peopleLookup,
            firstNameLookup, lastNameLookup, commonNameLookup);
    }

    private void OpenEditDialog(ClubEditViewModel editViewModel)
    {
        EditViewModel = editViewModel;
        ShowEditDialog = true;

        editViewModel.CloseRequested += OnEditDialogClosed;
    }

    private void OnEditDialogClosed(ClubEditViewModel viewModel, bool accepted)
    {
        viewModel.CloseRequested -= OnEditDialogClosed;

        if (accepted && viewModel.Validate())
        {
            if (viewModel.IsAddMode)
                AddClub(viewModel);
            else
                UpdateClub(viewModel);

            RefreshClubsDisplay();
        }

        ShowEditDialog = false;
        EditViewModel = null;
    }

    private void AddClub(ClubEditViewModel vm)
    {
        if (clubParser == null)
            return;

        var nextUid = clubParser.Items.Count > 0 ? clubParser.Items.Max(x => x.Uid) + 1 : 1;

        var newClub = new Club
        {
            Id = -1,
            Uid = nextUid,
            FullName = vm.FullName ?? string.Empty,
            FullNameTerminator = vm.FullNameTerminator,
            ShortName = vm.ShortName ?? string.Empty,
            ShortNameTerminator = vm.ShortNameTerminator,
            SixLetterName = vm.SixLetterName ?? string.Empty,
            ThreeLetterName = vm.ThreeLetterName ?? string.Empty,
            BasedId = vm.BasedId ?? 0,
            NationId = vm.NationId!.Value,
            Status = vm.Status,
            Academy = vm.Academy ?? 10,
            Facilities = vm.Facilities ?? 10,
            AttAvg = vm.AttAvg ?? 0,
            AttMin = vm.AttMin ?? 0,
            AttMax = vm.AttMax ?? 0,
            Reserves = vm.Reserves ?? 0,
            LeagueId = vm.SelectedCompetition?.Id ?? vm.LeagueId ?? (short)-1,
            OtherDivision = vm.OtherDivision,
            OtherLastPosition = vm.OtherLastPosition,
            Stadium = vm.SelectedStadium != null ? (short)vm.SelectedStadium.Id : vm.Stadium ?? (short)-1,
            LastLeague = vm.SelectedLastLeague?.Id ?? vm.LastLeague ?? (short)-1,
            LeaguePos = vm.LeaguePos ?? 0,
            Reputation = vm.Reputation ?? 0,
            MainClub = vm.MainClub ?? -1,
            Type = vm.Type,
            Gender = vm.Gender,
            Colors = new System.Drawing.Color[6],
            Kits = new Kit[6],
            Affiliates = vm.Affiliates ?? [],
            Players = [],
            Unknown4Flag = vm.Unknown4Flag,
            Unknown4 = vm.Unknown4 ?? [],
            Unknown5 = vm.Unknown5 ?? [],
            Unknown6 = vm.Unknown6 ?? new byte[20],
            Unknown7 = vm.Unknown7 ?? new int[11],
            Unknown8 = vm.Unknown8 ?? new byte[33],
            Unknown9 = vm.Unknown9 ?? new byte[40]
        };

        newClub.Colors[0] = vm.Color1;
        newClub.Colors[1] = vm.Color2;
        newClub.Colors[2] = vm.Color3;
        newClub.Colors[3] = vm.Color4;
        newClub.Colors[4] = vm.Color5;
        newClub.Colors[5] = vm.Color6;

        for (int i = 0; i < 6; i++)
        {
            newClub.Kits[i] = vm.Kits?[i] ?? new Kit();
        }

        clubParser.Add(newClub);
        StatusMessage = "Club added successfully";
    }

    private void UpdateClub(ClubEditViewModel vm)
    {
        if (clubParser == null)
            return;

        var existingClub = clubParser.Items.FirstOrDefault(x => x.Uid == vm.Uid);
        if (existingClub == null)
            return;

        existingClub.Id = -1;
        existingClub.FullName = vm.FullName ?? string.Empty;
        existingClub.FullNameTerminator = vm.FullNameTerminator;
        existingClub.ShortName = vm.ShortName ?? string.Empty;
        existingClub.ShortNameTerminator = vm.ShortNameTerminator;
        existingClub.SixLetterName = vm.SixLetterName ?? string.Empty;
        existingClub.ThreeLetterName = vm.ThreeLetterName ?? string.Empty;
        existingClub.BasedId = vm.BasedId ?? 0;
        existingClub.NationId = vm.NationId!.Value;
        existingClub.Status = vm.Status;
        existingClub.Academy = vm.Academy ?? 10;
        existingClub.Facilities = vm.Facilities ?? 10;
        existingClub.AttAvg = vm.AttAvg ?? 0;
        existingClub.AttMin = vm.AttMin ?? 0;
        existingClub.AttMax = vm.AttMax ?? 0;
        existingClub.Reserves = vm.Reserves ?? 0;
        existingClub.LeagueId = vm.SelectedCompetition?.Id ?? vm.LeagueId ?? (short)-1;
        existingClub.OtherDivision = vm.OtherDivision;
        existingClub.OtherLastPosition = vm.OtherLastPosition;
        existingClub.Stadium = vm.SelectedStadium != null ? (short)vm.SelectedStadium.Id : vm.Stadium ?? (short)-1;
        existingClub.LastLeague = vm.SelectedLastLeague?.Id ?? vm.LastLeague ?? (short)-1;
        existingClub.LeaguePos = vm.LeaguePos ?? 0;
        existingClub.Reputation = vm.Reputation ?? 0;
        existingClub.MainClub = vm.MainClub ?? -1;
        existingClub.Type = vm.Type;
        existingClub.Gender = vm.Gender;

        existingClub.Colors[0] = vm.Color1;
        existingClub.Colors[1] = vm.Color2;
        existingClub.Colors[2] = vm.Color3;
        existingClub.Colors[3] = vm.Color4;
        existingClub.Colors[4] = vm.Color5;
        existingClub.Colors[5] = vm.Color6;

        for (int i = 0; i < 6; i++)
        {
            existingClub.Kits[i] = vm.Kits?[i] ?? new Kit();
        }

        existingClub.Affiliates = vm.Affiliates ?? [];

        existingClub.Unknown4Flag = vm.Unknown4Flag;
        existingClub.Unknown4 = vm.Unknown4 ?? [];
        existingClub.Unknown5 = vm.Unknown5 ?? [];
        existingClub.Unknown6 = vm.Unknown6 ?? new byte[20];
        existingClub.Unknown7 = vm.Unknown7 ?? new int[11];
        existingClub.Unknown8 = vm.Unknown8 ?? new byte[33];
        existingClub.Unknown9 = vm.Unknown9 ?? new byte[40];

        StatusMessage = "Club updated successfully";
    }
}
