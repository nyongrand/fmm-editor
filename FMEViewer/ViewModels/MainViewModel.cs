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
    public class MainViewModel : ReactiveObject
    {
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        [Reactive] public string SearchQuery { get; set; } = "";

        public extern string? FilePath { [ObservableAsProperty] get; }
        public extern SecondNameParser? SecondNameParser { [ObservableAsProperty] get; }
        public ObservableCollection<Name> Names { get; }

        public ReactiveCommand<Unit, Unit> HideResult { get; private set; }
        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<string, SecondNameParser> Parse { get; private set; }

        private readonly ICollectionView namesView;
        private readonly IDialogService dialogService;

        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            Names = new ObservableCollection<Name>();
            namesView = CollectionViewSource.GetDefaultView(Names);
            namesView.Filter = obj =>
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

            HideResult = ReactiveCommand.Create(() => { SearchQuery = ""; });

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FilePath);

            Parse = ReactiveCommand.CreateFromTask<string, SecondNameParser>(ParseImpl);
            Parse.ToPropertyEx(this, vm => vm.SecondNameParser);

            Save = ReactiveCommand.CreateFromTask(SaveImpl);

            this.WhenAnyValue(vm => vm.FilePath)
                .WhereNotNull()
                .InvokeCommand(Parse);

            this.WhenAnyValue(vm => vm.SecondNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Names.Clear();
                    Names.AddRange(x.Names);
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    namesView.Refresh();
                });
        }

        private async Task SaveImpl()
        {
            if (SecondNameParser != null)
            {
                SecondNameParser.Count = Names.Count;
                SecondNameParser.Names = Names.ToList();

                await SecondNameParser.Save();
            }
        }

        private string? LoadImpl()
        {
            var dialog = new OpenFileDialogSettings { Filter = "FM22 File (*.dat)|*.dat" };
            bool? success = dialogService.ShowOpenFileDialog(this, dialog);
            return (success == true) ? dialog.FileName : null;
        }

        private async Task<SecondNameParser> ParseImpl(string file)
        {
            var parser = new SecondNameParser(file);
            await parser.Parse();
            return parser;
        }
    }
}
