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

        [Reactive] public NationParser NationParser { get; set; }
        [Reactive] public CompetitionParser CompetitionParser { get; set; }
        [Reactive] public ClubParser ClubParser { get; set; }

        public extern bool Loading { [ObservableAsProperty] get; }
        public extern bool EnableNation { [ObservableAsProperty] get; }
        public extern bool EnableCompetition { [ObservableAsProperty] get; }
        public extern bool EnableClub { [ObservableAsProperty] get; }

        public ObservableCollection<Nation> Nations { get; }
        public ObservableCollection<Competition> Competitions { get; }
        public ObservableCollection<Club> Clubs { get; }

        public ReactiveCommand<Unit, string> Load { get; private set; }
        public ReactiveCommand<string, Unit> Parse { get; private set; }

        public MainViewModel()
        {
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

            this.WhenAnyValue(vm => vm.NationParser)
                .Select(x => x != null && x.Items.Any())
                .ToPropertyEx(this, vm => vm.EnableNation);

            this.WhenAnyValue(vm => vm.CompetitionParser)
                .Select(x => x != null && x.Items.Any())
                .ToPropertyEx(this, vm => vm.EnableCompetition);

            this.WhenAnyValue(vm => vm.ClubParser)
                .Select(x => x != null && x.Items.Any())
                .ToPropertyEx(this, vm => vm.EnableClub);
        }

        #region Private Methods

        private string LoadImpl()
        {
#if ANDROID
            var filesDir = Android.App.Application.Context.GetExternalFilesDir(null);
            var databaseDir = Path.Combine(filesDir.AbsolutePath, "database");
            if (Directory.Exists(databaseDir))
                return databaseDir;
#endif
            return "";

            //var settings = new FolderBrowserDialogSettings();
            //bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            //return (success == true) ? settings.SelectedPath : null;
        }

        private async Task ParseImpl(string dbPath)
        {
            NationParser = await NationParser.Load(Path.Combine(dbPath, "nation.dat"));
            CompetitionParser = await CompetitionParser.Load(Path.Combine(dbPath, "competition.dat"));
            ClubParser = await ClubParser.Load(Path.Combine(dbPath, "club.dat"));
        }

        #endregion
    }
}
