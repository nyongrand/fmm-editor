using DynamicData;
using FMELibrary;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FMEViewer.ViewModels
{
    public class NameViewModel : ReactiveObject
    {
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        [Reactive] public string SearchQuery { get; set; } = "";
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        public extern string? FilePath1 { [ObservableAsProperty] get; }
        public extern string? FilePath2 { [ObservableAsProperty] get; }
        public extern NameParser? NameParser1 { [ObservableAsProperty] get; }
        public extern NameParser? NameParser2 { [ObservableAsProperty] get; }
        public ObservableCollection<Name> Names1 { get; }
        public ObservableCollection<Name> Names2 { get; }

        public ReactiveCommand<Unit, string?> Load1 { get; private set; }
        public ReactiveCommand<Unit, string?> Load2 { get; private set; }
        public ReactiveCommand<Unit, Unit> Save1 { get; private set; }
        public ReactiveCommand<Unit, Unit> Save2 { get; private set; }
        public ReactiveCommand<string, NameParser> Parse1 { get; private set; }
        public ReactiveCommand<string, NameParser> Parse2 { get; private set; }

        private readonly ICollectionView names1View;
        private readonly ICollectionView names2View;
        private readonly IDialogService dialogService;

        public NameViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            #region Left Part

            Names1 = new ObservableCollection<Name>();
            names1View = CollectionViewSource.GetDefaultView(Names1);
            names1View.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                    || name.Value.Contains(SearchQuery)
                    || name.Nation.Contains(SearchQuery)
                    || name.Others.Contains(SearchQuery);
                }

                return true;
            };

            Load1 = ReactiveCommand.Create(LoadImpl);
            Load1.ToPropertyEx(this, vm => vm.FilePath1);

            Parse1 = ReactiveCommand.CreateFromTask<string, NameParser>(ParseImpl);
            Parse1.ToPropertyEx(this, vm => vm.NameParser1);

            Save1 = ReactiveCommand.CreateFromTask(SaveImpl);

            this.WhenAnyValue(vm => vm.FilePath1)
                .WhereNotNull()
                .InvokeCommand(Parse1);

            this.WhenAnyValue(vm => vm.NameParser1)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Names1.Clear();
                    Names1.AddRange(x.Names);
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    names1View.Refresh();
                });

            #endregion

            #region Right Part

            Names2 = new ObservableCollection<Name>();
            names2View = CollectionViewSource.GetDefaultView(Names2);
            names2View.Filter = obj =>
            {
                if (obj is Name name)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                    || name.Value.Contains(SearchQuery)
                    || name.Nation.Contains(SearchQuery)
                    || name.Others.Contains(SearchQuery);
                }

                return true;
            };

            Load2 = ReactiveCommand.Create(LoadImpl);
            Load2.ToPropertyEx(this, vm => vm.FilePath2);

            Parse2 = ReactiveCommand.CreateFromTask<string, NameParser>(ParseImpl);
            Parse2.ToPropertyEx(this, vm => vm.NameParser2);

            Save2 = ReactiveCommand.CreateFromTask(SaveImpl);

            this.WhenAnyValue(vm => vm.FilePath2)
                .WhereNotNull()
                .InvokeCommand(Parse2);

            this.WhenAnyValue(vm => vm.NameParser2)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Names2.Clear();
                    Names2.AddRange(x.Names);
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    names2View.Refresh();
                });

            #endregion
        }

        private async Task SaveImpl()
        {
            if (NameParser1 != null)
            {
                NameParser1.Count = Names1.Count;
                NameParser1.Names = Names1.ToList();

                await NameParser1.Save();
            }

            if (NameParser2 != null)
            {
                NameParser2.Count = Names2.Count;
                NameParser2.Names = Names2.ToList();

                await NameParser2.Save();
            }
        }

        private string? LoadImpl()
        {
            var dialog = new OpenFileDialogSettings { Filter = "FM22 File (*.dat)|*.dat" };
            bool? success = dialogService.ShowOpenFileDialog(this, dialog);
            return (success == true) ? dialog.FileName : null;
        }

        private async Task<NameParser> ParseImpl(string file)
        {
            var parser = new NameParser(file);
            await parser.Parse();
            return parser;
        }
    }
}
