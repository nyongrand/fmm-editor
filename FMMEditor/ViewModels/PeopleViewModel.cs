using FMMEditor.Collections;
using FMMEditor.Converters;
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

        #region Dialog

        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<PeopleDisplayModel, Unit> EditCommand { get; private set; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ConfirmCommand { get; private set; }

        [Reactive] public bool ShowDialog { get; set; }
        [Reactive] public PeopleDisplayModel? SelectedPerson { get; set; }

        // Dialog fields
        [Reactive] public int? SelectedUid { get; set; }
        [Reactive] public int? SelectedFirstNameId { get; set; }
        [Reactive] public int? SelectedLastNameId { get; set; }
        [Reactive] public int? SelectedCommonNameId { get; set; }
        [Reactive] public short? SelectedNationId { get; set; }
        [Reactive] public int? SelectedClubId { get; set; }
        [Reactive] public DateTime? SelectedDateOfBirth { get; set; }
        [Reactive] public int SelectedType { get; set; }
        [Reactive] public short? SelectedNationalCaps { get; set; }
        [Reactive] public short? SelectedNationalGoals { get; set; }
        [Reactive] public byte? SelectedNationalU21Caps { get; set; }
        [Reactive] public byte? SelectedNationalU21Goals { get; set; }
        [Reactive] public byte? SelectedAdaptability { get; set; }
        [Reactive] public byte? SelectedAmbition { get; set; }
        [Reactive] public byte? SelectedLoyalty { get; set; }
        [Reactive] public byte? SelectedPressure { get; set; }
        [Reactive] public byte? SelectedProfessionalism { get; set; }
        [Reactive] public byte? SelectedTemperament { get; set; }

        #endregion

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

            AddCommand = ReactiveCommand.Create(Add);
            EditCommand = ReactiveCommand.Create<PeopleDisplayModel>(Edit);

            CancelCommand = ReactiveCommand.Create(Cancel);
            ConfirmCommand = ReactiveCommand.Create(Confirm);

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
                .Select(x => x + "\\player.dat")
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

        private void Add()
        {
            SelectedUid = -1;
            SelectedFirstNameId = null;
            SelectedLastNameId = null;
            SelectedCommonNameId = null;
            SelectedNationId = null;
            SelectedClubId = null;
            SelectedDateOfBirth = new DateTime(1990, 1, 1);
            SelectedType = 0;
            SelectedNationalCaps = 0;
            SelectedNationalGoals = 0;
            SelectedNationalU21Caps = 0;
            SelectedNationalU21Goals = 0;
            SelectedAdaptability = 10;
            SelectedAmbition = 10;
            SelectedLoyalty = 10;
            SelectedPressure = 10;
            SelectedProfessionalism = 10;
            SelectedTemperament = 10;

            ShowDialog = true;
        }

        private void Edit(PeopleDisplayModel person)
        {
            if (person == null) return;

            var p = person.Person;
            SelectedUid = p.Uid;
            SelectedFirstNameId = p.FirstNameId;
            SelectedLastNameId = p.LastNameId;
            SelectedCommonNameId = p.CommonNameId > 0 ? p.CommonNameId : null;
            SelectedNationId = p.NationId;
            SelectedClubId = p.ClubId;
            SelectedDateOfBirth = DateConverter.ToDateTime(p.DateOfBirth);
            SelectedType = p.Type;
            SelectedNationalCaps = p.NationalCaps;
            SelectedNationalGoals = p.NationalGoals;
            SelectedNationalU21Caps = p.NationalU21Caps;
            SelectedNationalU21Goals = p.NationalU21Goals;
            SelectedAdaptability = p.Adaptability;
            SelectedAmbition = p.Ambition;
            SelectedLoyalty = p.Loyality;
            SelectedPressure = p.Pressure;
            SelectedProfessionalism = p.Professionalism;
            SelectedTemperament = p.Temperament;

            ShowDialog = true;
        }

        private void Cancel()
        {
            ResetDialogFields();
            ShowDialog = false;
        }

        private void Confirm()
        {
            if (SelectedFirstNameId == null || SelectedLastNameId == null || SelectedNationId == null)
            {
                MessageQueue.Enqueue("Please fill in required fields (First Name, Last Name, Nation)");
                return;
            }

            bool isAddMode = SelectedUid == -1;

            if (isAddMode)
            {
                // Create new person
                var nextUid = PeopleParser?.Items.Max(x => x.Uid) + 1 ?? 1;

                var newPerson = new People
                {
                    Id = -1,
                    Uid = nextUid,
                    FirstNameId = SelectedFirstNameId.Value,
                    LastNameId = SelectedLastNameId.Value,
                    CommonNameId = SelectedCommonNameId ?? -1,
                    DateOfBirth = DateConverter.FromDateTime(SelectedDateOfBirth),
                    NationId = SelectedNationId.Value,
                    OtherNationalities = [],
                    ClubId = SelectedClubId ?? -1,
                    Type = (byte)SelectedType,
                    NationalCaps = SelectedNationalCaps ?? 0,
                    NationalGoals = SelectedNationalGoals ?? 0,
                    NationalU21Caps = SelectedNationalU21Caps ?? 0,
                    NationalU21Goals = SelectedNationalU21Goals ?? 0,
                    Adaptability = SelectedAdaptability ?? 10,
                    Ambition = SelectedAmbition ?? 10,
                    Loyality = SelectedLoyalty ?? 10,
                    Pressure = SelectedPressure ?? 10,
                    Professionalism = SelectedProfessionalism ?? 10,
                    Temperament = SelectedTemperament ?? 10,
                    // Default values for other fields
                    Ethnicity = 0,
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
            else
            {
                // Edit existing person
                var existingPerson = PeopleParser?.Items.FirstOrDefault(x => x.Uid == SelectedUid);
                if (existingPerson != null)
                {
                    existingPerson.FirstNameId = SelectedFirstNameId.Value;
                    existingPerson.LastNameId = SelectedLastNameId.Value;
                    existingPerson.CommonNameId = SelectedCommonNameId ?? -1;
                    existingPerson.DateOfBirth = DateConverter.FromDateTime(SelectedDateOfBirth);
                    existingPerson.NationId = SelectedNationId.Value;
                    existingPerson.ClubId = SelectedClubId ?? -1;
                    existingPerson.Type = (byte)SelectedType;
                    existingPerson.NationalCaps = SelectedNationalCaps ?? 0;
                    existingPerson.NationalGoals = SelectedNationalGoals ?? 0;
                    existingPerson.NationalU21Caps = SelectedNationalU21Caps ?? 0;
                    existingPerson.NationalU21Goals = SelectedNationalU21Goals ?? 0;
                    existingPerson.Adaptability = SelectedAdaptability ?? 10;
                    existingPerson.Ambition = SelectedAmbition ?? 10;
                    existingPerson.Loyality = SelectedLoyalty ?? 10;
                    existingPerson.Pressure = SelectedPressure ?? 10;
                    existingPerson.Professionalism = SelectedProfessionalism ?? 10;
                    existingPerson.Temperament = SelectedTemperament ?? 10;

                    MessageQueue.Enqueue("Person updated successfully");
                }
            }

            RefreshPeopleDisplay();
            ResetDialogFields();
            ShowDialog = false;
        }

        private void ResetDialogFields()
        {
            SelectedUid = null;
            SelectedFirstNameId = null;
            SelectedLastNameId = null;
            SelectedCommonNameId = null;
            SelectedNationId = null;
            SelectedClubId = null;
            SelectedDateOfBirth = null;
            SelectedType = 0;
            SelectedNationalCaps = null;
            SelectedNationalGoals = null;
            SelectedNationalU21Caps = null;
            SelectedNationalU21Goals = null;
            SelectedAdaptability = null;
            SelectedAmbition = null;
            SelectedLoyalty = null;
            SelectedPressure = null;
            SelectedProfessionalism = null;
            SelectedTemperament = null;
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
