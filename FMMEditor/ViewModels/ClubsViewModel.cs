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

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => clubsView.Refresh());
        }

        private async Task OpenAddClubDialogAsync()
        {
            var viewModel = new ClubEditViewModel(Nations);
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

            var viewModel = new ClubEditViewModel(Nations);
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

            var viewModel = new ClubEditViewModel(Nations);
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

            // Create a temporary memory stream to initialize the Club
            using var ms = new System.IO.MemoryStream();
            using var writer = new System.IO.BinaryWriter(ms);
            
            // Write minimal required data for Club constructor
            writer.Write(-1); // Id
            writer.Write(nextUid); // Uid
            writer.Write((byte)0); // FullName length (will be set later)
            writer.Write((byte)0); // FullName terminator
            writer.Write((byte)0); // ShortName length
            writer.Write((byte)0); // ShortName terminator
            writer.Write((byte)0); // SixLetterName length
            writer.Write((byte)0); // ThreeLetterName length
            writer.Write((short)0); // BasedId
            writer.Write((short)0); // NationId
            
            // Colors
            for (int i = 0; i < 6; i++)
                writer.Write((short)0);
            
            // Kits
            for (int i = 0; i < 6; i++)
            {
                writer.Write((byte)0); // Unknown1
                writer.Write((byte)0); // Unknown2
                for (int j = 0; j < 10; j++)
                    writer.Write((short)0); // Colors
            }
            
            writer.Write((byte)1); // Status
            writer.Write((byte)10); // Academy
            writer.Write((byte)10); // Facilities
            writer.Write((short)0); // AttAvg
            writer.Write((short)0); // AttMin
            writer.Write((short)0); // AttMax
            writer.Write((byte)0); // Reserves
            writer.Write((short)-1); // LeagueId
            writer.Write((short)-1); // OtherDivision
            writer.Write((byte)0); // OtherLastPosition
            writer.Write((short)-1); // Stadium
            writer.Write((short)-1); // LastLeague
            writer.Write(false); // Unknown4Flag
            writer.Write(0); // Unknown5 length
            writer.Write((byte)0); // LeaguePos
            writer.Write((short)0); // Reputation
            writer.Write(new byte[20]); // Unknown6
            writer.Write((short)0); // Affiliates count
            writer.Write((short)0); // Players count
            for (int i = 0; i < 11; i++)
                writer.Write(0); // Unknown7
            writer.Write(-1); // MainClub
            writer.Write((short)0); // IsNational
            writer.Write(new byte[33]); // Unknown8
            writer.Write(new byte[40]); // Unknown9
            writer.Write((short)0); // IsWomanFlag
            
            ms.Position = 0;
            using var reader = new System.IO.BinaryReader(ms);
            var newClub = new Club(reader);

            // Now set the actual values
            newClub.Id = -1;
            newClub.Uid = nextUid;
            newClub.FullName = vm.FullName ?? "";
            newClub.ShortName = vm.ShortName ?? "";
            newClub.SixLetterName = vm.SixLetterName ?? "";
            newClub.ThreeLetterName = vm.ThreeLetterName ?? "";
            newClub.BasedId = vm.BasedId ?? 0;
            newClub.NationId = vm.NationId!.Value;
            newClub.Status = vm.Status;
            newClub.Academy = vm.Academy ?? 10;
            newClub.Facilities = vm.Facilities ?? 10;
            newClub.AttAvg = vm.AttAvg ?? 0;
            newClub.AttMin = vm.AttMin ?? 0;
            newClub.AttMax = vm.AttMax ?? 0;
            newClub.Reserves = vm.Reserves ?? 0;
            newClub.LeagueId = vm.LeagueId ?? -1;
            newClub.LeaguePos = vm.LeaguePos ?? 0;
            newClub.Reputation = vm.Reputation ?? 0;
            newClub.Stadium = vm.Stadium ?? -1;
            newClub.LastLeague = vm.LastLeague ?? -1;
            newClub.MainClub = vm.MainClub ?? -1;
            newClub.IsNational = vm.IsNational;
            newClub.IsWomanFlag = vm.IsWomanFlag;

            ClubParser.Items.Add(newClub);
            MessageQueue.Enqueue("Club added successfully");
        }

        private void UpdateClub(ClubEditViewModel vm)
        {
            var existingClub = ClubParser?.Items.FirstOrDefault(x => x.Uid == vm.Uid);
            if (existingClub == null) return;

            existingClub.Id = -1;
            existingClub.FullName = vm.FullName ?? "";
            existingClub.ShortName = vm.ShortName ?? "";
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
            existingClub.LeaguePos = vm.LeaguePos ?? 0;
            existingClub.Reputation = vm.Reputation ?? 0;
            existingClub.Stadium = vm.Stadium ?? -1;
            existingClub.LastLeague = vm.LastLeague ?? -1;
            existingClub.MainClub = vm.MainClub ?? -1;
            existingClub.IsNational = vm.IsNational;
            existingClub.IsWomanFlag = vm.IsWomanFlag;

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
