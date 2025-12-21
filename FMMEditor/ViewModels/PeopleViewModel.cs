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
using System.IO;
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
        public extern LanguageParser? LanguageParser { [ObservableAsProperty] get; }
        public extern AlwaysLoadParser? AlwaysLoadMaleParser { [ObservableAsProperty] get; }
        public extern AlwaysLoadParser? AlwaysLoadFemaleParser { [ObservableAsProperty] get; }

        // Collections for display
        public BulkObservableCollection<Nation> Nations { get; } = [];
        public BulkObservableCollection<Club> Clubs { get; } = [];
        public BulkObservableCollection<Name> FirstNames { get; } = [];
        public BulkObservableCollection<Name> LastNames { get; } = [];
        public BulkObservableCollection<Name> CommonNames { get; } = [];
        public BulkObservableCollection<PeopleDisplayModel> PeopleList { get; } = [];
        public BulkObservableCollection<Language> Languages { get; } = [];

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
        public ReactiveCommand<string, LanguageParser> ParseLanguages { get; private set; }
        public ReactiveCommand<string, AlwaysLoadParser> ParseAlwaysLoadMale { get; private set; }
        public ReactiveCommand<string, AlwaysLoadParser> ParseAlwaysLoadFemale { get; private set; }

        // Commands for Add/Edit
        public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
        public ReactiveCommand<PeopleDisplayModel, Unit> EditCommand { get; private set; }
        public ReactiveCommand<PeopleDisplayModel, Unit> CopyCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopyUidCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CopyUidHexCommand { get; private set; }

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
        private Dictionary<int, byte> firstNameGenderLookup = [];
        private HashSet<int> alwaysLoadMaleUids = [];
        private HashSet<int> alwaysLoadFemaleUids = [];

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

            ParseLanguages = ReactiveCommand.CreateFromTask<string, LanguageParser>(LanguageParser.Load);
            ParseLanguages.ToPropertyEx(this, vm => vm.LanguageParser);

            ParseAlwaysLoadMale = ReactiveCommand.CreateFromTask<string, AlwaysLoadParser>(AlwaysLoadParser.Load);
            ParseAlwaysLoadMale.ToPropertyEx(this, vm => vm.AlwaysLoadMaleParser);

            ParseAlwaysLoadFemale = ReactiveCommand.CreateFromTask<string, AlwaysLoadParser>(AlwaysLoadParser.Load);
            ParseAlwaysLoadFemale.ToPropertyEx(this, vm => vm.AlwaysLoadFemaleParser);

            AddCommand = ReactiveCommand.CreateFromTask(OpenAddPersonDialogAsync);
            EditCommand = ReactiveCommand.CreateFromTask<PeopleDisplayModel>(OpenEditPersonDialogAsync);
            CopyCommand = ReactiveCommand.CreateFromTask<PeopleDisplayModel>(OpenCopyPersonDialogAsync);
            CopyUidCommand = ReactiveCommand.Create(CopyUidToClipboard);
            CopyUidHexCommand = ReactiveCommand.Create(CopyUidHexToClipboard);

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
                .WhereNotNull()
                .Select(x => x + "\\languages.dat")
                .InvokeCommand(ParseLanguages);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\people_to_always_load_male.dat")
                .InvokeCommand(ParseAlwaysLoadMale);

            this.WhenAnyValue(vm => vm.FolderPath)
                .WhereNotNull()
                .Select(x => x + "\\people_to_always_load_female.dat")
                .InvokeCommand(ParseAlwaysLoadFemale);

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
                    firstNameGenderLookup = x.Items.ToDictionary(n => n.Id, n => n.Gender);
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

            this.WhenAnyValue(vm => vm.LanguageParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    Languages.Reset(x.Items.OrderBy(y => y.Name));
                });

            this.WhenAnyValue(vm => vm.AlwaysLoadMaleParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    alwaysLoadMaleUids = x.Items.ToHashSet();
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.AlwaysLoadFemaleParser)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    alwaysLoadFemaleUids = x.Items.ToHashSet();
                    RefreshPeopleDisplay();
                });

            this.WhenAnyValue(vm => vm.SearchQuery)
                .Subscribe(x => peopleView.Refresh());
        }

        private async Task OpenAddPersonDialogAsync()
        {
            var viewModel = new PersonEditViewModel(FirstNames, LastNames, CommonNames, Nations, Clubs, Languages);
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

            var viewModel = new PersonEditViewModel(FirstNames, LastNames, CommonNames, Nations, Clubs, Languages);
            viewModel.InitializeForEdit(person);

            var view = new PersonEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "PeopleDialogHost");

            if (result is PersonEditViewModel vm && vm.Validate())
            {
                UpdatePerson(vm);
                RefreshPeopleDisplay();
            }
        }

        private async Task OpenCopyPersonDialogAsync(PeopleDisplayModel? person)
        {
            if (person == null) return;

            var viewModel = new PersonEditViewModel(FirstNames, LastNames, CommonNames, Nations, Clubs, Languages);
            viewModel.InitializeForCopy(person);

            var view = new PersonEditView { DataContext = viewModel };

            var result = await DialogHost.Show(view, "PeopleDialogHost");

            if (result is PersonEditViewModel vm && vm.Validate())
            {
                AddPerson(vm);
                RefreshPeopleDisplay();
            }
        }

        private void CopyUidToClipboard()
        {
            if (SelectedPerson != null)
            {
                Clipboard.SetText(SelectedPerson.Uid.ToString());
                MessageQueue.Enqueue("UID copied to clipboard");
            }
        }

        private void CopyUidHexToClipboard()
        {
            if (SelectedPerson != null)
            {
                // Convert UID to little-endian hex bytes
                byte[] bytes = BitConverter.GetBytes(SelectedPerson.Uid);
                string hex = BitConverter.ToString(bytes).Replace("-", "");
                Clipboard.SetText(hex);
                MessageQueue.Enqueue($"UID hex copied: {hex}");
            }
        }

        private void AddPerson(PersonEditViewModel vm)
        {
            if (PeopleParser == null || PlayerParser == null) return;

            var nextUid = PeopleParser.Items.Count > 0 ? PeopleParser.Items.Max(x => x.Uid) + 1 : 1;

            // Always create a player when adding a new person
            var newPlayer = CreatePlayerFromViewModel(vm, nextUid);
            PlayerParser.Add(newPlayer);
            playerLookup[newPlayer.Id] = newPlayer;

            var clubId = vm.ClubId ?? -1;
            var joinedDate = clubId == -1 
                ? DateConverter.FromDateTime(new DateTime(2025, 5, 30)) 
                : DateConverter.FromDateTime(vm.JoinedDate);

            var newPerson = new People
            {
                Id = -1,
                Uid = nextUid,
                FirstNameId = vm.FirstNameId!.Value,
                LastNameId = vm.LastNameId!.Value,
                CommonNameId = vm.CommonNameId ?? -1,
                DateOfBirth = DateConverter.FromDateTime(vm.DateOfBirth),
                NationId = vm.NationId!.Value,
                OtherNationalities = vm.OtherNationalities.Select(n => n.NationId).ToList(),
                ClubId = clubId,
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
                Unknown1 = vm.Unknown1,
                UnknownDate = DateConverter.FromDateTime(vm.UnknownDate),
                JoinedDate = joinedDate,
                Unknown3 = 0,
                Controversy = vm.Controversy ?? 10,
                Sportmanship = vm.Sportmanship ?? 10,
                PlayerId = newPlayer.Id,
                Unknown6b = -1,
                Unknown6c = -1,
                Unknown6d = -1,
                Unknown6e = -1,
                Unknown6f = -1,
                Unknown7 = 0,
                Unknown8 = 0,
                DefaultLanguages = vm.DefaultLanguages.Select(l => (l.LanguageId, l.Proficiency)).ToArray(),
                OtherLanguages = vm.OtherLanguages.Select(l => (l.LanguageId, l.Proficiency)).ToArray(),
                Relationships = [],
                Unknown21 = 0
            };

            PeopleParser.Add(newPerson);
            MessageQueue.Enqueue("Person and player added successfully");
        }

        private void UpdatePerson(PersonEditViewModel vm)
        {
            var existingPerson = PeopleParser?.Items.FirstOrDefault(x => x.Uid == vm.Uid);
            if (existingPerson == null) return;

            var clubId = vm.ClubId ?? -1;
            var joinedDate = clubId == -1 
                ? DateConverter.FromDateTime(new DateTime(2025, 5, 30)) 
                : DateConverter.FromDateTime(vm.JoinedDate);

            existingPerson.Id = -1;  // Always save Id as -1
            existingPerson.FirstNameId = vm.FirstNameId!.Value;
            existingPerson.LastNameId = vm.LastNameId!.Value;
            existingPerson.CommonNameId = vm.CommonNameId ?? -1;
            existingPerson.DateOfBirth = DateConverter.FromDateTime(vm.DateOfBirth);
            existingPerson.NationId = vm.NationId!.Value;
            existingPerson.OtherNationalities = vm.OtherNationalities.Select(n => n.NationId).ToList();
            existingPerson.ClubId = clubId;
            existingPerson.JoinedDate = joinedDate;
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
            existingPerson.Controversy = vm.Controversy ?? 10;
            existingPerson.Sportmanship = vm.Sportmanship ?? 10;

            // Update languages
            existingPerson.DefaultLanguages = vm.DefaultLanguages.Select(l => (l.LanguageId, l.Proficiency)).ToArray();
            existingPerson.OtherLanguages = vm.OtherLanguages.Select(l => (l.LanguageId, l.Proficiency)).ToArray();

            // Handle player data
            if (vm.HasPlayer && PlayerParser != null)
            {
                var existingPlayer = playerLookup.GetValueOrDefault(existingPerson.PlayerId);
                if (existingPlayer != null)
                {
                    // Update existing player
                    UpdatePlayerFromViewModel(existingPlayer, vm);
                }
                else
                {
                    // Create new player for existing person
                    var nextPlayerUid = PlayerParser.Items.Count > 0 ? PlayerParser.Items.Max(x => x.Uid) + 1 : 1;
                    var newPlayer = CreatePlayerFromViewModel(vm, nextPlayerUid);
                    PlayerParser.Add(newPlayer);
                    existingPerson.PlayerId = newPlayer.Id;
                    playerLookup[newPlayer.Id] = newPlayer;
                }
            }

            MessageQueue.Enqueue("Person updated successfully");
        }

        private Player CreatePlayerFromViewModel(PersonEditViewModel vm, int uid)
        {
            return new Player
            {
                Id = -1,
                Uid = uid,
                CA = vm.CA ?? 0,
                PA = vm.PA ?? 0,
                Height = vm.Height ?? 180,
                Weight = vm.Weight ?? 75,
                Pace = vm.Pace ?? 10,
                Stamina = vm.Stamina ?? 10,
                Strength = vm.Strength ?? 10,
                Agility = vm.Agility ?? 10,
                Jumping = vm.Jumping ?? 10,
                Finishing = vm.Finishing ?? 10,
                Passing = vm.Passing ?? 10,
                Dribbling = vm.Dribbling ?? 10,
                Tackling = vm.Tackling ?? 10,
                Heading = vm.Heading ?? 10,
                Crossing = vm.Crossing ?? 10,
                Technique = vm.Technique ?? 10,
                LongShot = vm.LongShot ?? 10,
                SetPieces = vm.SetPieces ?? 10,
                Creativity = vm.Creativity ?? 10,
                Leadership = vm.Leadership ?? 10,
                WorkRate = vm.WorkRate ?? 10,
                Positioning = vm.Positioning ?? 10,
                Movement = vm.Movement ?? 10,
                Flair = vm.Flair ?? 10,
                Decision = vm.Decision ?? 10,
                Unselfishness = vm.Unselfishness ?? 10,
                Consistency = vm.Consistency ?? 10,
                Aggression = vm.Aggression ?? 10,
                BigMatch = vm.BigMatch ?? 10,
                InjuryProne = vm.InjuryProne ?? 10,
                Versatility = vm.Versatility ?? 10,
                Penalty = vm.Penalty ?? 10,
                LeftFoot = vm.LeftFoot ?? 10,
                RightFoot = vm.RightFoot ?? 10,
                Handling = vm.Handling ?? 10,
                Kicking = vm.Kicking ?? 10,
                Aerial = vm.Aerial ?? 10,
                Reflexes = vm.Reflexes ?? 10,
                Communication = vm.Communication ?? 10,
                Throwing = vm.Throwing ?? 10,
                GK = vm.GK ?? 1,
                LIB = vm.LIB ?? 1,
                LB = vm.LB ?? 1,
                CB = vm.CB ?? 1,
                RB = vm.RB ?? 1,
                DM = vm.DM ?? 1,
                LM = vm.LM ?? 1,
                CM = vm.CM ?? 1,
                RM = vm.RM ?? 1,
                LW = vm.LW ?? 1,
                AM = vm.AM ?? 1,
                RW = vm.RW ?? 1,
                CF = vm.CF ?? 1,
                LWB = vm.LWB ?? 1,
                RWB = vm.RWB ?? 1,
                HomeReputation = vm.HomeReputation ?? 0,
                CurrentReputation = vm.CurrentReputation ?? 0,
                WorldReputation = vm.WorldReputation ?? 0,
                InternationalRetirement = vm.InternationalRetirement ?? 0,
                SquadNumber = vm.SquadNumber ?? 0,
                PreferredSquadNumber = vm.PreferredSquadNumber ?? 0,
                Unknown1 = 0,
                Unknown2 = 0
            };
        }

        private void UpdatePlayerFromViewModel(Player player, PersonEditViewModel vm)
        {
            player.CA = vm.CA ?? player.CA;
            player.PA = vm.PA ?? player.PA;
            player.Height = vm.Height ?? player.Height;
            player.Weight = vm.Weight ?? player.Weight;
            player.Pace = vm.Pace ?? player.Pace;
            player.Stamina = vm.Stamina ?? player.Stamina;
            player.Strength = vm.Strength ?? player.Strength;
            player.Agility = vm.Agility ?? player.Agility;
            player.Jumping = vm.Jumping ?? player.Jumping;
            player.Finishing = vm.Finishing ?? player.Finishing;
            player.Passing = vm.Passing ?? player.Passing;
            player.Dribbling = vm.Dribbling ?? player.Dribbling;
            player.Tackling = vm.Tackling ?? player.Tackling;
            player.Heading = vm.Heading ?? player.Heading;
            player.Crossing = vm.Crossing ?? player.Crossing;
            player.Technique = vm.Technique ?? player.Technique;
            player.LongShot = vm.LongShot ?? player.LongShot;
            player.SetPieces = vm.SetPieces ?? player.SetPieces;
            player.Creativity = vm.Creativity ?? player.Creativity;
            player.Leadership = vm.Leadership ?? player.Leadership;
            player.WorkRate = vm.WorkRate ?? player.WorkRate;
            player.Positioning = vm.Positioning ?? player.Positioning;
            player.Movement = vm.Movement ?? player.Movement;
            player.Flair = vm.Flair ?? player.Flair;
            player.Decision = vm.Decision ?? player.Decision;
            player.Unselfishness = vm.Unselfishness ?? player.Unselfishness;
            player.Consistency = vm.Consistency ?? player.Consistency;
            player.Aggression = vm.Aggression ?? player.Aggression;
            player.BigMatch = vm.BigMatch ?? player.BigMatch;
            player.InjuryProne = vm.InjuryProne ?? player.InjuryProne;
            player.Versatility = vm.Versatility ?? player.Versatility;
            player.Penalty = vm.Penalty ?? player.Penalty;
            player.LeftFoot = vm.LeftFoot ?? player.LeftFoot;
            player.RightFoot = vm.RightFoot ?? player.RightFoot;
            player.Handling = vm.Handling ?? player.Handling;
            player.Kicking = vm.Kicking ?? player.Kicking;
            player.Aerial = vm.Aerial ?? player.Aerial;
            player.Reflexes = vm.Reflexes ?? player.Reflexes;
            player.Communication = vm.Communication ?? player.Communication;
            player.Throwing = vm.Throwing ?? player.Throwing;
            player.GK = vm.GK ?? player.GK;
            player.LIB = vm.LIB ?? player.LIB;
            player.LB = vm.LB ?? player.LB;
            player.CB = vm.CB ?? player.CB;
            player.RB = vm.RB ?? player.RB;
            player.DM = vm.DM ?? player.DM;
            player.LM = vm.LM ?? player.LM;
            player.CM = vm.CM ?? player.CM;
            player.RM = vm.RM ?? player.RM;
            player.LW = vm.LW ?? player.LW;
            player.AM = vm.AM ?? player.AM;
            player.RW = vm.RW ?? player.RW;
            player.CF = vm.CF ?? player.CF;
            player.LWB = vm.LWB ?? player.LWB;
            player.RWB = vm.RWB ?? player.RWB;
            player.HomeReputation = vm.HomeReputation ?? player.HomeReputation;
            player.CurrentReputation = vm.CurrentReputation ?? player.CurrentReputation;
            player.WorldReputation = vm.WorldReputation ?? player.WorldReputation;
            player.InternationalRetirement = vm.InternationalRetirement ?? player.InternationalRetirement;
            player.SquadNumber = vm.SquadNumber ?? player.SquadNumber;
            player.PreferredSquadNumber = vm.PreferredSquadNumber ?? player.PreferredSquadNumber;
        }

        private void RefreshPeopleDisplay()
        {
            if (PeopleParser == null) return;

            var displayList = PeopleParser.Items.Select(p =>
            {
                var gender = firstNameGenderLookup.GetValueOrDefault(p.FirstNameId, (byte)0);
                var isMale = gender == 0;
                var display = new PeopleDisplayModel(p)
                {
                    FirstName = firstNameLookup.GetValueOrDefault(p.FirstNameId, ""),
                    LastName = lastNameLookup.GetValueOrDefault(p.LastNameId, ""),
                    CommonName = commonNameLookup.GetValueOrDefault(p.CommonNameId, ""),
                    NationName = nationLookup.GetValueOrDefault(p.NationId, ""),
                    ClubName = clubLookup.GetValueOrDefault(p.ClubId, "Free Agent"),
                    Player = playerLookup.GetValueOrDefault(p.PlayerId),
                    Gender = gender,
                    IsAlwaysLoad = isMale ? alwaysLoadMaleUids.Contains(p.Uid) : alwaysLoadFemaleUids.Contains(p.Uid)
                };
                
                // Subscribe to changes in IsAlwaysLoad
                display.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(PeopleDisplayModel.IsAlwaysLoad) && s is PeopleDisplayModel model)
                    {
                        OnAlwaysLoadChanged(model);
                    }
                };
                
                return display;
            });

            PeopleList.Reset(displayList);
        }

        private void OnAlwaysLoadChanged(PeopleDisplayModel person)
        {
            var isMale = person.Gender == 0;
            var targetSet = isMale ? alwaysLoadMaleUids : alwaysLoadFemaleUids;

            if (person.IsAlwaysLoad)
            {
                if (!targetSet.Contains(person.Uid))
                {
                    targetSet.Add(person.Uid);
                }
            }
            else
            {
                if (targetSet.Contains(person.Uid))
                {
                    targetSet.Remove(person.Uid);
                }
            }
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
                }
                if (PlayerParser != null)
                {
                    await PlayerParser.Save();
                }
                if (AlwaysLoadMaleParser != null)
                {
                    await SaveAlwaysLoadParser(AlwaysLoadMaleParser, alwaysLoadMaleUids);
                }
                if (AlwaysLoadFemaleParser != null)
                {
                    await SaveAlwaysLoadParser(AlwaysLoadFemaleParser, alwaysLoadFemaleUids);
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
                if (PeopleParser != null)
                {
                    await PeopleParser.Save(settings.SelectedPath + "\\people.dat");
                }
                if (PlayerParser != null)
                {
                    await PlayerParser.Save(settings.SelectedPath + "\\players.dat");
                }
                if (AlwaysLoadMaleParser != null)
                {
                    await SaveAlwaysLoadParser(AlwaysLoadMaleParser, alwaysLoadMaleUids, settings.SelectedPath + "\\people_to_always_load_male.dat");
                }
                if (AlwaysLoadFemaleParser != null)
                {
                    await SaveAlwaysLoadParser(AlwaysLoadFemaleParser, alwaysLoadFemaleUids, settings.SelectedPath + "\\people_to_always_load_female.dat");
                }
                MessageQueue.Enqueue("Save Successful");
            }
            catch (Exception e)
            {
                dialogService.ShowMessageBox(this, $"Save error: {e.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveAlwaysLoadParser(AlwaysLoadParser parser, HashSet<int> uids, string? filePath = null)
        {
            if (parser == null) return;

            var bytes = ToAlwaysLoadBytes(parser, uids);
            await File.WriteAllBytesAsync(filePath ?? parser.FilePath, bytes);
        }

        private byte[] ToAlwaysLoadBytes(AlwaysLoadParser parser, HashSet<int> uids)
        {
            if (parser == null) return [];

            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);

            writer.Write(parser.Header);
            writer.Write(uids.Count);
            writer.Write((int)0);

            foreach (var uid in uids.OrderBy(x => x))
                writer.Write(uid);

            return stream.ToArray();
        }
    }
}
