using DynamicData;
using FMELibrary;
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
using System.Windows.Data;

namespace FMEViewer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public extern bool ShowSearch { [ObservableAsProperty] get; }
        [Reactive] public string SearchQuery { get; set; } = "";
        [Reactive] public Competition? SelectedCompetition { get; set; }
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        public extern string? FolderPath { [ObservableAsProperty] get; }

        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern CompetitionParser? CompParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }
        public ObservableCollection<Competition> Comps { get; }
        public ObservableCollection<Club> Clubs { get; }

        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNation { get; private set; }
        public ReactiveCommand<string, CompetitionParser> ParseComp { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClub { get; private set; }

        private readonly ICollectionView compsView;
        private readonly ICollectionView clubsView;
        private readonly IDialogService dialogService;

        public MainViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; });

            Comps = new ObservableCollection<Competition>();
            compsView = CollectionViewSource.GetDefaultView(Comps);
            compsView.Filter = obj =>
            {
                if (obj is Competition competition)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                    || competition.FullName.ToLowerInvariant().Contains(SearchQuery.ToLowerInvariant())
                    || competition.Nation.ToLowerInvariant().Contains(SearchQuery.ToLowerInvariant());
                }

                return true;
            };

            Clubs = new ObservableCollection<Club>();
            clubsView = CollectionViewSource.GetDefaultView(Clubs);
            clubsView.Filter = obj =>
            {
                if (obj is not Club club)
                    return false;

                return club.LeagueId == SelectedCompetition?.Id;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNation = ReactiveCommand.Create<string, NationParser>((path) => new NationParser(path));
            ParseNation.ToPropertyEx(this, vm => vm.NationParser);

            ParseComp = ReactiveCommand.Create<string, CompetitionParser>((path) => new CompetitionParser(path));
            ParseComp.ToPropertyEx(this, vm => vm.CompParser);

            ParseClub = ReactiveCommand.Create<string, ClubParser>((path) => new ClubParser(path));
            ParseClub.ToPropertyEx(this, vm => vm.ClubParser);

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

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x =>
                {
                    compsView.Refresh();
                });

            this.WhenAnyValue(vm => vm.SelectedCompetition)
                .Subscribe(x =>
                {
                    clubsView.Refresh();
                });
        }

        private string? LoadImpl()
        {
            var settings = new FolderBrowserDialogSettings();
            bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
            return (success == true) ? settings.SelectedPath : null;
        }

        private async Task SaveImpl()
        {
            if (ClubParser != null)
            {
                ClubParser.Count = Clubs.Count;
                ClubParser.Items = Clubs.ToList();

                await ClubParser.Save();
            }
        }

        private async Task SaveAsImpl()
        {
            try
            {
                var settings = new FolderBrowserDialogSettings();
                bool? success = dialogService.ShowFolderBrowserDialog(this, settings);
                if (success == true)
                {
                    if (ClubParser != null)
                    {
                        ClubParser.Count = Clubs.Count;
                        ClubParser.Items = Clubs.ToList();

                        await ClubParser.Save(settings.SelectedPath + "\\club.dat");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private static string GetNationName(IEnumerable<Nation>? nations, int id)
        {
            return nations?.FirstOrDefault(x => x.Id == id)?.Name ?? "-";
        }
    }
}
