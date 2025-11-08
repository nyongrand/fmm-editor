using DynamicData;
using FMELibrary;
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

namespace FMEViewer.ViewModels
{
    public class MainViewModel : ReactiveObject
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

        public ReactiveCommand<Unit, Unit> SwitchClub { get; private set; }
        public ReactiveCommand<Unit, Unit> MoveClub { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveClub { get; private set; }

        public ReactiveCommand<Unit, Unit> CancelSwitch { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmSwitch { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmMove { get; private set; }

        public extern string? FolderPath { [ObservableAsProperty] get; }

        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser? CompParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }
        public ObservableCollection<Nation> Nations { get; }
        public ObservableCollection<Competition> Comps { get; }
        public ObservableCollection<Club> Clubs { get; }
        public ObservableCollection<Club> Switchs { get; }

        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNation { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseComp { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClub { get; private set; }

        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView compsView;
        private readonly ICollectionView clubsView;
        private readonly ICollectionView switchsView;
        private readonly IDialogService dialogService;

        public MainViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            Nations = [];

            Comps = [];
            compsView = CollectionViewSource.GetDefaultView(Comps);
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
            clubsView = CollectionViewSource.GetDefaultView(Clubs);
            clubsView.Filter = obj =>
            {
                if (obj is not Club club)
                    return false;

                return club.LeagueId == SelectedCompetition?.Id;
            };

            Switchs = [];
            switchsView = CollectionViewSource.GetDefaultView(Switchs);
            switchsView.Filter = obj =>
            {
                if (obj is Club club)
                {
                    if (club.LeagueId == SelectedCompetition?.Id)
                        return false;

                    return club.NationId == FilterSwitchNation?.Id;
                }

                return false;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNation = ReactiveCommand.CreateFromTask<string, NationParser>((path) => NationParser.Load(path));
            ParseNation.ToPropertyEx(this, vm => vm.NationParser);

            ParseComp = ReactiveCommand.CreateFromTask<string, CompetitionParser>((path) => CompetitionParser.Load(path));
            ParseComp.ToPropertyEx(this, vm => vm.CompParser);

            ParseClub = ReactiveCommand.CreateFromTask<string, ClubParser>((path) => ClubParser.Load(path));
            ParseClub.ToPropertyEx(this, vm => vm.ClubParser);

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
                .InvokeCommand(ParseNation);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\competition.dat")
                .InvokeCommand(ParseComp);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\club.dat")
                .InvokeCommand(ParseClub);

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
                    Comps.Clear();

                    var competitions = pair.Item1?.Items;
                    if (competitions != null)
                    {
                        competitions.ForEach(x =>
                        {
                            x.Nation = GetNationName(pair.Item2?.Items, x.NationId);
                        });

                        Comps.AddRange(competitions);
                    }
                });

            this.WhenAnyValue(vm => vm.ClubParser, vm => vm.NationParser, vm => vm.SelectedCompetition)
                .Where(pair => pair.Item1 != null)
                .Subscribe(pair =>
                {
                    Clubs.Clear();
                    Switchs.Clear();

                    var clubs = pair.Item1?.Items;
                    if (clubs != null && clubs.Count != 0)
                    {
                        clubs.ForEach(x =>
                        {
                            x.Based = GetNationName(pair.Item2?.Items, x.BasedId);
                            x.Nation = GetNationName(pair.Item2?.Items, x.NationId);
                        });

                        Clubs.AddRange(clubs);
                        Switchs.AddRange(clubs);
                    }
                });

            this.WhenAnyValue(vm => vm.ShowMoveDialog, vm => vm.ShowSwitchDialog)
                .Select(pair => pair.Item1 || pair.Item2)
                .Subscribe(x => ShowDialog = x);

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => compsView.Refresh());

            this.WhenAnyValue(vm => vm.SelectedCompetition)
                .Subscribe(x =>
                {
                    clubsView.Refresh();
                    switchsView.Refresh();
                });

            this.WhenAnyValue(vm => vm.FilterSwitchNation)
                .Subscribe(x => switchsView.Refresh());
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
                SelectedClub.BasedId = SelectedClub.NationId;
                SelectedClub.LeagueId = -1;

                clubsView.Refresh();
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
                SwitchedWithClub.BasedId = comp.NationId;
                SwitchedWithClub.LeagueId = comp.Id;

                clubsView.Refresh();
                switchsView.Refresh();
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

                clubsView.Refresh();
                switchsView.Refresh();
            }
        }

        private async Task SaveImpl()
        {
            try
            {
                if (CompParser != null && ClubParser != null)
                {
                    CompParser.Count = (short)Comps.Count;
                    CompParser.Items = [.. Comps];
                    await CompParser.Save();

                    ClubParser.Count = Clubs.Count;
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
                    CompParser.Count = (short)Comps.Count;
                    CompParser.Items = [.. Comps];
                    await CompParser.Save(settings.SelectedPath + "\\competition.dat");

                    ClubParser.Count = Clubs.Count;
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

        private static string GetNationName(IEnumerable<Nation>? nations, int id)
        {
            return nations?.FirstOrDefault(x => x.Id == id)?.Name ?? "-";
        }

        #endregion
    }
}
