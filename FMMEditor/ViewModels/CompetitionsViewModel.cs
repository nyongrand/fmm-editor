using DynamicData;
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
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        [Reactive] public string SearchQuery { get; set; } = "";
        [Reactive] public Competition? SelectedCompetition { get; set; }
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

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

        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser? CompParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }

        /// <summary>
        /// All nations loaded from nation.dat
        /// </summary>
        public ObservableCollection<Nation> Nations { get; }

        /// <summary>
        /// All competitions loaded from competition.dat
        /// </summary>
        public ObservableCollection<Competition> Competitions { get; }

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

        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseCompetitions { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClubs { get; private set; }

        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView compsView;
        private readonly IDialogService dialogService;
        private Dictionary<short, List<Club>> clubsByLeagueLookup = [];

        public CompetitionsViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            Nations = [];

            Competitions = [];
            compsView = CollectionViewSource.GetDefaultView(Competitions);
            compsView.Filter = obj =>
            {
                if (obj is Competition competition)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                    || competition.FullName.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase)
                    || competition.Nation.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase);
                }

                return true;
            };

            Clubs = [];
            ClubsInSelectedCompetition = [];
            ClubsAvailableForSwitch = [];

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseCompetitions = ReactiveCommand.CreateFromTask<string, CompetitionParser>(CompetitionParser.Load);
            ParseCompetitions.ToPropertyEx(this, vm => vm.CompParser);

            ParseClubs = ReactiveCommand.CreateFromTask<string, ClubParser>(ClubParser.Load);
            ParseClubs.ToPropertyEx(this, vm => vm.ClubParser);

            SwitchClub = ReactiveCommand.Create(SwitchClubImpl);
            MoveClub = ReactiveCommand.Create(MoveClubImpl);
            RemoveClub = ReactiveCommand.Create(RemoveClubImpl);

            CancelSwitch = ReactiveCommand.Create(CancelSwitchImpl);
            ConfirmMove = ReactiveCommand.Create(ConfirmMoveImpl);
            ConfirmSwitch = ReactiveCommand.Create(ConfirmSwitchImpl);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

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

            this.WhenAnyValue(vm => vm.NationParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Nations.Clear();
                    Nations.AddRange(x.Items.OrderBy(y => y.Name));
                });

            this.WhenAnyValue(vm => vm.CompParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    Competitions.Clear();

                    var competitions = pair.Item1?.Items;
                    if (competitions != null)
                    {
                        competitions.ForEach(x =>
                        {
                            x.Nation = GetNationName(pair.Item2?.Items, x.NationId);
                        });

                        Competitions.AddRange(
                            competitions
                                .OrderByDescending(x => x.NationId != -1)
                                .ThenBy(x => x.IsWomen)
                                .ThenBy(x => x.Nation)
                                .ThenBy(x => x.Level)
                                .ThenBy(x => x.Id)
                        );
                    }
                });

            this.WhenAnyValue(vm => vm.ClubParser, vm => vm.NationParser)
                .Where(pair => pair.Item1 != null)
                .Subscribe(pair =>
                {
                    Clubs.Clear();
                    ClubsInSelectedCompetition.Clear();
                    ClubsAvailableForSwitch.Clear();
                    clubsByLeagueLookup.Clear();

                    var clubs = pair.Item1?.Items;
                    if (clubs != null && clubs.Count != 0)
                    {
                        clubs.ForEach(x =>
                        {
                            x.Based = GetNationName(pair.Item2?.Items, x.BasedId);
                            x.Nation = GetNationName(pair.Item2?.Items, x.NationId);
                        });

                        Clubs.AddRange(clubs);

                        // Build index for fast lookup
                        clubsByLeagueLookup = clubs
                            .Where(c => c.LeagueId >= 0)
                            .GroupBy(c => c.LeagueId)
                            .ToDictionary(g => g.Key, g => g.ToList());
                    }

                    // Trigger initial filter
                    UpdateFilteredClubs();
                    UpdateFilteredSwitchs();
                });

            this.WhenAnyValue(vm => vm.ShowMoveDialog, vm => vm.ShowSwitchDialog, vm => vm.SelectedClub)
                .Select(x => (x.Item1 || x.Item2) && x.Item3 != null)
                .Subscribe(x => ShowDialog = x);

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => compsView.Refresh());

            this.WhenAnyValue(vm => vm.SelectedCompetition)
                .Subscribe(x => UpdateFilteredClubs());

            this.WhenAnyValue(vm => vm.SelectedCompetition, vm => vm.FilterSwitchNation)
                .Subscribe(x => UpdateFilteredSwitchs());

            this.WhenAnyValue(vm => vm.SelectedCompetition, vm => vm.ClubsInSelectedCompetition.Count)
                .Select(x => x.Item1 != null && x.Item2 == 0)
                .ToPropertyEx(this, vm => vm.HasNoClubs);
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
                        c.Competition = GetCompetitionName(Competitions, c.LeagueId);
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

            if (SelectedCompetition is Competition comp && SwitchedWithClub is not null)
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
                if (CompParser != null && ClubParser != null)
                {
                    CompParser.OriginalCount = (short)Competitions.Count;
                    CompParser.Items = [.. Competitions];
                    await CompParser.Save();

                    ClubParser.OriginalCount = Clubs.Count;
                    ClubParser.Items = [.. Clubs];
                    await ClubParser.Save();

                    MessageQueue.Enqueue("Save Successfull");
                }
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
                if (CompParser != null && ClubParser != null)
                {
                    CompParser.OriginalCount = (short)Competitions.Count;
                    CompParser.Items = [.. Competitions];
                    await CompParser.Save(settings.SelectedPath + "\\competition.dat");

                    ClubParser.OriginalCount = Clubs.Count;
                    ClubParser.Items = [.. Clubs];
                    await ClubParser.Save(settings.SelectedPath + "\\club.dat");

                    MessageQueue.Enqueue("Save Successfull");
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Private Methods

        private static string GetCompetitionName(IEnumerable<Competition>? competitions, int id)
        {
            return competitions?.FirstOrDefault(x => x.Id == id)?.FullName ?? "--";
        }

        private static string GetNationName(IEnumerable<Nation>? nations, int id)
        {
            return nations?.FirstOrDefault(x => x.Id == id)?.Name ?? "--";
        }

        #endregion
    }
}
