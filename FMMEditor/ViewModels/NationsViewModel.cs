using FMMEditor.Collections;
using FMMEditor.Models;
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
    public class NationsViewModel : ReactiveObject
    {
        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        [Reactive] public string SearchQuery { get; set; } = string.Empty;
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern ContinentParser? ContinentParser { [ObservableAsProperty] get; }

        public BulkObservableCollection<NationDisplayModel> NationsList { get; } = [];
        public BulkObservableCollection<Continent> Continents { get; } = [];

        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, ContinentParser> ParseContinents { get; private set; }

        public ReactiveCommand<Unit, Unit> CopyUidCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopyUidHexCommand { get; private set; }

        [Reactive] public NationDisplayModel? SelectedNation { get; set; }
        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView nationsView;
        private readonly IDialogService dialogService;
        private Dictionary<short, string> continentLookup = [];

        public NationsViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = string.Empty; }, outputScheduler: RxApp.MainThreadScheduler);

            nationsView = CollectionViewSource.GetDefaultView(NationsList);
            nationsView.Filter = obj =>
            {
                if (obj is NationDisplayModel nation)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || nation.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || nation.Nationality.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || nation.CodeName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || nation.ContinentName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseContinents = ReactiveCommand.CreateFromTask<string, ContinentParser>(ContinentParser.Load);
            ParseContinents.ToPropertyEx(this, vm => vm.ContinentParser);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);
            SaveAs = ReactiveCommand.CreateFromTask(SaveAsImpl);

            CopyUidCommand = ReactiveCommand.Create(CopyUidToClipboard);
            CopyUidHexCommand = ReactiveCommand.Create(CopyUidHexToClipboard);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(path => path + "\\nation.dat")
                .InvokeCommand(ParseNations);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(path => path + "\\continent.dat")
                .InvokeCommand(ParseContinents);

            this.WhenAnyValue(vm => vm.FolderPath)
                .Select(path => !string.IsNullOrEmpty(path))
                .ToPropertyEx(this, vm => vm.IsDatabaseLoaded);

            this.WhenAnyValue(vm => vm.ContinentParser)
                .WhereNotNull()
                .Subscribe(parser =>
                {
                    Continents.Reset(parser.Items.OrderBy(c => c.Name));
                    continentLookup = parser.Items.ToDictionary(c => c.Id, c => c.Name);
                    RefreshNationsDisplay();
                });

            this.WhenAnyValue(vm => vm.NationParser)
                .WhereNotNull()
                .Subscribe(_ => RefreshNationsDisplay());

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(_ => nationsView.Refresh());
        }

        private void RefreshNationsDisplay()
        {
            if (NationParser == null) return;

            var displayList = NationParser.Items
                .Select(n => new NationDisplayModel(n)
                {
                    ContinentName = continentLookup.GetValueOrDefault(n.ContinentId, "None")
                })
                .OrderBy(n => n.Name);

            NationsList.Reset(displayList);
        }

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return success == true ? settings.SelectedPath : null;
        }

        private async Task SaveImpl()
        {
            try
            {
                if (NationParser != null)
                {
                    await NationParser.Save();
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
                if (NationParser != null)
                {
                    await NationParser.Save(settings.SelectedPath + "\\nation.dat");
                }
                MessageQueue.Enqueue("Save Successful");
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyUidToClipboard()
        {
            if (SelectedNation != null)
            {
                Clipboard.SetText(SelectedNation.Uid.ToString());
                MessageQueue.Enqueue("UID copied to clipboard");
            }
        }

        private void CopyUidHexToClipboard()
        {
            if (SelectedNation != null)
            {
                byte[] bytes = BitConverter.GetBytes(SelectedNation.Uid);
                string hex = BitConverter.ToString(bytes).Replace("-", string.Empty);
                Clipboard.SetText(hex);
                MessageQueue.Enqueue($"UID hex copied: {hex}");
            }
        }
    }
}
