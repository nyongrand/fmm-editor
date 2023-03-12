using FMELibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace FMEditor.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public extern string DatabasePath { [ObservableAsProperty] get; }

        [Reactive] public string LoadMessage { get; set; } = "";
        public extern bool Idle { [ObservableAsProperty] get; }
        public extern bool Loading { [ObservableAsProperty] get; }
        public extern bool EnableNation { [ObservableAsProperty] get; }
        public extern bool EnableCompetition { [ObservableAsProperty] get; }
        public extern bool EnableClub { [ObservableAsProperty] get; }

        [Reactive] public ContinentParser ContinentParser { get; set; }
        [Reactive] public NationParser NationParser { get; set; }
        [Reactive] public CompetitionParser CompetitionParser { get; set; }
        [Reactive] public ClubParser ClubParser { get; set; }

        public ObservableCollection<Continent> Continents { get; }
        public ObservableCollection<Nation> Nations { get; }
        public ObservableCollection<Competition> Competitions { get; }
        public ObservableCollection<Club> Clubs { get; }

        public ReactiveCommand<Unit, string> Load { get; private set; }
        public ReactiveCommand<string, Unit> Parse { get; private set; }

        public MainViewModel(
            ContinentParser continentParser,
            NationParser nationParser,
            CompetitionParser competitionParser,
            ClubParser clubParser)
        {
            ContinentParser = continentParser;
            NationParser = nationParser;
            CompetitionParser = competitionParser;
            ClubParser = clubParser;

            Continents = new ObservableCollection<Continent>();
            Nations = new ObservableCollection<Nation>();
            Competitions = new ObservableCollection<Competition>();
            Clubs = new ObservableCollection<Club>();

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.DatabasePath);

            Parse = ReactiveCommand.CreateFromTask<string>(ParseImpl);
            Parse.IsExecuting.ToPropertyEx(this, vm => vm.Loading);
            Parse.ThrownExceptions.Subscribe(x =>
            {

            });

            this.WhenAnyValue(vm => vm.DatabasePath)
                .WhereNotNull()
                .InvokeCommand(Parse);

            this.WhenAnyValue(vm => vm.Loading)
                .Select(x => !x)
                .ToPropertyEx(this, vm => vm.Idle);

            //this.WhenAnyValue(vm => vm.NationParser)
            //    .Select(x => x != null && x.Items.Any())
            //    .ToPropertyEx(this, vm => vm.EnableNation);

            //this.WhenAnyValue(vm => vm.CompetitionParser)
            //    .Select(x => x != null && x.Items.Any())
            //    .ToPropertyEx(this, vm => vm.EnableCompetition);

            //this.WhenAnyValue(vm => vm.ClubParser)
            //    .Select(x => x != null && x.Items.Any())
            //    .ToPropertyEx(this, vm => vm.EnableClub);
        }

        #region Private Methods

        private string LoadImpl()
        {
            var dbPath = "";

#if ANDROID

            var filesDir = Android.App.Application.Context.GetExternalFilesDir(null);
            var databaseDir = Path.Combine(filesDir.AbsolutePath, "database");
            if (Directory.Exists(databaseDir))
                dbPath = databaseDir;

#elif WINDOWS

            dbPath = @"D:\Downloads\database23";

#endif
            return dbPath;

            //var settings = new FolderBrowserDialogSettings();
            //bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            //return (success == true) ? settings.SelectedPath : null;
        }

        private async Task ParseImpl(string dbPath)
        {
            LoadMessage = "Loading Continents";
            await ContinentParser.Load(Path.Combine(dbPath, "continent.dat"));

            LoadMessage = "Loading Nations";
            await NationParser.Load(Path.Combine(dbPath, "nation.dat"));

            LoadMessage = "Loading Competitions";
            await CompetitionParser.Load(Path.Combine(dbPath, "competition.dat"));

            //LoadMessage = "Loading Clubs";
            //await ClubParser.Load(Path.Combine(dbPath, "club.dat"));


            LoadMessage = "Loading Complete";
        }

        #endregion
    }
}
