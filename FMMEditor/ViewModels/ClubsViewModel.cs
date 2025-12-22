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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.ViewModels
{
    public class ClubsViewModel : ReactiveObject
    {
        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        [Reactive] public string SearchQuery { get; set; } = "";
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        // Parsers
        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }
        public extern CompetitionParser? CompetitionParser { [ObservableAsProperty] get; }
        public extern PeopleParser? PeopleParser { [ObservableAsProperty] get; }
        public extern PlayerParser? PlayerParser { [ObservableAsProperty] get; }
        public extern NameParser? FirstNameParser { [ObservableAsProperty] get; }
        public extern NameParser? SecondNameParser { [ObservableAsProperty] get; }
        public extern NameParser? CommonNameParser { [ObservableAsProperty] get; }

        // Collections for display
        public BulkObservableCollection<Nation> Nations { get; } = [];
        public BulkObservableCollection<ClubDisplayModel> ClubsList { get; } = [];

        // Commands
        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClubs { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseCompetitions { get; private set; }
        public ReactiveCommand<string, PeopleParser> ParsePeople { get; private set; }
        public ReactiveCommand<string, PlayerParser> ParsePlayers { get; private set; }
        public ReactiveCommand<string, NameParser> ParseFirstName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseSecondName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseCommonName { get; private set; }

        // Commands for Add/Edit
        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<ClubDisplayModel, Unit> EditCommand { get; private set; }
        public ReactiveCommand<ClubDisplayModel, Unit> CopyCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopyUidCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopyUidHexCommand { get; private set; }

        [Reactive] public ClubDisplayModel? SelectedClub { get; set; }
        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView clubsView;
        private readonly IDialogService dialogService;

        // Lookup dictionaries for performance
        private Dictionary<short, string> nationLookup = [];
        private Dictionary<short, string> competitionLookup = [];
        private Dictionary<int, People> peopleLookup = [];
        private Dictionary<int, Player> playerLookup = [];
        private Dictionary<int, string> firstNameLookup = [];
        private Dictionary<int, string> lastNameLookup = [];
        private Dictionary<int, string> commonNameLookup = [];

        public ClubsViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; }, outputScheduler: RxApp.MainThreadScheduler);

            clubsView = CollectionViewSource.GetDefaultView(ClubsList);
            clubsView.Filter = obj =>
            {
                if (obj is ClubDisplayModel club)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || club.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || club.ShortName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || club.NationName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || club.CompetitionName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }
                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseClubs = ReactiveCommand.CreateFromTask<string, ClubParser>(ClubParser.Load);
            ParseClubs.ToPropertyEx(this, vm => vm.ClubParser);

            ParseCompetitions = ReactiveCommand.CreateFromTask<string, CompetitionParser>(CompetitionParser.Load);
            ParseCompetitions.ToPropertyEx(this, vm => vm.CompetitionParser);

            ParsePeople = ReactiveCommand.CreateFromTask<string, PeopleParser>(PeopleParser.Load);
            ParsePeople.ToPropertyEx(this, vm => vm.PeopleParser);

            ParsePlayers = ReactiveCommand.CreateFromTask<string, PlayerParser>(PlayerParser.Load);
            ParsePlayers.ToPropertyEx(this, vm => vm.PlayerParser);

            ParseFirstName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseFirstName.ToPropertyEx(this, vm => vm.FirstNameParser);

            ParseSecondName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseSecondName.ToPropertyEx(this, vm => vm.SecondNameParser);

            ParseCommonName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseCommonName.ToPropertyEx(this, vm => vm.CommonNameParser);

            AddCommand = ReactiveCommand.CreateFromTask(OpenAddClubDialogAsync);
            EditCommand = ReactiveCommand.CreateFromTask<ClubDisplayModel>(OpenEditClubDialogAsync);
            CopyCommand = ReactiveCommand.CreateFromTask<ClubDisplayModel>(OpenCopyClubDialogAsync);
            CopyUidCommand = ReactiveCommand.Create(CopyUidToClipboard);
            CopyUidHexCommand = ReactiveCommand.Create(CopyUidHexToClipboard);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

            // Wire up folder path to parsers
            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\nation.dat")
                .InvokeCommand(ParseNations);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\club.dat")
                .InvokeCommand(ParseClubs);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\competition.dat")
                .InvokeCommand(ParseCompetitions);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\people.dat")
                .InvokeCommand(ParsePeople);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\players.dat")
                .InvokeCommand(ParsePlayers);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\first_names.dat")
                .InvokeCommand(ParseFirstName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\second_names.dat")
                .InvokeCommand(ParseSecondName);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\common_names.dat")
                .InvokeCommand(ParseCommonName);

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
                    RefreshClubsDisplay();
                });

            this.WhenAnyValue(vm => vm.ClubParser)
                .WhereNotNull()
                .Subscribe(x => RefreshClubsDisplay());

            this.WhenAnyValue(vm => vm.CompetitionParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    competitionLookup = x.Items.ToDictionary(c => c.Id, c => c.FullName);
                    RefreshClubsDisplay();
                });

            this.WhenAnyValue(vm => vm.PeopleParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    peopleLookup = x.Items.ToDictionary(p => p.Id, p => p);
                });

            this.WhenAnyValue(vm => vm.PlayerParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    playerLookup = x.Items.ToDictionary(p => p.Id, p => p);
                });

            this.WhenAnyValue(vm => vm.FirstNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    firstNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                });

            this.WhenAnyValue(vm => vm.SecondNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    lastNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                });

            this.WhenAnyValue(vm => vm.CommonNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    commonNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => clubsView.Refresh());
        }

        private async Task OpenAddClubDialogAsync()
        {
            var viewModel = new ClubEditViewModel(Nations, peopleLookup, playerLookup, firstNameLookup, lastNameLookup, commonNameLookup);
            viewModel.InitializeForAdd();

            var view = new ClubEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "ClubsDialogHost");

            if (result is ClubEditViewModel vm && vm.Validate())
            {
                AddClub(vm);
                RefreshClubsDisplay();
            }
        }

        private async Task OpenEditClubDialogAsync(ClubDisplayModel? club)
        {
            if (club == null) return;

            var viewModel = new ClubEditViewModel(Nations, peopleLookup, playerLookup, firstNameLookup, lastNameLookup, commonNameLookup);
            viewModel.InitializeForEdit(club);

            var view = new ClubEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "ClubsDialogHost");

            if (result is ClubEditViewModel vm && vm.Validate())
            {
                UpdateClub(vm);
                RefreshClubsDisplay();
            }
        }

        private async Task OpenCopyClubDialogAsync(ClubDisplayModel? club)
        {
            if (club == null) return;

            var viewModel = new ClubEditViewModel(Nations, peopleLookup, playerLookup, firstNameLookup, lastNameLookup, commonNameLookup);
            viewModel.InitializeForCopy(club);

            var view = new ClubEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "ClubsDialogHost");

            if (result is ClubEditViewModel vm && vm.Validate())
            {
                AddClub(vm);
                RefreshClubsDisplay();
            }
        }

        private void CopyUidToClipboard()
        {
            if (SelectedClub != null)
            {
                Clipboard.SetText(SelectedClub.Uid.ToString());
                MessageQueue.Enqueue("UID copied to clipboard");
            }
        }

        private void CopyUidHexToClipboard()
        {
            if (SelectedClub != null)
            {
                byte[] bytes = BitConverter.GetBytes(SelectedClub.Uid);
                string hex = BitConverter.ToString(bytes).Replace("-", "");
                Clipboard.SetText(hex);
                MessageQueue.Enqueue($"UID hex copied: {hex}");
            }
        }

        private void AddClub(ClubEditViewModel vm)
        {
            if (ClubParser == null) return;

            var nextUid = ClubParser.Items.Count > 0 ? ClubParser.Items.Max(x => x.Uid) + 1 : 1;

            var newClub = new Club
            {
                Id = -1,
                Uid = nextUid,
                FullName = vm.FullName ?? "",
                FullNameTerminator = vm.FullNameTerminator,
                ShortName = vm.ShortName ?? "",
                ShortNameTerminator = vm.ShortNameTerminator,
                SixLetterName = vm.SixLetterName ?? "",
                ThreeLetterName = vm.ThreeLetterName ?? "",
                BasedId = vm.BasedId ?? 0,
                NationId = vm.NationId!.Value,
                Status = vm.Status,
                Academy = vm.Academy ?? 10,
                Facilities = vm.Facilities ?? 10,
                AttAvg = vm.AttAvg ?? 0,
                AttMin = vm.AttMin ?? 0,
                AttMax = vm.AttMax ?? 0,
                Reserves = vm.Reserves ?? 0,
                LeagueId = vm.LeagueId ?? -1,
                OtherDivision = vm.OtherDivision,
                OtherLastPosition = vm.OtherLastPosition,
                Stadium = vm.Stadium ?? -1,
                LastLeague = vm.LastLeague ?? -1,
                LeaguePos = vm.LeaguePos ?? 0,
                Reputation = vm.Reputation ?? 0,
                MainClub = vm.MainClub ?? -1,
                IsNational = vm.IsNational,
                Gender = vm.Gender,
                Colors = new Color[6],
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

            ClubParser.Add(newClub);
            MessageQueue.Enqueue("Club added successfully");
        }

        private void UpdateClub(ClubEditViewModel vm)
        {
            var existingClub = ClubParser?.Items.FirstOrDefault(x => x.Uid == vm.Uid);
            if (existingClub == null) return;

            existingClub.Id = -1;
            existingClub.FullName = vm.FullName ?? "";
            existingClub.FullNameTerminator = vm.FullNameTerminator;
            existingClub.ShortName = vm.ShortName ?? "";
            existingClub.ShortNameTerminator = vm.ShortNameTerminator;
            existingClub.SixLetterName = vm.SixLetterName ?? "";
            existingClub.ThreeLetterName = vm.ThreeLetterName ?? "";
            existingClub.BasedId = vm.BasedId ?? 0;
            existingClub.NationId = vm.NationId!.Value;
            existingClub.Status = vm.Status;
            existingClub.Academy = vm.Academy ?? 10;
            existingClub.Facilities = vm.Facilities ?? 10;
            existingClub.AttAvg = vm.AttAvg ?? 0;
            existingClub.AttMin = vm.AttMin ?? 0;
            existingClub.AttMax = vm.AttMax ?? 0;
            existingClub.Reserves = vm.Reserves ?? 0;
            existingClub.LeagueId = vm.LeagueId ?? -1;
            existingClub.OtherDivision = vm.OtherDivision;
            existingClub.OtherLastPosition = vm.OtherLastPosition;
            existingClub.Stadium = vm.Stadium ?? -1;
            existingClub.LastLeague = vm.LastLeague ?? -1;
            existingClub.LeaguePos = vm.LeaguePos ?? 0;
            existingClub.Reputation = vm.Reputation ?? 0;
            existingClub.MainClub = vm.MainClub ?? -1;
            existingClub.IsNational = vm.IsNational;
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

            MessageQueue.Enqueue("Club updated successfully");
        }

        private void RefreshClubsDisplay()
        {
            if (ClubParser == null) return;

            var displayList = ClubParser.Items.Select(c =>
            {
                var display = new ClubDisplayModel(c)
                {
                    NationName = nationLookup.GetValueOrDefault(c.NationId, ""),
                    BasedName = nationLookup.GetValueOrDefault(c.BasedId, ""),
                    CompetitionName = competitionLookup.GetValueOrDefault(c.LeagueId, "")
                };
                return display;
            });

            ClubsList.Reset(displayList);
        }

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return (success == true) ? settings.SelectedPath : null;
        }

        private async Task SaveImpl()
        {
            try
            {
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
    }
}
