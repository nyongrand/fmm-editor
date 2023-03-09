using DynamicData;
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

        public extern NationParser NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser CompParser { [ObservableAsProperty] get; }
        public extern ClubParser ClubParser { [ObservableAsProperty] get; }
        public ObservableCollection<Nation> Nations { get; }
        public ObservableCollection<Competition> Comps { get; }
        public ObservableCollection<Club> Clubs { get; }

        public ReactiveCommand<Unit, string> Load { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNation { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseComp { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClub { get; private set; }

        public MainViewModel()
        {
            Nations = new ObservableCollection<Nation>();
            Comps = new ObservableCollection<Competition>();
            Clubs = new ObservableCollection<Club>();

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNation = ReactiveCommand.Create<string, NationParser>((path) => new NationParser(path));
            ParseNation.ToPropertyEx(this, vm => vm.NationParser);
            ParseNation.ThrownExceptions.Subscribe(x =>
            {
            });

            ParseComp = ReactiveCommand.Create<string, CompetitionParser>((path) => new CompetitionParser(path));
            ParseComp.ToPropertyEx(this, vm => vm.CompParser);
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
                .Select(x => Path.Combine(x, "club.dat"))
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

            this.WhenAnyValue(vm => vm.ClubParser, vm => vm.NationParser)
                .Where(pair => pair.Item1 != null)
                .Subscribe(pair =>
                {
                    Clubs.Clear();

                    var clubs = pair.Item1?.Items;
                    if (clubs != null && clubs.Any())
                    {
                        clubs.ForEach(x =>
                        {
                            x.Based = GetNationName(pair.Item2?.Items, x.BasedId);
                            x.Nation = GetNationName(pair.Item2?.Items, x.NationId);
                        });

                        Clubs.AddRange(clubs);
                    }
                });

        }

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

        #region Private Methods

        private static string GetNationName(IEnumerable<Nation> nations, int id)
        {
            return nations?.FirstOrDefault(x => x.Id == id)?.Name ?? "-";
        }

        #endregion
    }
}
