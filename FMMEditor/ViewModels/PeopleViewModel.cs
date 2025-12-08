using FMMEditor.Collections;
using FMMEditor.Converters;
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
    public class PeopleViewModel : ReactiveObject
    {
        public extern string? FolderPath { [ObservableAsProperty] get; }
        public extern bool IsDatabaseLoaded { [ObservableAsProperty] get; }

        [Reactive] public string SearchQuery { get; set; } = "";
        public ReactiveCommand<Unit, Unit> ClearSearch { get; private set; }

        // Parsers
        public extern NationParser? NationParser { [ObservableAsProperty] get; }
        public extern ClubParser? ClubParser { [ObservableAsProperty] get; }
        public extern NameParser? FirstNameParser { [ObservableAsProperty] get; }
        public extern NameParser? SecondNameParser { [ObservableAsProperty] get; }
        public extern NameParser? CommonNameParser { [ObservableAsProperty] get; }
        public extern PeopleParser? PeopleParser { [ObservableAsProperty] get; }
        public extern PlayerParser? PlayerParser { [ObservableAsProperty] get; }

        // Collections for display
        public BulkObservableCollection<Nation> Nations { get; } = [];
        public BulkObservableCollection<Club> Clubs { get; } = [];
        public BulkObservableCollection<Name> FirstNames { get; } = [];
        public BulkObservableCollection<Name> LastNames { get; } = [];
        public BulkObservableCollection<Name> CommonNames { get; } = [];
        public BulkObservableCollection<PeopleDisplayModel> PeopleList { get; } = [];

        // Commands
        public ReactiveCommand<Unit, string?> Load { get; private set; }
        public ReactiveCommand<Unit, Unit> Save { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveAs { get; private set; }
        public ReactiveCommand<string, NationParser> ParseNations { get; private set; }
        public ReactiveCommand<string, ClubParser> ParseClubs { get; private set; }
        public ReactiveCommand<string, NameParser> ParseFirstName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseSecondName { get; private set; }
        public ReactiveCommand<string, NameParser> ParseCommonName { get; private set; }
        public ReactiveCommand<string, PeopleParser> ParsePeople { get; private set; }
        public ReactiveCommand<string, PlayerParser> ParsePlayers { get; private set; }

        // Commands for Add/Edit
        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<PeopleDisplayModel, Unit> EditCommand { get; private set; }

        [Reactive] public PeopleDisplayModel? SelectedPerson { get; set; }

        [Reactive] public ISnackbarMessageQueue MessageQueue { get; set; }

        private readonly ICollectionView peopleView;
        private readonly IDialogService dialogService;

        // Lookup dictionaries for performance
        private Dictionary<int, string> firstNameLookup = [];
        private Dictionary<int, string> lastNameLookup = [];
        private Dictionary<int, string> commonNameLookup = [];
        private Dictionary<short, string> nationLookup = [];
        private Dictionary<int, string> clubLookup = [];
        private Dictionary<int, Player> playerLookup = [];

        public PeopleViewModel(IDialogService dialogService, ISnackbarMessageQueue messageQueue)
        {
            this.dialogService = dialogService;
            MessageQueue = messageQueue;

            ClearSearch = ReactiveCommand.Create(() => { SearchQuery = ""; }, outputScheduler: RxApp.MainThreadScheduler);

            peopleView = CollectionViewSource.GetDefaultView(PeopleList);
            peopleView.Filter = obj =>
            {
                if (obj is PeopleDisplayModel person)
                {
                    return string.IsNullOrEmpty(SearchQuery)
                        || person.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || person.FirstName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || person.LastName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || person.NationName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                        || person.ClubName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
                }
                return true;
            };

            Load = ReactiveCommand.Create(LoadImpl);
            Load.ToPropertyEx(this, vm => vm.FolderPath);

            ParseNations = ReactiveCommand.CreateFromTask<string, NationParser>(NationParser.Load);
            ParseNations.ToPropertyEx(this, vm => vm.NationParser);

            ParseClubs = ReactiveCommand.CreateFromTask<string, ClubParser>(ClubParser.Load);
            ParseClubs.ToPropertyEx(this, vm => vm.ClubParser);

            ParseFirstName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseFirstName.ToPropertyEx(this, vm => vm.FirstNameParser);

            ParseSecondName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseSecondName.ToPropertyEx(this, vm => vm.SecondNameParser);

            ParseCommonName = ReactiveCommand.CreateFromTask<string, NameParser>(NameParser.Load);
            ParseCommonName.ToPropertyEx(this, vm => vm.CommonNameParser);

            ParsePeople = ReactiveCommand.CreateFromTask<string, PeopleParser>(PeopleParser.Load);
            ParsePeople.ToPropertyEx(this, vm => vm.PeopleParser);

            ParsePlayers = ReactiveCommand.CreateFromTask<string, PlayerParser>(PlayerParser.Load);
            ParsePlayers.ToPropertyEx(this, vm => vm.PlayerParser);

            AddCommand = ReactiveCommand.CreateFromTask(OpenAddPersonDialogAsync);
            EditCommand = ReactiveCommand.CreateFromTask<PeopleDisplayModel>(OpenEditPersonDialogAsync);

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
                .WhereNotNull()
                .Select(x => x + "\\people.dat")
                .InvokeCommand(ParsePeople);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\players.dat")
                .InvokeCommand(ParsePlayers);

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
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.ClubParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Clubs.Reset(x.Items.OrderBy(y => y.FullName));
                    clubLookup = x.Items.ToDictionary(c => c.Id, c => c.FullName);
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.FirstNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    FirstNames.Reset(x.Items.OrderBy(y => y.Value));
                    firstNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.SecondNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    LastNames.Reset(x.Items.OrderBy(y => y.Value));
                    lastNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.CommonNameParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    CommonNames.Reset(x.Items.OrderBy(y => y.Value));
                    commonNameLookup = x.Items.ToDictionary(n => n.Id, n => n.Value);
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.PeopleParser)
                .WhereNotNull()
                .Subscribe(x => RefreshPeopleDisplay());

            this.WhenAnyValue(vm => vm.PlayerParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    playerLookup = x.Items.ToDictionary(p => p.Id);
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => peopleView.Refresh());
        }

        private async Task OpenAddPersonDialogAsync()
        {
            var viewModel = new PersonEditViewModel(FirstNames, LastNames, CommonNames, Nations, Clubs);
            viewModel.InitializeForAdd();

            var view = new PersonEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "PeopleDialogHost");

            if (result is PersonEditViewModel vm && vm.Validate())
            {
                AddPerson(vm);
                RefreshPeopleDisplay();
            }
        }

        private async Task OpenEditPersonDialogAsync(PeopleDisplayModel? person)
        {
            if (person == null) return;

            var viewModel = new PersonEditViewModel(FirstNames, LastNames, CommonNames, Nations, Clubs);
            viewModel.InitializeForEdit(person);

            var view = new PersonEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "PeopleDialogHost");

            if (result is PersonEditViewModel vm && vm.Validate())
            {
                UpdatePerson(vm);
                RefreshPeopleDisplay();
            }
        }

        private void AddPerson(PersonEditViewModel vm)
        {
            var nextUid = PeopleParser?.Items.Max(x => x.Uid) + 1 ?? 1;

            var newPerson = new People
            {
                Id = -1,
                Uid = nextUid,
                FirstNameId = vm.FirstNameId!.Value,
                LastNameId = vm.LastNameId!.Value,
                CommonNameId = vm.CommonNameId ?? -1,
                DateOfBirth = DateConverter.FromDateTime(vm.DateOfBirth),
                NationId = vm.NationId!.Value,
                OtherNationalities = [],
                ClubId = vm.ClubId ?? -1,
                Type = (byte)vm.PersonType,
                NationalCaps = vm.NationalCaps ?? 0,
                NationalGoals = vm.NationalGoals ?? 0,
                NationalU21Caps = vm.NationalU21Caps ?? 0,
                NationalU21Goals = vm.NationalU21Goals ?? 0,
                Adaptability = vm.Adaptability ?? 10,
                Ambition = vm.Ambition ?? 10,
                Loyality = vm.Loyalty ?? 10,
                Pressure = vm.Pressure ?? 10,
                Professionalism = vm.Professionalism ?? 10,
                Temperament = vm.Temperament ?? 10,
                Ethnicity = vm.Ethnicity,
                Unknown1 = 0,
                UnknownDate = 0,
                JoinedDate = 0,
                Unknown3 = 0,
                Controversy = 10,
                Sportmanship = 10,
                PlayerId = -1,
                Unknown6b = -1,
                Unknown6c = -1,
                Unknown6d = -1,
                Unknown6e = -1,
                Unknown6f = -1,
                Unknown7 = 0,
                Unknown8 = 0,
                DefaultLanguages = [],
                OtherLanguages = [],
                Relationships = [],
                Unknown21 = 0
            };

            PeopleParser?.Add(newPerson);
            MessageQueue.Enqueue("Person added successfully");
        }

        private void UpdatePerson(PersonEditViewModel vm)
        {
            var existingPerson = PeopleParser?.Items.FirstOrDefault(x => x.Uid == vm.Uid);
            if (existingPerson == null) return;

            existingPerson.FirstNameId = vm.FirstNameId!.Value;
            existingPerson.LastNameId = vm.LastNameId!.Value;
            existingPerson.CommonNameId = vm.CommonNameId ?? -1;
            existingPerson.DateOfBirth = DateConverter.FromDateTime(vm.DateOfBirth);
            existingPerson.NationId = vm.NationId!.Value;
            existingPerson.ClubId = vm.ClubId ?? -1;
            existingPerson.Type = (byte)vm.PersonType;
            existingPerson.Ethnicity = vm.Ethnicity;
            existingPerson.NationalCaps = vm.NationalCaps ?? 0;
            existingPerson.NationalGoals = vm.NationalGoals ?? 0;
            existingPerson.NationalU21Caps = vm.NationalU21Caps ?? 0;
            existingPerson.NationalU21Goals = vm.NationalU21Goals ?? 0;
            existingPerson.Adaptability = vm.Adaptability ?? 10;
            existingPerson.Ambition = vm.Ambition ?? 10;
            existingPerson.Loyality = vm.Loyalty ?? 10;
            existingPerson.Pressure = vm.Pressure ?? 10;
            existingPerson.Professionalism = vm.Professionalism ?? 10;
            existingPerson.Temperament = vm.Temperament ?? 10;

            MessageQueue.Enqueue("Person updated successfully");
        }

        private void RefreshPeopleDisplay()
        {
            if (PeopleParser == null) return;

            var displayList = PeopleParser.Items.Select(p =>
            {
                var display = new PeopleDisplayModel(p)
                {
                    FirstName = firstNameLookup.GetValueOrDefault(p.FirstNameId, ""),
                    LastName = lastNameLookup.GetValueOrDefault(p.LastNameId, ""),
                    CommonName = commonNameLookup.GetValueOrDefault(p.CommonNameId, ""),
                    NationName = nationLookup.GetValueOrDefault(p.NationId, ""),
                    ClubName = clubLookup.GetValueOrDefault(p.ClubId, "Free Agent"),
                    Player = playerLookup.GetValueOrDefault(p.PlayerId)
                };
                return display;
            });

            PeopleList.Reset(displayList);
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
                if (PeopleParser != null)
                {
                    await PeopleParser.Save();
                    MessageQueue.Enqueue("Save Successful");
                }
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
                if (PeopleParser != null)
                {
                    await PeopleParser.Save(settings.SelectedPath + "\\people.dat");
                    MessageQueue.Enqueue("Save Successful");
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
