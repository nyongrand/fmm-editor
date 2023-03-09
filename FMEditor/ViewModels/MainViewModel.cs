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
        public extern string FolderPath { [ObservableAsProperty] get; }

        public extern bool EnableNation { [ObservableAsProperty] get; }
        public extern bool EnableCompetition { [ObservableAsProperty] get; }
        public extern bool EnableClub { [ObservableAsProperty] get; }

        public extern NationParser NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser CompetitionParser { [ObservableAsProperty] get; }
        public extern ClubParser ClubParser { [ObservableAsProperty] get; }
        public ObservableCollection<Nation> Nations { get; }
        public ObservableCollection<Competition> Competitions { get; }
        public ObservableCollection<Club> Clubs { get; }

        public ReactiveCommand<Unit, string> Load { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNation { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseComp { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClub { get; private set; }

        public MainViewModel()
        {
            Nations = new ObservableCollection<Nation>();
            Competitions = new ObservableCollection<Competition>();
            Clubs = new ObservableCollection<Club>();

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNation = ReactiveCommand.Create<string, NationParser>((path) => new NationParser(path));
            ParseNation.ToPropertyEx(this, vm => vm.NationParser);
            ParseNation.ThrownExceptions.Subscribe(x =>
            {
            });

            ParseComp = ReactiveCommand.Create<string, CompetitionParser>((path) => new CompetitionParser(path));
            ParseComp.ToPropertyEx(this, vm => vm.CompetitionParser);
            ParseComp.ThrownExceptions.Subscribe(x =>
            {
            });

            ParseClub = ReactiveCommand.Create<string, ClubParser>((path) => new ClubParser(path));
            ParseClub.ToPropertyEx(this, vm => vm.ClubParser);
            ParseClub.ThrownExceptions.Subscribe(x =>
            {
            });

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => Path.Combine(x, "nation.dat"))
                .InvokeCommand(ParseNation);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => Path.Combine(x, "competition.dat"))
                .InvokeCommand(ParseComp);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => Path.Combine(x, "clubu.dat"))
                .InvokeCommand(ParseClub);

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

        #endregion
    }
}
