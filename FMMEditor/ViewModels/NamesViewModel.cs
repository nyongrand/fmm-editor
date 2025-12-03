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
using System.Xml.Linq;

namespace FMMEditor.ViewModels
{
    public class NamesViewModel : ReactiveObject
    {
        [Reactive] public string SearchQuery { get; set; } = "";
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        //[Reactive] public bool ShowMoveDialog { get; set; }
        //[Reactive] public bool ShowSwitchDialog { get; set; }

        //[Reactive] public Club? SelectedClub { get; set; }
        //[Reactive] public Nation? FilterSwitchNation { get; set; }
        //[Reactive] public Club? SwitchedWithClub { get; set; }

        //public extern bool HasNoClubs { [ObservableAsProperty] get; }

        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern NameParser? FirstNameParser { [ObservableAsProperty] get; }
        public extern NameParser? SecondNameParser { [ObservableAsProperty] get; }

        /// <summary>
        /// All nations loaded from nation.dat
        /// </summary>
        public ObservableCollection<Nation> Nations { get; } = [];

        /// <summary>
        /// All competitions loaded from competition.dat
        /// </summary>
        public ObservableCollection<Name> FirstNames { get; } = [];

        /// <summary>
        /// All clubs loaded from club.dat
        /// </summary>
        public ObservableCollection<Name> SecondNames { get; } = [];

        /// <summary>
        /// Clubs filtered by selected competition
        /// </summary>
        //public ObservableCollection<Club> ClubsInSelectedCompetition { get; }

        /// <summary>
        /// Clubs filtered by selected nation for switching / moving
        /// </summary>
        //public ObservableCollection<Club> ClubsAvailableForSwitch { get; }

        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, NameParser> ParseFirstName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseSecondName { get; private set; }

        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView firstNamesView;
        private readonly ICollectionView secondNamesView;
        private readonly IDialogService dialogService;

        #region

        public ReactiveCommand<Name, Unit> AddCommand { get; private set; }
        public ReactiveCommand<Name, Unit> EditCommand { get; private set; }
        public ReactiveCommand<Name, Unit> DeleteCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; private set; }
        //public ReactiveCommand<Unit, Unit> ConfirmMove { get; private set; }

        [Reactive] public bool ShowDialog { get; set; }
        [Reactive] public Name? SelectedName { get; set; }
        [Reactive] public int? SelectedId { get; set; }
        [Reactive] public byte? SelectedGender { get; set; }
        [Reactive] public int? SelectedNationUid { get; set; }
        [Reactive] public byte? SelectedUnknown3 { get; set; }
        [Reactive] public string? SelectedValue { get; set; }

        #endregion

