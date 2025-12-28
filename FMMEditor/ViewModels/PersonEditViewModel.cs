using FMMEditor.Collections;
using FMMEditor.Converters;
using FMMEditor.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace FMMEditor.ViewModels
{
    public class PersonEditViewModel : ReactiveObject
    {
        // Mode
        [Reactive] public bool IsAddMode { get; set; }
        public string WindowTitle => IsAddMode ? "Add New Person" : "Edit Person";

        // Collections for ComboBoxes
        public BulkObservableCollection<Name> FirstNames { get; }
        public BulkObservableCollection<Name> LastNames { get; }
        public BulkObservableCollection<Name> CommonNames { get; }
        public BulkObservableCollection<Nation> Nations { get; }
        public List<ClubOption> Clubs { get; }

        // Language collection
        public BulkObservableCollection<Language> AvailableLanguages { get; }

        // Ethnicity options
        public List<EthnicityOption> EthnicityOptions { get; } = Enum.GetValues<Ethnicity>()
            .Select(e => new EthnicityOption { Value = (byte)e, DisplayName = FormatEthnicityName(e) })
            .ToList();

        // Language commands
        public ReactiveCommand<Unit, Unit> AddDefaultLanguageCommand { get; }
        public ReactiveCommand<LanguageEntry, Unit> RemoveDefaultLanguageCommand { get; }
        public ReactiveCommand<Unit, Unit> AddOtherLanguageCommand { get; }
        public ReactiveCommand<LanguageEntry, Unit> RemoveOtherLanguageCommand { get; }

        // Other nationality commands
        public ReactiveCommand<Unit, Unit> AddOtherNationalityCommand { get; }
        public ReactiveCommand<NationalityEntry, Unit> RemoveOtherNationalityCommand { get; }

        // Person fields
        [Reactive] public int? Uid { get; set; }
        [Reactive] public int? FirstNameId { get; set; }
        [Reactive] public int? LastNameId { get; set; }
        [Reactive] public int? CommonNameId { get; set; }
        [Reactive] public short? NationId { get; set; }
        [Reactive] public short? SecondNationality { get; set; }
        [Reactive] public short? ThirdNationality { get; set; }
        [Reactive] public short OtherNationalityCount { get; set; }
        [Reactive] public int? ClubId { get; set; }
        [Reactive] public DateTime? JoinedDate { get; set; }
        [Reactive] public DateTime? DateOfBirth { get; set; }
        [Reactive] public int PersonType { get; set; }
        [Reactive] public int Unknown1 { get; set; }
        [Reactive] public DateTime? UnknownDate { get; set; }
        [Reactive] public byte Ethnicity { get; set; }
        [Reactive] public short? NationalCaps { get; set; }
        [Reactive] public short? NationalGoals { get; set; }
        [Reactive] public byte? NationalU21Caps { get; set; }
        [Reactive] public byte? NationalU21Goals { get; set; }

        // Other nationalities (observable collection for UI binding)
        public ObservableCollection<NationalityEntry> OtherNationalities { get; } = [];

        // Personality attributes
        [Reactive] public byte? Adaptability { get; set; }
        [Reactive] public byte? Ambition { get; set; }
        [Reactive] public byte? Controversy { get; set; }
        [Reactive] public byte? Loyalty { get; set; }
        [Reactive] public byte? Pressure { get; set; }
        [Reactive] public byte? Professionalism { get; set; }
        [Reactive] public byte? Sportmanship { get; set; }
        [Reactive] public byte? Temperament { get; set; }

        // Language and relationship counts
        [Reactive] public byte? DefaultLanguageCount { get; set; }
        [Reactive] public byte? OtherLanguageCount { get; set; }
        [Reactive] public byte? RelationshipCount { get; set; }

        // Language entries (observable collections for UI binding)
        public ObservableCollection<LanguageEntry> DefaultLanguages { get; } = [];
        public ObservableCollection<LanguageEntry> OtherLanguages { get; } = [];

        // Player fields
        [Reactive] public bool HasPlayer { get; set; }
        [Reactive] public short? CA { get; set; }
        [Reactive] public short? PA { get; set; }
        [Reactive] public short? Height { get; set; }
        [Reactive] public short? Weight { get; set; }

        // Physical attributes
        [Reactive] public byte? Pace { get; set; }
        [Reactive] public byte? Stamina { get; set; }
        [Reactive] public byte? Strength { get; set; }
        [Reactive] public byte? Agility { get; set; }
        [Reactive] public byte? Jumping { get; set; }

        // Technical attributes
        [Reactive] public byte? Finishing { get; set; }
        [Reactive] public byte? Passing { get; set; }
        [Reactive] public byte? Dribbling { get; set; }
        [Reactive] public byte? Tackling { get; set; }
        [Reactive] public byte? Heading { get; set; }
        [Reactive] public byte? Crossing { get; set; }
        [Reactive] public byte? Technique { get; set; }
        [Reactive] public byte? LongShot { get; set; }
        [Reactive] public byte? SetPieces { get; set; }

        // Mental attributes
        [Reactive] public byte? Creativity { get; set; }
        [Reactive] public byte? Leadership { get; set; }
        [Reactive] public byte? WorkRate { get; set; }
        [Reactive] public byte? Positioning { get; set; }
        [Reactive] public byte? Movement { get; set; }
        [Reactive] public byte? Flair { get; set; }
        [Reactive] public byte? Decision { get; set; }
        [Reactive] public byte? Unselfishness { get; set; }
        [Reactive] public byte? Consistency { get; set; }
        [Reactive] public byte? Aggression { get; set; }
        [Reactive] public byte? BigMatch { get; set; }
        [Reactive] public byte? InjuryProne { get; set; }
        [Reactive] public byte? Versatility { get; set; }
        [Reactive] public byte? Penalty { get; set; }

        // Foot preference
        [Reactive] public byte? LeftFoot { get; set; }
        [Reactive] public byte? RightFoot { get; set; }

        // Goalkeeper attributes
        [Reactive] public byte? Handling { get; set; }
        [Reactive] public byte? Kicking { get; set; }
        [Reactive] public byte? Aerial { get; set; }
        [Reactive] public byte? Reflexes { get; set; }
        [Reactive] public byte? Communication { get; set; }
        [Reactive] public byte? Throwing { get; set; }

        // Position ratings
        [Reactive] public byte? GK { get; set; }
        [Reactive] public byte? LIB { get; set; }
        [Reactive] public byte? LB { get; set; }
        [Reactive] public byte? CB { get; set; }
        [Reactive] public byte? RB { get; set; }
        [Reactive] public byte? DM { get; set; }
        [Reactive] public byte? LM { get; set; }
        [Reactive] public byte? CM { get; set; }
        [Reactive] public byte? RM { get; set; }
        [Reactive] public byte? LW { get; set; }
        [Reactive] public byte? AM { get; set; }
        [Reactive] public byte? RW { get; set; }
        [Reactive] public byte? CF { get; set; }
        [Reactive] public byte? LWB { get; set; }
        [Reactive] public byte? RWB { get; set; }

        // Reputation
        [Reactive] public short? HomeReputation { get; set; }
        [Reactive] public short? CurrentReputation { get; set; }
        [Reactive] public short? WorldReputation { get; set; }

        // Other player fields
        [Reactive] public byte? InternationalRetirement { get; set; }
        [Reactive] public byte? SquadNumber { get; set; }
        [Reactive] public byte? PreferredSquadNumber { get; set; }

        public PersonEditViewModel(
            BulkObservableCollection<Name> firstNames,
            BulkObservableCollection<Name> lastNames,
            BulkObservableCollection<Name> commonNames,
            BulkObservableCollection<Nation> nations,
            BulkObservableCollection<Club> clubs,
            BulkObservableCollection<Language> languages)
        {
            FirstNames = firstNames;
            LastNames = lastNames;
            CommonNames = commonNames;
            Nations = nations;
            Clubs = [new ClubOption(-1, "Free Agent"), .. clubs.Select(x => new ClubOption(x.Id, x.FullName))];
            AvailableLanguages = languages;

            // Initialize language commands
            AddDefaultLanguageCommand = ReactiveCommand.Create(AddDefaultLanguage);
            RemoveDefaultLanguageCommand = ReactiveCommand.Create<LanguageEntry>(RemoveDefaultLanguage);
            AddOtherLanguageCommand = ReactiveCommand.Create(AddOtherLanguage);
            RemoveOtherLanguageCommand = ReactiveCommand.Create<LanguageEntry>(RemoveOtherLanguage);

            // Initialize other nationality commands
            AddOtherNationalityCommand = ReactiveCommand.Create(AddOtherNationality);
            RemoveOtherNationalityCommand = ReactiveCommand.Create<NationalityEntry>(RemoveOtherNationality);

            this.WhenAnyValue(x => x.IsAddMode)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

            OtherNationalities.CollectionChanged += (_, _) => UpdateOtherNationalityFieldsFromCollection();
            this.WhenAnyValue(x => x.SecondNationality, x => x.ThirdNationality)
                .Subscribe(_ => UpdateOtherNationalitiesFromFields());
        }

        private void AddDefaultLanguage()
        {
            DefaultLanguages.Add(new LanguageEntry { LanguageId = 0, Proficiency = 20 });
        }

        private void RemoveDefaultLanguage(LanguageEntry entry)
        {
            if (entry != null)
                DefaultLanguages.Remove(entry);
        }

        private void AddOtherLanguage()
        {
            OtherLanguages.Add(new LanguageEntry { LanguageId = 0, Proficiency = 20 });
        }

        private void RemoveOtherLanguage(LanguageEntry entry)
        {
            if (entry != null)
                OtherLanguages.Remove(entry);
        }

        private void AddOtherNationality()
        {
            if (SecondNationality == null)
            {
                SecondNationality = 0;
            }
            else if (ThirdNationality == null)
            {
                ThirdNationality = 0;
            }
        }

        private void RemoveOtherNationality(NationalityEntry entry)
        {
            if (entry == null)
                return;

            if (OtherNationalities.Contains(entry))
            {
                OtherNationalities.Remove(entry);
                return;
            }

            if (SecondNationality.HasValue && entry.NationId == SecondNationality)
            {
                SecondNationality = ThirdNationality;
                ThirdNationality = null;
            }
            else if (ThirdNationality.HasValue && entry.NationId == ThirdNationality)
            {
                ThirdNationality = null;
            }
        }

        public void InitializeForAdd()
        {
            IsAddMode = true;
            ResetToDefaults();
        }

        public void InitializeForEdit(PeopleDisplayModel person)
        {
            if (person == null) return;

            IsAddMode = false;
            LoadFromPerson(person);
        }

        public void InitializeForCopy(PeopleDisplayModel person)
        {
            if (person == null) return;

            IsAddMode = true;
            LoadFromPerson(person);

            // Reset IDs to generate new ones
            Uid = null;
        }

        private void LoadFromPerson(PeopleDisplayModel person)
        {
            var p = person.Person;

            Uid = p.Uid;
            FirstNameId = p.FirstNameId;
            LastNameId = p.LastNameId;
            CommonNameId = p.CommonNameId > 0 ? p.CommonNameId : null;
            NationId = p.NationId;
            ClubId = p.ClubId;
            JoinedDate = DateConverter.ToDateTime(p.JoinedDate);
            DateOfBirth = DateConverter.ToDateTime(p.DateOfBirth);
            PersonType = p.Type;
            Unknown1 = p.Unknown1;
            UnknownDate = DateConverter.ToDateTime(p.UnknownDate);
            Ethnicity = p.Ethnicity;
            NationalCaps = p.NationalCaps;
            NationalGoals = p.NationalGoals;
            NationalU21Caps = p.NationalU21Caps;
            NationalU21Goals = p.NationalU21Goals;
            Adaptability = p.Adaptability;
            Ambition = p.Ambition;
            Controversy = p.Controversy;
            Loyalty = p.Loyality;
            Pressure = p.Pressure;
            Professionalism = p.Professionalism;
            Sportmanship = p.Sportmanship;
            Temperament = p.Temperament;
            DefaultLanguageCount = p.DefaultLanguageCount;
            OtherLanguageCount = p.OtherLanguageCount;
            RelationshipCount = p.RelationshipCount;
            OtherNationalityCount = p.OtherNationalityCount;

            // Load other nationalities
            using (new OtherNationalitySyncScope(this))
            {
                OtherNationalities.Clear();
                foreach (var nationId in p.OtherNationalities)
                {
                    OtherNationalities.Add(new NationalityEntry { NationId = nationId });
                }
            }
            UpdateOtherNationalityFieldsFromCollection();

            // Load language entries
            DefaultLanguages.Clear();
            foreach (var lang in p.DefaultLanguages)
            {
                DefaultLanguages.Add(new LanguageEntry { LanguageId = lang.Id, Proficiency = lang.Proficiency });
            }

            OtherLanguages.Clear();
            foreach (var lang in p.OtherLanguages)
            {
                OtherLanguages.Add(new LanguageEntry { LanguageId = lang.Id, Proficiency = lang.Proficiency });
            }

            // Load player data if available
            HasPlayer = person.Player != null;
            if (person.Player != null)
            {
                LoadFromPlayer(person.Player);
            }
            else
            {
                ResetPlayerFields();
            }
        }

        private void LoadFromPlayer(Player player)
        {
            CA = player.CA;
            PA = player.PA;
            Height = player.Height;
            Weight = player.Weight;
            Pace = player.Pace;
            Stamina = player.Stamina;
            Strength = player.Strength;
            Agility = player.Agility;
            Jumping = player.Jumping;
            Finishing = player.Finishing;
            Passing = player.Passing;
            Dribbling = player.Dribbling;
            Tackling = player.Tackling;
            Heading = player.Heading;
            Crossing = player.Crossing;
            Technique = player.Technique;
            Creativity = player.Creativity;
            Leadership = player.Leadership;
            WorkRate = player.WorkRate;
            Positioning = player.Positioning;
            Movement = player.Movement;
            Flair = player.Flair;
            Decision = player.Decision;
            LongShot = player.LongShot;
            SetPieces = player.SetPieces;
            LeftFoot = player.LeftFoot;
            RightFoot = player.RightFoot;
            Unselfishness = player.Unselfishness;
            Consistency = player.Consistency;
            Aggression = player.Aggression;
            BigMatch = player.BigMatch;
            InjuryProne = player.InjuryProne;
            Versatility = player.Versatility;
            Penalty = player.Penalty;

            // Goalkeeper attributes
            Handling = player.Handling;
            Kicking = player.Kicking;
            Aerial = player.Aerial;
            Reflexes = player.Reflexes;
            Communication = player.Communication;
            Throwing = player.Throwing;

            // Positions
            GK = player.GK;
            LIB = player.LIB;
            LB = player.LB;
            CB = player.CB;
            RB = player.RB;
            DM = player.DM;
            LM = player.LM;
            CM = player.CM;
            RM = player.RM;
            LW = player.LW;
            AM = player.AM;
            RW = player.RW;
            CF = player.CF;
            LWB = player.LWB;
            RWB = player.RWB;

            // Reputation
            HomeReputation = player.HomeReputation;
            CurrentReputation = player.CurrentReputation;
            WorldReputation = player.WorldReputation;

            // Other
            InternationalRetirement = player.InternationalRetirement;
            SquadNumber = player.SquadNumber;
            PreferredSquadNumber = player.PreferredSquadNumber;
        }

        private void ResetToDefaults()
        {
            using var _ = new OtherNationalitySyncScope(this);

            Uid = null;
            FirstNameId = null;
            LastNameId = null;
            CommonNameId = null;
            NationId = null;
            SecondNationality = null;
            ThirdNationality = null;
            OtherNationalityCount = 0;
            ClubId = null;
            JoinedDate = new DateTime(2025, 05, 30);
            DateOfBirth = new DateTime(2000, 1, 1);
            PersonType = 1;
            Unknown1 = -1;
            UnknownDate = new DateTime(1900, 1, 1);
            Ethnicity = 0;
            NationalCaps = 0;
            NationalGoals = 0;
            NationalU21Caps = 0;
            NationalU21Goals = 0;
            Adaptability = 10;
            Ambition = 10;
            Controversy = 10;
            Loyalty = 10;
            Pressure = 10;
            Professionalism = 10;
            Sportmanship = 10;
            Temperament = 10;
            DefaultLanguageCount = 0;
            OtherLanguageCount = 0;
            RelationshipCount = 0;
            DefaultLanguages.Clear();
            OtherLanguages.Clear();
            OtherNationalities.Clear();
            HasPlayer = true;  // Always create a player by default
            ResetPlayerFieldsToDefaults();
            UpdateOtherNationalityFieldsFromCollection();
        }

        private void ResetPlayerFieldsToDefaults()
        {
            CA = 50;
            PA = 100;
            Height = 180;
            Weight = 75;
            Pace = 10;
            Stamina = 10;
            Strength = 10;
            Agility = 10;
            Jumping = 10;
            Finishing = 10;
            Passing = 10;
            Dribbling = 10;
            Tackling = 10;
            Heading = 10;
            Crossing = 10;
            Technique = 10;
            Creativity = 10;
            Leadership = 10;
            WorkRate = 10;
            Positioning = 10;
            Movement = 10;
            Flair = 10;
            Decision = 10;
            LongShot = 10;
            SetPieces = 10;
            LeftFoot = 10;
            RightFoot = 10;
            Unselfishness = 10;
            Consistency = 10;
            Aggression = 10;
            BigMatch = 10;
            InjuryProne = 10;
            Versatility = 10;
            Penalty = 10;

            // Goalkeeper attributes
            Handling = 10;
            Kicking = 10;
            Aerial = 10;
            Reflexes = 10;
            Communication = 10;
            Throwing = 10;

            // Positions - default to 1
            GK = 1;
            LIB = 1;
            LB = 1;
            CB = 1;
            RB = 1;
            DM = 1;
            LM = 1;
            CM = 1;
            RM = 1;
            LW = 1;
            AM = 1;
            RW = 1;
            CF = 1;
            LWB = 1;
            RWB = 1;

            // Reputation
            HomeReputation = 0;
            CurrentReputation = 0;
            WorldReputation = 0;

            // Other
            InternationalRetirement = 0;
            SquadNumber = 0;
            PreferredSquadNumber = 0;
        }

        private void ResetPlayerFields()
        {
            CA = null;
            PA = null;
            Height = null;
            Weight = null;
            Pace = null;
            Stamina = null;
            Strength = null;
            Agility = null;
            Jumping = null;
            Finishing = null;
            Passing = null;
            Dribbling = null;
            Tackling = null;
            Heading = null;
            Crossing = null;
            Technique = null;
            Creativity = null;
            Leadership = null;
            WorkRate = null;
            Positioning = null;
            Movement = null;
            Flair = null;
            Decision = null;
            LongShot = null;
            SetPieces = null;
            LeftFoot = null;
            RightFoot = null;
            Unselfishness = null;
            Consistency = null;
            Aggression = null;
            BigMatch = null;
            InjuryProne = null;
            Versatility = null;
            Penalty = null;

            // Goalkeeper attributes
            Handling = null;
            Kicking = null;
            Aerial = null;
            Reflexes = null;
            Communication = null;
            Throwing = null;

            // Positions
            GK = null;
            LIB = null;
            LB = null;
            CB = null;
            RB = null;
            DM = null;
            LM = null;
            CM = null;
            RM = null;
            LW = null;
            AM = null;
            RW = null;
            CF = null;
            LWB = null;
            RWB = null;

            // Reputation
            HomeReputation = null;
            CurrentReputation = null;
            WorldReputation = null;

            // Other
            InternationalRetirement = null;
            SquadNumber = null;
            PreferredSquadNumber = null;
        }

        private void UpdateOtherNationalitiesFromFields()
        {
            if (_isSyncingOtherNationalities)
                return;

            _isSyncingOtherNationalities = true;
            OtherNationalities.Clear();
            if (SecondNationality.HasValue)
                OtherNationalities.Add(new NationalityEntry { NationId = SecondNationality.Value });
            if (ThirdNationality.HasValue)
                OtherNationalities.Add(new NationalityEntry { NationId = ThirdNationality.Value });
            OtherNationalityCount = (short)OtherNationalities.Count;
            _isSyncingOtherNationalities = false;
        }

        private void UpdateOtherNationalityFieldsFromCollection()
        {
            if (_isSyncingOtherNationalities)
                return;

            _isSyncingOtherNationalities = true;
            SecondNationality = OtherNationalities.Count > 0 ? OtherNationalities[0].NationId : null;
            ThirdNationality = OtherNationalities.Count > 1 ? OtherNationalities[1].NationId : null;
            OtherNationalityCount = (short)OtherNationalities.Count;
            _isSyncingOtherNationalities = false;
        }

        private bool _isSyncingOtherNationalities;

        private sealed class OtherNationalitySyncScope : IDisposable
        {
            private readonly PersonEditViewModel _owner;
            private readonly bool _previousState;

            public OtherNationalitySyncScope(PersonEditViewModel owner)
            {
                _owner = owner;
                _previousState = owner._isSyncingOtherNationalities;
                owner._isSyncingOtherNationalities = true;
            }

            public void Dispose()
            {
                _owner._isSyncingOtherNationalities = _previousState;
            }
        }

        public bool Validate() => FirstNameId != null && LastNameId != null && NationId != null;

        private static string FormatEthnicityName(Ethnicity ethnicity)
        {
            return ethnicity switch
            {
                FMMLibrary.Ethnicity.NorthenEuropean => "Northern European",
                FMMLibrary.Ethnicity.MediteranianHispanic => "Mediterranean/Hispanic",
                FMMLibrary.Ethnicity.NorthAfricanMiddleEastern => "North African/Middle Eastern",
                FMMLibrary.Ethnicity.AfricanCaribean => "African/Caribbean",
                FMMLibrary.Ethnicity.Asian => "Asian",
                FMMLibrary.Ethnicity.SouthEastAsian => "South East Asian",
                FMMLibrary.Ethnicity.PacificIslander => "Pacific Islander",
                FMMLibrary.Ethnicity.NativeAmerican => "Native American",
                FMMLibrary.Ethnicity.NativeAustralian => "Native Australian",
                FMMLibrary.Ethnicity.MixedRace => "Mixed Race",
                FMMLibrary.Ethnicity.EastAsian => "East Asian",
                FMMLibrary.Ethnicity.Unknown => "Unknown",
                _ => ethnicity.ToString()
            };
        }
    }

    public class LanguageEntry : ReactiveObject
    {
        [Reactive] public short LanguageId { get; set; }
        [Reactive] public byte Proficiency { get; set; } = 20;
    }

    public class NationalityEntry : ReactiveObject
    {
        [Reactive] public short NationId { get; set; }
    }
}
