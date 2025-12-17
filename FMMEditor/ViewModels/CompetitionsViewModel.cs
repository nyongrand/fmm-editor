using DynamicData;
using FMMEditor.Collections;
using FMMEditor.Models;
using FMMEditor.Views;
using FMMLibrary;
using MaterialDesignThemes.Wpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.ViewModels
{
    public class CompetitionsViewModel : ReactiveObject
    {
        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        [Reactive] public string SearchQuery { get; set; } = "";
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        // Parsers
        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser? CompParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }

        // Collections for display
        public BulkObservableCollection<Nation> Nations { get; } = [];
        public BulkObservableCollection<CompetitionDisplayModel> CompetitionsList { get; } = [];

        /// <summary>
        /// All clubs loaded from club.dat
        /// </summary>
        public ObservableCollection<Club> Clubs { get; }

        /// <summary>
        /// Clubs filtered by selected competition
        /// </summary>
        public ObservableCollection<Club> ClubsInSelectedCompetition { get; }

        /// <summary>
        /// Clubs filtered by selected nation for switching / moving
        /// </summary>
        public ObservableCollection<Club> ClubsAvailableForSwitch { get; }

        // Commands
        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseCompetitions { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClubs { get; private set; }

        // Commands for Add/Edit/Copy
        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<CompetitionDisplayModel, Unit> EditCommand { get; private set; }
        public ReactiveCommand<CompetitionDisplayModel, Unit> CopyCommand { get; private set; }

        [Reactive] public CompetitionDisplayModel? SelectedCompetition { get; set; }
        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        // Club management
        [Reactive] public bool ShowMoveDialog { get; set; }
        [Reactive] public bool ShowSwitchDialog { get; set; }
        [Reactive] public bool ShowDialog { get; set; }

        [Reactive] public Club? SelectedClub { get; set; }
        [Reactive] public Nation? FilterSwitchNation { get; set; }
        [Reactive] public Club? SwitchedWithClub { get; set; }

        public extern bool HasNoClubs { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, Unit> SwitchClub { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveClub { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveClub { get; private set; }

        public ReactiveCommand<Unit, Unit> CancelSwitch { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmSwitch { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmMove { get; private set; }

        private readonly ICollectionView competitionsView;
        private readonly IDialogService dialogService;

        // Lookup dictionaries for performance
        private Dictionary<short, string> nationLookup = [];
        private Dictionary<short, List<Club>> clubsByLeagueLookup = [];

        public CompetitionsViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; }, outputScheduler: RxApp.MainThreadScheduler);

            Clubs = [];
            ClubsInSelectedCompetition = [];
            ClubsAvailableForSwitch = [];

            competitionsView = CollectionViewSource.GetDefaultView(CompetitionsList);
            competitionsView.Filter = obj =>
            {
                if (obj is CompetitionDisplayModel competition)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || competition.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || competition.NationName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || competition.ShortName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }
                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseCompetitions = ReactiveCommand.CreateFromTask<string, CompetitionParser>(CompetitionParser.Load);
            ParseCompetitions.ToPropertyEx(this, vm => vm.CompParser);

            ParseClubs = ReactiveCommand.CreateFromTask<string, ClubParser>(ClubParser.Load);
            ParseClubs.ToPropertyEx(this, vm => vm.ClubParser);

            AddCommand = ReactiveCommand.CreateFromTask(OpenAddCompetitionDialogAsync);
            EditCommand = ReactiveCommand.CreateFromTask<CompetitionDisplayModel>(OpenEditCompetitionDialogAsync);
            CopyCommand = ReactiveCommand.CreateFromTask<CompetitionDisplayModel>(OpenCopyCompetitionDialogAsync);

            SwitchClub = ReactiveCommand.Create(SwitchClubImpl);
            MoveClub = ReactiveCommand.Create(MoveClubImpl);
            RemoveClub = ReactiveCommand.Create(RemoveClubImpl);

            CancelSwitch = ReactiveCommand.Create(CancelSwitchImpl);
            ConfirmMove = ReactiveCommand.Create(ConfirmMoveImpl);
            ConfirmSwitch = ReactiveCommand.Create(ConfirmSwitchImpl);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

            // Wire up folder path to parsers
            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\nation.dat")
                .InvokeCommand(ParseNations);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\competition.dat")
                .InvokeCommand(ParseCompetitions);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\club.dat")
                .InvokeCommand(ParseClubs);

            this.WhenAnyValue(vm => vm.FolderPath)
                .Select(x => !string.IsNullOrEmpty(x))
                .ToPropertyEx(this, vm => vm.IsDatabaseLoaded);

            // Build lookup dictionaries when parsers load
            this.WhenAnyValue(vm => vm.NationParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Nations.Reset(x.Items.OrderBy(y => y.Name));
                    nationLookup = x.Items.ToDictionary(n => n.Id, n => n.Name);
                    RefreshCompetitionsDisplay();
                });

            this.WhenAnyValue(vm => vm.CompParser)
                .WhereNotNull()
                .Subscribe(x => RefreshCompetitionsDisplay());

            this.WhenAnyValue(vm => vm.ClubParser)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    Clubs.Clear();
                    ClubsInSelectedCompetition.Clear();
                    ClubsAvailableForSwitch.Clear();
                    clubsByLeagueLookup.Clear();

                    var clubs = x?.Items.ToList();
                    if (clubs != null && clubs.Count != 0)
                    {
                        clubs.ForEach(c =>
                        {
                            c.Based = nationLookup.GetValueOrDefault(c.BasedId, "");
                            c.Nation = nationLookup.GetValueOrDefault(c.NationId, "");
                        });

                        Clubs.AddRange(clubs);

                        // Build index for fast lookup
                        clubsByLeagueLookup = clubs
                            .Where(c => c.LeagueId >= 0)
                            .GroupBy(c => c.LeagueId)
                            .ToDictionary(g => g.Key, g => g.ToList());
                    }

                    UpdateFilteredClubs();
                    UpdateFilteredSwitchs();
                });

            this.WhenAnyValue(vm => vm.ShowMoveDialog, vm => vm.ShowSwitchDialog, vm => vm.SelectedClub)
                .Select(x => (x.Item1 || x.Item2) && x.Item3 != null)
                .Subscribe(x => ShowDialog = x);

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => competitionsView.Refresh());

            this.WhenAnyValue(vm => vm.SelectedCompetition)
                .Subscribe(x => UpdateFilteredClubs());

            this.WhenAnyValue(vm => vm.SelectedCompetition, vm => vm.FilterSwitchNation)
                .Subscribe(x => UpdateFilteredSwitchs());

            this.WhenAnyValue(vm => vm.SelectedCompetition, vm => vm.ClubsInSelectedCompetition.Count)
                .Select(x => x.Item1 != null && x.Item2 == 0)
                .ToPropertyEx(this, vm => vm.HasNoClubs);
        }

        private async Task OpenAddCompetitionDialogAsync()
        {
            var viewModel = new CompetitionEditViewModel(Nations);
            viewModel.InitializeForAdd();

            var view = new CompetitionEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "CompetitionsDialogHost");

            if (result is CompetitionEditViewModel vm && vm.Validate())
            {
                AddCompetition(vm);
                RefreshCompetitionsDisplay();
            }
        }

        private async Task OpenEditCompetitionDialogAsync(CompetitionDisplayModel? competition)
        {
            if (competition == null) return;

            var viewModel = new CompetitionEditViewModel(Nations);
            viewModel.InitializeForEdit(competition);

            var view = new CompetitionEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "CompetitionsDialogHost");

            if (result is CompetitionEditViewModel vm && vm.Validate())
            {
                UpdateCompetition(vm);
                RefreshCompetitionsDisplay();
            }
        }

        private async Task OpenCopyCompetitionDialogAsync(CompetitionDisplayModel? competition)
        {
            if (competition == null) return;

            var viewModel = new CompetitionEditViewModel(Nations);
            viewModel.InitializeForCopy(competition);

            var view = new CompetitionEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "CompetitionsDialogHost");

            if (result is CompetitionEditViewModel vm && vm.Validate())
            {
                AddCompetition(vm);
                RefreshCompetitionsDisplay();
            }
        }

        private void AddCompetition(CompetitionEditViewModel vm)
        {
            if (CompParser == null) return;

            var nextId = CompParser.Items.Count > 0 ? (short)(CompParser.Items.Max(x => x.Id) + 1) : (short)1;
            var nextUid = CompParser.Items.Count > 0 ? CompParser.Items.Max(x => x.Uid) + 1 : 1;

            var newCompetition = new Competition(new BinaryReaderEx(new System.IO.MemoryStream()))
            {
                Id = nextId,
                Uid = nextUid,
                FullName = vm.FullName ?? "",
                FullNameTerminator = vm.FullNameTerminator,
                ShortName = vm.ShortName ?? "",
                ShortNameTerminator = vm.ShortNameTerminator,
                CodeName = vm.CodeName ?? "",
                Type = vm.Type,
                ContinentId = vm.ContinentId,
                NationId = vm.NationId ?? -1,
                ForegroundColor = vm.ForegroundColor,
                BackgroundColor = vm.BackgroundColor,
                Reputation = vm.Reputation,
                Level = vm.Level,
                ParentCompetitionId = vm.ParentCompetitionId,
                Qualifiers = vm.Qualifiers ?? [],
                Rank1 = vm.Rank1,
                Rank2 = vm.Rank2,
                Rank3 = vm.Rank3,
                Year1 = vm.Year1,
                Year2 = vm.Year2,
                Year3 = vm.Year3,
                Unknown3 = vm.Unknown3,
                IsWomen = vm.IsWomen
            };

            CompParser.Items.Add(newCompetition);
            MessageQueue.Enqueue("Competition added successfully");
        }

        private void UpdateCompetition(CompetitionEditViewModel vm)
        {
            var existingCompetition = CompParser?.Items.FirstOrDefault(x => x.Uid == vm.Uid);
            if (existingCompetition == null) return;

            existingCompetition.FullName = vm.FullName ?? "";
            existingCompetition.FullNameTerminator = vm.FullNameTerminator;
            existingCompetition.ShortName = vm.ShortName ?? "";
            existingCompetition.ShortNameTerminator = vm.ShortNameTerminator;
            existingCompetition.CodeName = vm.CodeName ?? "";
            existingCompetition.Type = vm.Type;
            existingCompetition.ContinentId = vm.ContinentId;
            existingCompetition.NationId = vm.NationId ?? -1;
            existingCompetition.ForegroundColor = vm.ForegroundColor;
            existingCompetition.BackgroundColor = vm.BackgroundColor;
            existingCompetition.Reputation = vm.Reputation;
            existingCompetition.Level = vm.Level;
            existingCompetition.ParentCompetitionId = vm.ParentCompetitionId;
            existingCompetition.Qualifiers = vm.Qualifiers ?? [];
            existingCompetition.Rank1 = vm.Rank1;
            existingCompetition.Rank2 = vm.Rank2;
            existingCompetition.Rank3 = vm.Rank3;
            existingCompetition.Year1 = vm.Year1;
            existingCompetition.Year2 = vm.Year2;
            existingCompetition.Year3 = vm.Year3;
            existingCompetition.Unknown3 = vm.Unknown3;
            existingCompetition.IsWomen = vm.IsWomen;

            MessageQueue.Enqueue("Competition updated successfully");
        }

        private void RefreshCompetitionsDisplay()
        {
            if (CompParser == null) return;

            var displayList = CompParser.Items.Select(c =>
            {
                var display = new CompetitionDisplayModel(c)
                {
                    NationName = nationLookup.GetValueOrDefault(c.NationId, "")
                };
                return display;
            }).OrderByDescending(x => x.Competition.NationId != -1)
              .ThenBy(x => x.IsWomen)
              .ThenBy(x => x.NationName)
              .ThenBy(x => x.Level)
              .ThenBy(x => x.Id);

            CompetitionsList.Reset(displayList);
        }

        private void UpdateFilteredClubs()
        {
            ClubsInSelectedCompetition.Clear();

            if (SelectedCompetition?.Id != null && clubsByLeagueLookup.TryGetValue(SelectedCompetition.Id, out var clubs))
            {
                ClubsInSelectedCompetition.AddRange(clubs);
            }
        }

        private void UpdateFilteredSwitchs()
        {
            ClubsAvailableForSwitch.Clear();

            if (FilterSwitchNation?.Id != null && SelectedCompetition?.Id != null)
            {
                var switchClubs = Clubs
                    .Where(x => x.IsNational == 0)
                    .Where(x => x.IsWomanFlag == SelectedClub?.IsWomanFlag)
                    .Where(c => c.NationId == FilterSwitchNation.Id && c.LeagueId != SelectedCompetition.Id)
                    .Select(c =>
                    {
                        c.Competition = GetCompetitionName(c.LeagueId);
                        return c;
                    })
                    .OrderByDescending(c => c.Reputation)
                    .ToList();

                ClubsAvailableForSwitch.AddRange(switchClubs);
            }
        }

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return (success == true) ? settings.SelectedPath : null;
        }

        private void SwitchClubImpl()
        {
            FilterSwitchNation = null;
            SwitchedWithClub = null;
            ShowMoveDialog = false;
            ShowSwitchDialog = true;
        }

        private void MoveClubImpl()
        {
            FilterSwitchNation = null;
            SwitchedWithClub = null;
            ShowMoveDialog = true;
            ShowSwitchDialog = false;
        }

        private void RemoveClubImpl()
        {
            if (SelectedClub is not null)
            {
                var oldLeagueId = SelectedClub.LeagueId;

                SelectedClub.BasedId = SelectedClub.NationId;
                SelectedClub.LeagueId = -1;

                // Update index
                if (oldLeagueId >= 0 && clubsByLeagueLookup.TryGetValue((short)oldLeagueId, out var clubs))
                {
                    clubs.Remove(SelectedClub);
                }

                UpdateFilteredClubs();
            }
        }

        private void CancelSwitchImpl()
        {
            FilterSwitchNation = null;
            SwitchedWithClub = null;
            ShowMoveDialog = false;
            ShowSwitchDialog = false;
        }

        private void ConfirmMoveImpl()
        {
            ShowMoveDialog = false;

            if (SelectedCompetition?.Competition is Competition comp && SwitchedWithClub is not null)
            {
                var oldLeagueId = SwitchedWithClub.LeagueId;

                SwitchedWithClub.BasedId = comp.NationId;
                SwitchedWithClub.LeagueId = comp.Id;

                // Update index
                if (oldLeagueId >= 0 && clubsByLeagueLookup.TryGetValue(oldLeagueId, out var oldClubs))
                {
                    oldClubs.Remove(SwitchedWithClub);
                }

                if (!clubsByLeagueLookup.TryGetValue(comp.Id, out var newClubs))
                {
                    newClubs = [];
                    clubsByLeagueLookup[comp.Id] = newClubs;
                }
                newClubs.Add(SwitchedWithClub);

                UpdateFilteredClubs();
                UpdateFilteredSwitchs();
            }
        }

        private void ConfirmSwitchImpl()
        {
            ShowSwitchDialog = false;

            if (SelectedClub is Club club1 && SwitchedWithClub is Club club2)
            {
                var based1 = club1.BasedId;
                var league1 = club1.LeagueId;
                var based2 = club2.BasedId;
                var league2 = club2.LeagueId;

                SelectedClub.BasedId = based2;
                SelectedClub.LeagueId = league2;
                SwitchedWithClub.BasedId = based1;
                SwitchedWithClub.LeagueId = league1;

                // Update index
                if (league1 >= 0 && clubsByLeagueLookup.TryGetValue((short)league1, out var clubs1))
                {
                    clubs1.Remove(club1);
                }
                if (league2 >= 0 && clubsByLeagueLookup.TryGetValue((short)league2, out var clubs2))
                {
                    clubs2.Remove(club2);
                    clubs2.Add(club1);
                }

                if (league1 >= 0)
                {
                    if (!clubsByLeagueLookup.TryGetValue((short)league1, out var newClubs1))
                    {
                        newClubs1 = [];
                        clubsByLeagueLookup[league1] = newClubs1;
                    }
                    newClubs1.Add(club2);
                }

                UpdateFilteredClubs();
                UpdateFilteredSwitchs();
            }
        }

        private async Task SaveImpl()
        {
            try
            {
                if (CompParser != null)
                {
                    CompParser.OriginalCount = (short)CompParser.Items.Count;
                    await CompParser.Save();
                }
                
                if (ClubParser != null)
                {
                    await ClubParser.Save();
                }
                
                MessageQueue.Enqueue("Save Successful");
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveAsImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            if (success != true) return;

            try
            {
                if (CompParser != null)
                {
                    CompParser.OriginalCount = (short)CompParser.Items.Count;
                    await CompParser.Save(settings.SelectedPath + "\\competition.dat");
                }
                
                if (ClubParser != null)
                {
                    await ClubParser.Save(settings.SelectedPath + "\\club.dat");
                }
                
                MessageQueue.Enqueue("Save Successful");
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetCompetitionName(int id)
        {
            return CompParser?.Items.FirstOrDefault(x => x.Id == id)?.FullName ?? "--";
        }
    }
}
