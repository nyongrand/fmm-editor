using FMMEditor.Converters;
using FMMLibrary;

namespace FMMEditor.Models
{
    /// <summary>
    /// Display model for People with resolved names
    /// </summary>
    public class PeopleDisplayModel(People person)
    {
        public People Person { get; set; } = person;
        public Player? Player { get; set; }
        public int Uid => Person.Uid;
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string CommonName { get; set; } = "";
        public string FullName => !string.IsNullOrEmpty(CommonName) ? CommonName : $"{LastName}, {FirstName}".Trim();
        public string NationName { get; set; } = "";
        public string ClubName { get; set; } = "";
        public string DateOfBirth => DateConverter.ToDateString(Person.DateOfBirth);
        public string PersonType => Person.Type switch { 0 => "Non-player", 1 => "Player", _ => $"Type: {Person.Type}" };
        public short NationalCaps => Person.NationalCaps;
        public short NationalGoals => Person.NationalGoals;
        public byte Adaptability => Person.Adaptability;
        public byte Ambition => Person.Ambition;
        public byte Loyalty => Person.Loyality;
        public byte Pressure => Person.Pressure;
        public byte Professionalism => Person.Professionalism;
        public byte Temperament => Person.Temperament;
        public string Ethnicity => EthnicityConverter.ToDisplayString(Person.Ethnicity);

        // Player attributes (null if not a player)
        public short? CurrentAbility => Player?.CA;
        public short? PotentialAbility => Player?.PA;
        public short? Height => Player?.Height;
        public short? Weight => Player?.Weight;
        public byte? Pace => Player?.Pace;
        public byte? Finishing => Player?.Finishing;
        public byte? Passing => Player?.Passing;
        public byte? Dribbling => Player?.Dribbling;
        public byte? Tackling => Player?.Tackling;
        public byte? Heading => Player?.Heading;
        public byte? Crossing => Player?.Crossing;
        public byte? Technique => Player?.Technique;
        public byte? Creativity => Player?.Creativity;
        public byte? Stamina => Player?.Stamina;
        public byte? Strength => Player?.Strength;
        public byte? Leadership => Player?.Leadership;
        public byte? Agility => Player?.Agility;
        public byte? Jumping => Player?.Jumping;
        public byte? WorkRate => Player?.WorkRate;
        public byte? Positioning => Player?.Positioning;
        public byte? Movement => Player?.Movement;
        public byte? Flair => Player?.Flair;
        public byte? Decision => Player?.Decision;
        public byte? LongShot => Player?.LongShot;
        public byte? SetPieces => Player?.SetPieces;
        public byte? LeftFoot => Player?.LeftFoot;
        public byte? RightFoot => Player?.RightFoot;
    }
}
