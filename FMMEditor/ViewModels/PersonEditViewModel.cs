using FMMEditor.Collections;
using FMMEditor.Converters;
using FMMEditor.Models;
using FMMLibrary;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public BulkObservableCollection<Club> Clubs { get; }

        // Ethnicity options
        public List<EthnicityOption> EthnicityOptions { get; } = Enum.GetValues<Ethnicity>()
            .Select(e => new EthnicityOption { Value = (byte)e, DisplayName = FormatEthnicityName(e) })
            .ToList();

        // Person fields
        [Reactive] public int? Uid { get; set; }
        [Reactive] public int? FirstNameId { get; set; }
        [Reactive] public int? LastNameId { get; set; }
        [Reactive] public int? CommonNameId { get; set; }
        [Reactive] public short? NationId { get; set; }
        [Reactive] public int? ClubId { get; set; }
        [Reactive] public DateTime? DateOfBirth { get; set; }
        [Reactive] public int PersonType { get; set; }
        [Reactive] public byte Ethnicity { get; set; }
        [Reactive] public short? NationalCaps { get; set; }
        [Reactive] public short? NationalGoals { get; set; }
        [Reactive] public byte? NationalU21Caps { get; set; }
        [Reactive] public byte? NationalU21Goals { get; set; }

        // Personality attributes
        [Reactive] public byte? Adaptability { get; set; }
        [Reactive] public byte? Ambition { get; set; }
        [Reactive] public byte? Loyalty { get; set; }
        [Reactive] public byte? Pressure { get; set; }
        [Reactive] public byte? Professionalism { get; set; }
        [Reactive] public byte? Temperament { get; set; }

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

        // Foot preference
        [Reactive] public byte? LeftFoot { get; set; }
        [Reactive] public byte? RightFoot { get; set; }

        public PersonEditViewModel(
            BulkObservableCollection<Name> firstNames,
            BulkObservableCollection<Name> lastNames,
            BulkObservableCollection<Name> commonNames,
            BulkObservableCollection<Nation> nations,
            BulkObservableCollection<Club> clubs)
        {
            FirstNames = firstNames;
            LastNames = lastNames;
            CommonNames = commonNames;
            Nations = nations;
            Clubs = clubs;

            this.WhenAnyValue(x => x.IsAddMode)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
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

        private void LoadFromPerson(PeopleDisplayModel person)
        {
            var p = person.Person;

            Uid = p.Uid;
            FirstNameId = p.FirstNameId;
            LastNameId = p.LastNameId;
            CommonNameId = p.CommonNameId > 0 ? p.CommonNameId : null;
            NationId = p.NationId;
            ClubId = p.ClubId;
            DateOfBirth = DateConverter.ToDateTime(p.DateOfBirth);
            PersonType = p.Type;
            Ethnicity = p.Ethnicity;
            NationalCaps = p.NationalCaps;
            NationalGoals = p.NationalGoals;
            NationalU21Caps = p.NationalU21Caps;
            NationalU21Goals = p.NationalU21Goals;
            Adaptability = p.Adaptability;
            Ambition = p.Ambition;
            Loyalty = p.Loyality;
            Pressure = p.Pressure;
            Professionalism = p.Professionalism;
            Temperament = p.Temperament;

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
        }

        private void ResetToDefaults()
        {
            Uid = null;
            FirstNameId = null;
            LastNameId = null;
            CommonNameId = null;
            NationId = null;
            ClubId = null;
            DateOfBirth = new DateTime(1990, 1, 1);
            PersonType = 0;
            Ethnicity = 0;
            NationalCaps = 0;
            NationalGoals = 0;
            NationalU21Caps = 0;
            NationalU21Goals = 0;
            Adaptability = 10;
            Ambition = 10;
            Loyalty = 10;
            Pressure = 10;
            Professionalism = 10;
            Temperament = 10;
            HasPlayer = false;
            ResetPlayerFields();
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

    public class EthnicityOption
    {
        public byte Value { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