        public NamesViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            firstNamesView = CollectionViewSource.GetDefaultView(FirstNames);
            firstNamesView.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            };

            secondNamesView = CollectionViewSource.GetDefaultView(SecondNames);
            secondNamesView.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || name.Value.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseFirstName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseFirstName.ToPropertyEx(this, vm => vm.FirstNameParser);

            ParseSecondName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseSecondName.ToPropertyEx(this, vm => vm.SecondNameParser);

            AddCommand = ReactiveCommand.Create<Name>(Add);
            EditCommand = ReactiveCommand.Create<Name>(Edit);
            DeleteCommand = ReactiveCommand.Create<Name>(Delete);

            CancelCommand = ReactiveCommand.Create(Cancel);
            ConfirmCommand = ReactiveCommand.Create(Confirm);
            //ConfirmSwitch = ReactiveCommand.Create(ConfirmSwitchImpl);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\first_names.dat")
                .InvokeCommand(ParseFirstName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\second_names.dat")
                .InvokeCommand(ParseSecondName);

            //this.WhenAnyValue(vm => vm.FolderPath)
            //    .WhereNotNull()
            //    .Select(x => x + "\\common_names.dat")
            //    .InvokeCommand(ParseCompetitions);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\nation.dat")
                .InvokeCommand(ParseNations);

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

            this.WhenAnyValue(vm => vm.FirstNameParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    FirstNames.Clear();

                    var names = pair.Item1?.Items;
                    if (names != null)
                    {
                        FirstNames.AddRange(
                            names.OrderBy(x => x.NationUid)
                                .ThenBy(x => x.Value)
                        );
                    }
                });

            this.WhenAnyValue(vm => vm.SecondNameParser, vm => vm.NationParser)
                .Subscribe(pair =>
                {
                    SecondNames.Clear();

                    var names = pair.Item1?.Items;
                    if (names != null)
                    {
                        SecondNames.AddRange(
                            names.OrderBy(x => x.NationUid)
                                .ThenBy(x => x.Value)
                        );
                    }
                });

            //this.WhenAnyValue(vm => vm.ShowMoveDialog, vm => vm.ShowSwitchDialog, vm => vm.SelectedClub)
            //    .Select(x => (x.Item1 || x.Item2) && x.Item3 != null)
            //    .Subscribe(x => ShowDialog = x);

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    firstNamesView.Refresh();
                    secondNamesView.Refresh();
                });

            //this.WhenAnyValue(vm => vm.SelectedName)
            //    .Subscribe(x => UpdateFilteredClubs());

            //this.WhenAnyValue(vm => vm.SelectedName, vm => vm.FilterSwitchNation)
            //    .Subscribe(x => UpdateFilteredSwitchs());

            //this.WhenAnyValue(vm => vm.SelectedName, vm => vm.ClubsInSelectedCompetition.Count)
            //    .Select(x => x.Item1 != null && x.Item2 == 0)
            //    .ToPropertyEx(this, vm => vm.HasNoClubs);
        }

        //private void UpdateFilteredClubs()
        //{
        //    ClubsInSelectedCompetition.Clear();

        //    if (SelectedName?.Id != null && clubsByLeagueLookup.TryGetValue(SelectedName.Id, out var clubs))
        //    {
        //        ClubsInSelectedCompetition.AddRange(clubs);
        //    }
        //}

        //private void UpdateFilteredSwitchs()
        //{
        //    ClubsAvailableForSwitch.Clear();

        //    if (FilterSwitchNation?.Id != null && SelectedName?.Id != null)
        //    {
        //        var switchClubs = SecondNames
        //            .Where(x => x.IsNational == 0)
        //            .Where(x => x.IsWomanFlag == SelectedClub?.IsWomanFlag)
        //            .Where(c => c.NationId == FilterSwitchNation.Id && c.LeagueId != SelectedName.Id)
        //            .Select(c =>
        //            {
        //                c.Competition = GetCompetitionName(FirstNames, c.LeagueId);
        //                return c;
        //            })
        //            .OrderByDescending(c => c.Reputation)
        //            .ToList();

        //        ClubsAvailableForSwitch.AddRange(switchClubs);
        //    }
        //}

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return (success == true) ? settings.SelectedPath : null;
        }

        private void Add(Name name)
        {
            SelectedId = -1;
            SelectedGender = name.Gender;
            SelectedNationUid = name.NationUid;
            SelectedUnknown3 = name.Unknown3;
            SelectedValue = null;

            ShowDialog = true;
        }

        private void Edit(Name name)
        {
            SelectedId = name.Id;
            SelectedGender = name.Gender;
            SelectedNationUid = name.NationUid;
            SelectedUnknown3 = name.Unknown3;
            SelectedValue = name.Value;

            ShowDialog = true;
        }

        private void Delete(Name name)
        {
            //if (SelectedClub is not null)
            //{
            //    var oldLeagueId = SelectedClub.LeagueId;

            //    SelectedClub.BasedId = SelectedClub.NationId;
            //    SelectedClub.LeagueId = -1;

            //    // Update index
            //    if (oldLeagueId >= 0 && clubsByLeagueLookup.TryGetValue((short)oldLeagueId, out var clubs))
            //    {
            //        clubs.Remove(SelectedClub);
            //    }

            //    UpdateFilteredClubs();
            //}
        }

        private void Cancel()
        {
            SelectedId = null;
            SelectedGender = null;
            SelectedNationUid = null;
            SelectedUnknown3 = null;
            SelectedValue = null;

            ShowDialog = false;
        }

        private void Confirm()
        {
            //ShowMoveDialog = false;

            //if (SelectedName is Competition comp && SwitchedWithClub is not null)
            //{
            //    var oldLeagueId = SwitchedWithClub.LeagueId;

            //    SwitchedWithClub.BasedId = comp.NationId;
            //    SwitchedWithClub.LeagueId = comp.Id;

            //    // Update index
            //    if (oldLeagueId >= 0 && clubsByLeagueLookup.TryGetValue(oldLeagueId, out var oldClubs))
            //    {
            //        oldClubs.Remove(SwitchedWithClub);
            //    }

            //    if (!clubsByLeagueLookup.TryGetValue(comp.Id, out var newClubs))
            //    {
            //        newClubs = [];
            //        clubsByLeagueLookup[comp.Id] = newClubs;
            //    }
            //    newClubs.Add(SwitchedWithClub);

            //    UpdateFilteredClubs();
            //    UpdateFilteredSwitchs();
            //}
        }

        //private void ConfirmSwitchImpl()
        //{
        //    ShowSwitchDialog = false;

        //    if (SelectedClub is Club club1 && SwitchedWithClub is Club club2)
        //    {
        //        var based1 = club1.BasedId;
        //        var league1 = club1.LeagueId;
        //        var based2 = club2.BasedId;
        //        var league2 = club2.LeagueId;

        //        SelectedClub.BasedId = based2;
        //        SelectedClub.LeagueId = league2;
        //        SwitchedWithClub.BasedId = based1;
        //        SwitchedWithClub.LeagueId = league1;

        //        // Update index
        //        if (league1 >= 0 && clubsByLeagueLookup.TryGetValue((short)league1, out var clubs1))
        //        {
        //            clubs1.Remove(club1);
        //        }
        //        if (league2 >= 0 && clubsByLeagueLookup.TryGetValue((short)league2, out var clubs2))
        //        {
        //            clubs2.Remove(club2);
        //            clubs2.Add(club1);
        //        }

        //        if (league1 >= 0)
        //        {
        //            if (!clubsByLeagueLookup.TryGetValue((short)league1, out var newClubs1))
        //            {
        //                newClubs1 = [];
        //                clubsByLeagueLookup[league1] = newClubs1;
        //            }
        //            newClubs1.Add(club2);
        //        }

        //        UpdateFilteredClubs();
        //        UpdateFilteredSwitchs();
        //    }
        //}

        private async Task SaveImpl()
        {
            try
            {
                if (FirstNameParser != null && SecondNameParser != null)
                {
                    FirstNameParser.Replace(FirstNames);
                    await FirstNameParser.Save();

                    SecondNameParser.Replace(FirstNames);
                    await SecondNameParser.Save();

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
                if (FirstNameParser != null && SecondNameParser != null)
                {
                    FirstNameParser.Replace(FirstNames);
                    await FirstNameParser.Save(settings.SelectedPath + "\\competition.dat");

                    SecondNameParser.Replace(FirstNames);
                    await SecondNameParser.Save(settings.SelectedPath + "\\club.dat");

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
